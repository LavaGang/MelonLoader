#include <mach/mach.h>
#include <mach/mach_vm.h>
#include <sys/mman.h>
#include <unistd.h>
#include <pthread.h>
#include <libkern/OSCacheControl.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

extern "C" int MLMacOSJitCopy(void*, const void*, size_t);

constexpr size_t patchSize = 512;
struct WriterContext {
    uint8_t* destination;
    uint8_t value;
    int* failure;
    pthread_mutex_t* failureMutex;
};

static void* write_concurrently(void* rawContext)
{
    auto context = (WriterContext*)rawContext;
    uint8_t patch[patchSize];
    memset(patch, context->value, sizeof(patch));
    for (int iteration = 0; iteration < 100; ++iteration) {
        if (MLMacOSJitCopy(context->destination, patch, sizeof(patch)) != 1) {
            pthread_mutex_lock(context->failureMutex);
            *context->failure = 1;
            pthread_mutex_unlock(context->failureMutex);
        }
    }
    return nullptr;
}

static void check(bool ok, const char* message)
{
    if (!ok) {
        fprintf(stderr, "FAIL: %s\n", message);
        exit(1);
    }
    printf("PASS: %s\n", message);
}

static vm_prot_t protection(void* pointer)
{
    mach_vm_address_t address = (mach_vm_address_t)pointer;
    mach_vm_size_t size = 0;
    vm_region_basic_info_data_64_t info = {};
    mach_msg_type_number_t count = VM_REGION_BASIC_INFO_COUNT_64;
    mach_port_t object = MACH_PORT_NULL;
    kern_return_t result = mach_vm_region(mach_task_self(), &address, &size,
        VM_REGION_BASIC_INFO_64, (vm_region_info_t)&info, &count, &object);
    if (object != MACH_PORT_NULL) mach_port_deallocate(mach_task_self(), object);
    check(result == KERN_SUCCESS && address <= (mach_vm_address_t)pointer, "query mapped page");
    return info.protection;
}

int main()
{
    const size_t page = getpagesize();
    check(MLMacOSJitCopy(nullptr, nullptr, 0) == 1, "capability probe");
    uint32_t before[] = {0x528000e0, 0xd65f03c0}; // mov w0,#7; ret
    uint32_t after[] = {0x52800120, 0xd65f03c0};  // mov w0,#9; ret
    check(MLMacOSJitCopy(nullptr, after, sizeof(after)) == 0, "reject null destination");
    check(MLMacOSJitCopy((void*)1, after, sizeof(after)) == 0, "reject unmapped destination");
    check(MLMacOSJitCopy((void*)after, (void*)1, sizeof(after)) == 0, "reject unmapped source");

    auto data = (uint8_t*)mmap(nullptr, page * 2, PROT_READ | PROT_WRITE,
        MAP_ANON | MAP_PRIVATE, -1, 0);
    check(data != MAP_FAILED, "allocate private mapping");
    auto original = protection(data);
    check(MLMacOSJitCopy(data, before, sizeof(before)) == 1, "copy writable data");
    check(memcmp(data, before, sizeof(before)) == 0 && protection(data) == original,
        "preserve writable protection and bytes");
    uint8_t overlap[24];
    for (size_t i = 0; i < sizeof(overlap); ++i) overlap[i] = (uint8_t)i;
    memcpy(data, overlap, sizeof(overlap));
    memmove(overlap + 4, overlap, 16);
    check(MLMacOSJitCopy(data + 4, data, 16) == 1 && memcmp(data, overlap, sizeof(overlap)) == 0,
        "overlapping source uses memmove semantics");

    memcpy(data, before, sizeof(before));
    check(mprotect(data, page, PROT_READ | PROT_EXEC) == 0, "protect code RX");
    using Fn = int(*)();
    auto fn = (Fn)data;
    sys_icache_invalidate(data, sizeof(before));
    check(fn() == 7, "execute initial RX code");
    check(MLMacOSJitCopy(data, after, sizeof(after)) == 1, "patch RX code");
    check(fn() == 9 && protection(data) == (VM_PROT_READ | VM_PROT_EXECUTE),
        "execute patched code and restore RX");

    uint8_t cross[16];
    memset(cross, 0x5a, sizeof(cross));
    check(MLMacOSJitCopy(data + page - 8, cross, sizeof(cross)) == 1,
        "copy across RX/RW page boundary");
    check(memcmp(data + page - 8, cross, sizeof(cross)) == 0, "cross-page bytes correct");
    check(protection(data) == (VM_PROT_READ | VM_PROT_EXECUTE) &&
        protection(data + page) == original, "restore both page protections");

    int concurrentFailure = 0;
    pthread_mutex_t failureMutex = PTHREAD_MUTEX_INITIALIZER;
    pthread_t writers[6];
    WriterContext contexts[6];
    for (uint8_t value = 1; value <= 6; ++value) {
        contexts[value - 1] = {data, value, &concurrentFailure, &failureMutex};
        check(pthread_create(&writers[value - 1], nullptr, write_concurrently,
            &contexts[value - 1]) == 0, "start concurrent helper writer");
    }
    for (auto& writer : writers) check(pthread_join(writer, nullptr) == 0, "join concurrent helper writer");
    bool uniform = data[0] >= 1 && data[0] <= 6;
    for (size_t i = 1; i < patchSize; ++i) uniform = uniform && data[i] == data[0];
    check(!concurrentFailure && uniform && protection(data) == (VM_PROT_READ | VM_PROT_EXECUTE),
        "serialize concurrent helper writers and restore RX");
    munmap(data, page * 2);

    auto jit = mmap(nullptr, page, PROT_READ | PROT_WRITE | PROT_EXEC,
        MAP_ANON | MAP_PRIVATE | MAP_JIT, -1, 0);
    check(jit != MAP_FAILED, "allocate MAP_JIT page");
    check(MLMacOSJitCopy(jit, before, sizeof(before)) == 1 && ((Fn)jit)() == 7,
        "patch and execute MAP_JIT code");
    if (pthread_jit_write_protect_supported_np()) {
        pthread_jit_write_protect_np(0);
        check(MLMacOSJitCopy(jit, after, sizeof(after)) == 1,
            "patch inside caller-owned JIT write scope");
        ((volatile uint32_t*)jit)[0] = after[0];
        pthread_jit_write_protect_np(1);
        sys_icache_invalidate(jit, sizeof(after));
        check(((Fn)jit)() == 9, "preserve caller's incoming JIT write-enabled state");
    }
    check(MLMacOSJitCopy(jit, before, sizeof(before)) == 1 && ((Fn)jit)() == 7,
        "repatch MAP_JIT code after caller scope");
    munmap(jit, page);
    puts("ALL ARM64 DETOUR COPY CHECKS PASSED");
}

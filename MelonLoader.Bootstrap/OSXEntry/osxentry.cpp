#include <sys/resource.h>
#include <dlfcn.h>
#include <string.h>
#include <pthread.h>
#include <libkern/OSCacheControl.h>
#include <mach/mach.h>
#include <mach/mach_vm.h>
#include <stdint.h>
#include <stdlib.h>

extern "C"
{
    void Init();
    int SetRlimitHook(int resource, rlimit* rlp);
    void MLRegisterDlsymHook(void* detour);
    void* MLRealDlsym(void* handle, const char* symbol);
    int MLMacOSJitCopy(void* dst, const void* src, size_t len);
    void* DlsymHook(void* handle, const char* symbol);
}

#if defined(__APPLE__) && defined(__arm64__)
static pthread_mutex_t g_jitCopyMutex;
static pthread_once_t g_jitCopyMutexOnce = PTHREAD_ONCE_INIT;
static int g_jitCopyMutexInitResult = -1;

static void InitJitCopyMutex()
{
    pthread_mutexattr_t attributes;
    int result = pthread_mutexattr_init(&attributes);
    if (result == 0) {
        result = pthread_mutexattr_settype(&attributes, PTHREAD_MUTEX_RECURSIVE);
        if (result == 0)
            result = pthread_mutex_init(&g_jitCopyMutex, &attributes);
        pthread_mutexattr_destroy(&attributes);
    }
    g_jitCopyMutexInitResult = result;
}
#endif

#define DYLD_INTERPOSE(_replacement,_replacee) \
       __attribute__((used)) static struct{ const void* replacement; const void* replacee; } _interpose_##_replacee \
       __attribute__ ((section ("__DATA,__interpose"))) = { (const void*)(unsigned long)&_replacement, (const void*)(unsigned long)&_replacee };

int SetRlimitHook(int resource, rlimit* rlp)
{
    int result = setrlimit(resource, rlp);
    Init();
    return result;
}

DYLD_INTERPOSE(SetRlimitHook, setrlimit)

// Detour registered by the managed bootstrap (ModuleSymbolRedirect.Attach).
// Receives the symbol AND its already-resolved real address, and returns either
// MelonLoader's hook (for the runtime entry points) or the real address.
typedef void* (*DlsymDetourFn)(void* handle, const char* symbol, void* real);
static DlsymDetourFn g_dlsymDetour = nullptr;

void MLRegisterDlsymHook(void* detour)
{
    g_dlsymDetour = (DlsymDetourFn)detour;
}

// The real dlsym, exposed to managed code. dyld does not interpose the image
// that declares the interpose, so this call resolves the genuine export. The
// managed side MUST use this (not its own dlsym P/Invoke, whose GOT slot IS
// interposed) so MelonLoader resolves the real mono/il2cpp exports rather than
// its own detours -- otherwise the detours call themselves and recurse.
void* MLRealDlsym(void* handle, const char* symbol)
{
    return dlsym(handle, symbol);
}

int MLMacOSJitCopy(void* dst, const void* src, size_t len)
{
#if defined(__APPLE__) && defined(__arm64__)
    if (len == 0) return 1;
    if (!dst || !src || len > UINTPTR_MAX - (uintptr_t)dst ||
        len > UINTPTR_MAX - (uintptr_t)src) return 0;
    if (pthread_once(&g_jitCopyMutexOnce, InitJitCopyMutex) != 0 ||
        g_jitCopyMutexInitResult != 0 || pthread_mutex_lock(&g_jitCopyMutex) != 0) return 0;

    struct MutexUnlocker {
        ~MutexUnlocker() { pthread_mutex_unlock(&g_jitCopyMutex); }
    } mutexUnlocker;

    // Validate the complete source before changing destination protections.
    mach_vm_address_t sourceCursor = (mach_vm_address_t)src;
    const mach_vm_address_t sourceEnd = sourceCursor + len;
    while (sourceCursor < sourceEnd) {
        mach_vm_address_t region = sourceCursor;
        mach_vm_size_t size = 0;
        vm_region_basic_info_data_64_t info = {};
        mach_msg_type_number_t count = VM_REGION_BASIC_INFO_COUNT_64;
        mach_port_t object = MACH_PORT_NULL;
        kern_return_t result = mach_vm_region(mach_task_self(), &region, &size,
            VM_REGION_BASIC_INFO_64, (vm_region_info_t)&info, &count, &object);
        if (object != MACH_PORT_NULL) mach_port_deallocate(mach_task_self(), object);
        if (result != KERN_SUCCESS || region > sourceCursor || size == 0 ||
            region > UINT64_MAX - size || !(info.protection & VM_PROT_READ)) return 0;
        mach_vm_address_t stop = region + size;
        sourceCursor = stop < sourceEnd ? stop : sourceEnd;
    }

    struct Range { mach_vm_address_t start; mach_vm_size_t size; vm_prot_t protection; bool changed; };
    const size_t capacity = len / vm_page_size + 2;
    if (capacity > SIZE_MAX / sizeof(Range)) return 0;
    Range* ranges = (Range*)calloc(capacity, sizeof(Range));
    if (!ranges) return 0;
    size_t rangeCount = 0;
    mach_vm_address_t cursor = (mach_vm_address_t)dst;
    const mach_vm_address_t end = cursor + len;
    const mach_vm_size_t page = vm_page_size;
    // Validate every destination region before changing protections or bytes.
    while (cursor < end) {
        mach_vm_address_t region = cursor;
        mach_vm_size_t size = 0;
        vm_region_basic_info_data_64_t info = {};
        mach_msg_type_number_t count = VM_REGION_BASIC_INFO_COUNT_64;
        mach_port_t object = MACH_PORT_NULL;
        kern_return_t result = mach_vm_region(mach_task_self(), &region, &size,
            VM_REGION_BASIC_INFO_64, (vm_region_info_t)&info, &count, &object);
        if (object != MACH_PORT_NULL) mach_port_deallocate(mach_task_self(), object);
        if (result != KERN_SUCCESS || region > cursor || size == 0 ||
            region > UINT64_MAX - size || !(info.protection & VM_PROT_READ)) {
            free(ranges);
            return 0;
        }
        mach_vm_address_t stop = region + size < end ? region + size : end;
        mach_vm_address_t firstPage = cursor & ~(page - 1);
        mach_vm_address_t lastPage = (stop - 1) & ~(page - 1);
        if (rangeCount == capacity) {
            free(ranges);
            return 0;
        }
        ranges[rangeCount++] = {firstPage, lastPage - firstPage + page, info.protection, false};
        cursor = stop;
    }
    bool ready = true;
    for (size_t i = 0; i < rangeCount; ++i) {
        Range& range = ranges[i];
        // Remove execute while writing, including from MAP_JIT pages reported
        // as RWX. This preserves the caller's per-thread JIT-write state and
        // never creates an RWX transition.
        if ((range.protection & VM_PROT_WRITE) && !(range.protection & VM_PROT_EXECUTE)) continue;
        vm_prot_t writable = (range.protection & ~VM_PROT_EXECUTE) | VM_PROT_WRITE | VM_PROT_COPY;
        if (mach_vm_protect(mach_task_self(), range.start, range.size, false, writable) != KERN_SUCCESS) {
            ready = false;
            break;
        }
        range.changed = true;
    }
    if (ready) {
        memmove(dst, src, len);
        sys_icache_invalidate(dst, len);
    }
    bool restored = true;
    for (size_t i = rangeCount; i > 0; --i) {
        Range& range = ranges[i - 1];
        if (range.changed && mach_vm_protect(mach_task_self(), range.start, range.size,
                false, range.protection) != KERN_SUCCESS) restored = false;
    }
    free(ranges);
    return ready && restored ? 1 : 0;
#else
    return 0;
#endif
}

// The game's engine (UnityPlayer.dylib) resolves the mono/il2cpp entry points
// via dlsym, so we interpose dlsym to return MelonLoader's detours for them.
// plthook cannot patch UnityPlayer.dylib's GOT on modern macOS
// (LC_DYLD_CHAINED_FIXUPS); DYLD interpose is the mechanism that reliably works
// here (it is what triggers Init via setrlimit). Only runtime-entry symbols
// (mono_/il2cpp_) are handed to managed code -- routing every dlsym in the
// process (e.g. AppKit's during startup) through a managed callback is needless
// and unsafe.
void* DlsymHook(void* handle, const char* symbol)
{
    void* real = dlsym(handle, symbol);
    if (g_dlsymDetour != nullptr && symbol != nullptr &&
        (strncmp(symbol, "mono_", 5) == 0 || strncmp(symbol, "il2cpp_", 7) == 0))
        return g_dlsymDetour(handle, symbol, real);
    return real;
}

DYLD_INTERPOSE(DlsymHook, dlsym)

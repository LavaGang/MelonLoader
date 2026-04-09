#include <sys/resource.h>

extern "C"
{
    void Init();
    int SetRlimitHook(int resource, rlimit* rlp);
}

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

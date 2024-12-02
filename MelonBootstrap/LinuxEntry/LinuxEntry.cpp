#include <stdio.h>
#include <dlfcn.h>

extern "C"
{
    // Dobby: "hook and commit function"
    int DobbyHook(void *address, void *replace_call, void **origin_call);

    // Init from NativeAOT
    void Init(void *hBootstrap);
}

typedef void (*PlayerMain)(int a, char **b);
PlayerMain original;

void detour(int a, char **b)
{
    Dl_info dl_info;
    dladdr(reinterpret_cast<void*>(detour), &dl_info);

    Init(dl_info.dli_fbase);

    return original(a, b);
}

__attribute__((constructor))
void library_init()
{
    void *hUnity = dlopen("UnityPlayer.so", RTLD_NOW);
    if (hUnity == nullptr)
    {
        printf("Failed to load UnityPlayer");
        return;
    }
    
    void *entry = dlsym(hUnity, "_Z10PlayerMainiPPc");
    if (entry == nullptr)
    {
        printf("Failed to get PlayerMain func");
        return;
    }

    int a = DobbyHook(entry, reinterpret_cast<void*>(&detour), reinterpret_cast<void**>(&original));
    if (a != 0)
        printf("Failed to hook PlayerMain: %d\n", a);
}
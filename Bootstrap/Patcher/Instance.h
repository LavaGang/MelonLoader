#pragma once
#include <stdint.h>
#include <stdbool.h>

struct DetourDef
{
    bool enabled;
    void* detour;
};

struct Patch
{
    bool enabled;
    void* main;
    __uint32_t count;
    struct DetourDef** detours;
};

struct PatchMapItem {
    __uintptr_t key;
    struct Patch* value;
};


struct PatchMap
{
    __uint32_t size;
    struct PatchMapItem** items;
};

__uint32_t patcher_instance_getHash(__uint32_t size, void* address);

bool patcher_instance_findItem_index(struct PatchMap* instance, void* address, struct PatchMapItem** output, __uint32_t* index);

bool patcher_instance_findItem(struct PatchMap* instance, void* address, struct PatchMapItem** output);

bool patcher_instance_createItem(struct PatchMap* instance, void* address, struct PatchMapItem** output);

void patcher_instance_removeItem(struct PatchMap* instance, void* address);

struct PatchMap* patcher_instance_create(__uint32_t size);

bool patcher_instance_createPatch(struct PatchMap* instance, void* address, struct PatchMapItem** output);

bool patcher_instance_findDetour(struct PatchMap* instance, void* address, void* detour, struct PatchMapItem** output, struct DetourDef** res);

bool patcher_instance_registerDetour(struct PatchMap* instance, void* address, void* detour, struct PatchMapItem** output, struct DetourDef** ref);

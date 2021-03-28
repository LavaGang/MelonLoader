#include <cstdlib>

#include "./Instance.h"

struct PatchMapItem patcher_instance_emptyItem;

__uint32_t patcher_instance_getHash(__uint32_t size, void* address)
{
    return (__int64_t)address % size;
}

bool patcher_instance_findItem_index(struct PatchMap* instance, void* address, struct PatchMapItem* &output, __uint32_t &index)
{
    struct PatchMapItem** items = instance->items;
	
    __uint32_t i = patcher_instance_getHash(instance->size, address);
	for (; i < instance->size; i++)
	{
		if (items[i]->key != (__uintptr_t)address)
            continue;

        if (items[i]->key == (__uintptr_t)NULL)
            break;

        output = items[i];
        index = i;
        return true;
	}

    return false;
}

bool patcher_instance_findItem(struct PatchMap* instance, void* address, struct PatchMapItem* &output)
{
    __uint32_t index = 0;
    return patcher_instance_findItem_index(instance, address, output, index);
}

bool patcher_instance_createItem(struct PatchMap* instance, void* address, struct PatchMapItem* &output)
{
    if (patcher_instance_findItem(instance, address, output))
        return true;

    struct PatchMapItem** items = instance->items;

    __uint32_t i = patcher_instance_getHash(instance->size, address);
    for (; i < instance->size; i++)
    {
        if (items[i]->key != (__uintptr_t)NULL)
            continue;

        output = items[i] = (struct PatchMapItem*)malloc(sizeof(struct PatchMapItem));
        items[i]->key = (__uintptr_t)address;
    	
        return true;
    }

    return false;
}

// todo: this does not account for mixed hashes
// hit - hit - miss - hit
// del - hit - miss - hit
// hit - del - miss - hit
// the last hit, doesn't get checked
void patcher_instance_removeItem(struct PatchMap* instance, void* address)
{
    __uint32_t i;
    struct PatchMapItem* output = NULL;
	
    if (!patcher_instance_findItem_index(instance, address, &output, &i))
        return;

    const __uint32_t itemHash = patcher_instance_getHash(instance->size, address);
	
    __uint32_t lastShiftedHash = i;
    __uint32_t hash;
    struct PatchMapItem** items = instance->items;

    for (; i < instance->size; i++)
    {
        if (items[i]->key == (__uintptr_t)NULL)
            break;

        hash = patcher_instance_getHash(instance->size, (void*)items[i]->key);
        if (hash != itemHash)
            break;

        memcpy(instance->items[lastShiftedHash], instance->items[i], sizeof(struct PatchMapItem));
        memcpy(instance->items[i], &patcher_instance_emptyItem, sizeof(struct PatchMapItem));
    	
        lastShiftedHash = i;
    }
	
    memcpy(instance->items[lastShiftedHash], &patcher_instance_emptyItem, sizeof(struct PatchMapItem));
}

struct PatchMap* patcher_instance_create(__uint32_t size)
{
    struct PatchMap* res = malloc(sizeof(struct PatchMap));
    res->size = size;
    res->items = malloc(sizeof(__uintptr_t) * size);
    return res;
}

bool patcher_instance_createPatch(struct PatchMap* instance, void* address, struct PatchMapItem** output)
{
    if (
        patcher_instance_findItem(instance, address, output) || 
        !patcher_instance_createItem(instance, address, output))
    {
    	return false;
    }

    (*output)->value = malloc(sizeof(struct Patch));
    (*output)->value->main = address;

    return true;
}

bool patcher_instance_findDetour(struct PatchMap* instance, void* address, void* detour, struct PatchMapItem** output, struct DetourDef** res)
{
    if (!patcher_instance_findItem(instance, address, output))
    {
        return false;
    }

    __uint32_t i;
    for (i = 0; i < (*output)->value->count; i++)
    {
        if ((*output)->value->detours[i]->detour == detour)
        {
            *res = (*output)->value->detours[i];
            return true;
        }
    }

    return false;
}

bool patcher_instance_registerDetour(struct PatchMap* instance, void* address, void* detour, struct PatchMapItem** output, struct DetourDef** ref)
{
	if (patcher_instance_findDetour(instance, address, detour, output, ref))
        return false;

    (*output)->value->count++;
    (*output)->value->detours = realloc((*output)->value->detours, sizeof(__uintptr_t) * (*output)->value->count);
    *ref = (*output)->value->detours[(*output)->value->count - 1];
    (*ref)->detour = detour;

    return true;
}
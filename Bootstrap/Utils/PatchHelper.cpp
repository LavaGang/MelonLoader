#include "./PatchHelper.h"

#include <android/log.h>


#include "Logger.h"

ks_engine* PatchHelper::ks = nullptr;

bool PatchHelper::Init()
{
	ks_err err;
	size_t count;
	unsigned char* encode;
	size_t size;

	err = ks_open(KS_ARCH_ARM64, KS_MODE_LITTLE_ENDIAN, &ks);
	if (err != KS_ERR_OK) {
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "ERROR: failed on ks_open(), quit %s", ks_strerror(err));
		return false;
	}

	return true;
}

// THIS IS INEFFICIENT. YOU DONT NEED A LIBRARY TO DO IT. I JUST DONT KNOW HOW TO GENERATE THE ASSEMBLY BINARY.
bool PatchHelper::GenerateAsm(void* fnPtr, unsigned char** encode, size_t* size)
{
	// MOVZ x11, 0x155c
	// MOVK x11, 0x5af2, lsl 16
	// MOVK x11, 0x0072, lsl 32
	// MOVK x11, 0x0000, lsl 48
	// BR x11
	const char* sourceCode =
		"movz x11,%#04x\n"
		"movk x11,%#04x,lsl 16\n"
		"movk x11,%#04x,lsl 32\n"
		"movk x11,%#04x,lsl 48\n"
		"BR x11";

	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%#x", fnPtr);

	// max output is 88 bytes
	char buffer[88];
	sprintf(buffer, sourceCode, (uint64_t)fnPtr & 0xFFFF, ((uint64_t)fnPtr >> 16) & 0xFFFF, ((uint64_t)fnPtr >> 32) & 0xFFFF, ((uint64_t)fnPtr >> 48) & 0xFFFF);

	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%s", buffer);

	size_t count;

	if (ks_asm(ks, buffer, 0, encode, size, &count) != KS_ERR_OK) {
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "ERROR: ks_asm() failed & count = %lu, error = %u",count, ks_errno(ks));
	}
	else {
		// size_t i;
		//
		// for (i = 0; i < *size; i++) {
		// 	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%02x ", (char *)(*encode)[i]);
		// }
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Compiled: %lu bytes, statements: %lu\n", *size, count);
	}

	return true;
}

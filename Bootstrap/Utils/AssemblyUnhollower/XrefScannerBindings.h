#pragma once
#include <stdint.h>
#include <unordered_map>
#include <capstone/capstone.h>
#include <unicorn/unicorn.h>

class XrefScannerBindings
{
public:
	enum XrefType
	{
		Global,
		Method,
	};

	struct XrefScanImplNativeRes {
		XrefType type;
		bool complete;
		void* target;
		void* codeStart;
	};

	static bool Init();

	class XrefScanner {
	public:
		static void XrefScanImplNative(void* codeStart, bool skipClassCheck, XrefScanImplNativeRes& res);
	};

	class XrefScannerLowLevel {
	public:
		static void* JumpTargetsImpl(void* codeStart);
	};

	class XrefScanUtilFinder {
	public:
		static void* FindLastRcxReadAddressBeforeCallTo(void* codeStart, void* callTarget);
		static void* FindByteWriteTargetRightAfterCallTo(void* codeStart, void* callTarget);
	};

	struct disassemblyInstance {
		void* codeStart;
		bool use_cs;
		cs_insn* cs_ins;
		size_t cs_len;
		size_t counter;
		bool exit;
	};

	static csh cs;
	static uc_engine* uc;
private:
	
	static std::unordered_map<void*, disassemblyInstance*> disMap;
	static void CheckEntry(void* codeStart);
	static bool CheckCapstone(void* codeStart);
	static void CleanupCapstoneInstance(void* codeStart);
	static void Cleanup(void* codeStart);
	static void PartialCleanup(void* codeStart);

	class Utils
	{
	public:
		static bool HasGroup(uint8_t groups[8], size_t groupCount, uint8_t group);
		static bool RegInteracted(uint16_t registers[20], size_t reg_count, uint16_t reg);
		static void* ExtractTargetAddress(disassemblyInstance* dis, cs_insn& instruction);
	};
};

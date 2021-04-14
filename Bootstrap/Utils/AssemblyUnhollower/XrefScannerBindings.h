#pragma once
#include <stdint.h>
#include <unordered_map>
#include <capstone/capstone.h>

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

	struct DisassemblyInstance {
		void* codeStart;
		size_t dependants;
		bool exit;

		// capstone data
		bool use_cs;
		cs_insn* cs_ins;
		size_t cs_len;
		uint64_t cs_addr;
	};

	struct DecoderSettings
	{
		void* codeStart;
		uint64_t transaction;
		int limit;
	};

	static bool Init();

	class XrefScanner {
	public:
		static void XrefScanImplNative(const DecoderSettings* decoder, bool skipClassCheck, XrefScanImplNativeRes& res);
	};

	class XrefScannerLowLevel {
	public:
		static void* JumpTargetsImpl(const DecoderSettings* decoder);
	};

	class XrefScanUtilFinder {
	public:
		static void* FindLastRcxReadAddressBeforeCallTo(const DecoderSettings*, void* callTarget);
		static void* FindByteWriteTargetRightAfterCallTo(const DecoderSettings*, void* callTarget);
	};

	static csh cs;
private:
	
	static std::unordered_map<uint64_t, DisassemblyInstance*> disassemble;
	static std::unordered_map<uint64_t, size_t*> transactions;
	static DisassemblyInstance* CheckEntry(const DecoderSettings*);
	static bool CheckCapstone(const DecoderSettings*);
	static void CleanupCapstoneInstance(const DecoderSettings*);
	static void Cleanup(const DecoderSettings*);
	static void PartialCleanup(const DecoderSettings*);
	static DisassemblyInstance* GetDisassembly(const DecoderSettings*);
	static size_t* GetCounter(const DecoderSettings*);

	class Utils
	{
	public:
		static bool HasGroup(uint8_t groups[8], size_t groupCount, uint8_t group);
		static bool RegInteracted(uint16_t registers[20], size_t reg_count, uint16_t reg);
		static void* ExtractTargetAddress(DisassemblyInstance* dis, cs_insn& instruction);
	};
};

#include "XrefScannerBindings.h"
#include "../Console/Logger.h"
#include "../Console/Debug.h"
#include "../../Managers/Il2Cpp.h"
#include <android/log.h>
#include <string>
#include <math.h>
#include <funchook.h>

// 4kb
#define MIN_PAGE_SIZE 0x1000
// 4mb
#define EMU_MEM_SIZE MIN_PAGE_SIZE
#define EMU_DEFAULT_ADDRESS 0x1000000

#define HAS_GROUP(x) Utils::HasGroup(instruction.detail->groups, instruction.detail->groups_count, x)
#define HEX(x) ((x) < 10 ? (x) + '0' : (x) - 10 + 'A')
#define PRIxPTR ""
#define ADDR_FMT "%08" PRIxPTR

csh XrefScannerBindings::cs = NULL;
uc_engine* XrefScannerBindings::uc = NULL;
std::unordered_map<void*, XrefScannerBindings::disassemblyInstance*> XrefScannerBindings::disMap;

static void funchook_logv(int set_error, const char* fmt, va_list ap)
{
	__android_log_vprint(ANDROID_LOG_INFO, "MelonLoader", fmt, ap);

}

void funchook_log(const char* fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	funchook_logv(0, fmt, ap);
	va_end(ap);
}


static const char* reg_name(csh handle, unsigned int reg_id)
{
	const char* name = cs_reg_name(handle, reg_id);
	return name ? name : "?";
}

static const char* group_name(csh handle, unsigned int grp_id)
{
	const char* name = cs_group_name(handle, grp_id);
	return name ? name : "?";
}

void funchook_disasm_log_instruction(cs_insn* insn)
{
	char hex[sizeof(insn->bytes) * 3];
	uint16_t i;

	for (i = 0; i < insn->size; i++) {
		hex[i * 3 + 0] = HEX(insn->bytes[i] >> 4);
		hex[i * 3 + 1] = HEX(insn->bytes[i] & 0x0F);
		hex[i * 3 + 2] = ' ';
	}
	hex[insn->size * 3 - 1] = '\0';
	funchook_log("    %p (%02d) %-24s %s%s%s", (size_t)insn->address, insn->size, hex, insn->mnemonic, insn->op_str[0] ? " " : "", insn->op_str);
	
	cs_detail* detail = insn->detail;
	if (detail == NULL) {
		return;
	}
	if (detail->regs_read_count > 0) {
		funchook_log("        regs_read:");
		for (i = 0; i < insn->detail->regs_read_count; i++) {
			funchook_log(" %s", reg_name(XrefScannerBindings::cs, insn->detail->regs_read[i]));
		}
		funchook_log("");
	}
	if (detail->regs_write_count > 0) {
		funchook_log("        regs_write:");
		for (i = 0; i < insn->detail->regs_write_count; i++) {
			funchook_log(" %s", reg_name(XrefScannerBindings::cs, insn->detail->regs_write[i]));
		}
		funchook_log("\n");
	}
	if (detail->groups_count > 0) {
		funchook_log("        groups:");
		for (i = 0; i < insn->detail->groups_count; i++) {
			funchook_log("			%s", group_name(XrefScannerBindings::cs, insn->detail->groups[i]));
		}
	}
}
bool XrefScannerBindings::Init()
{
	if (cs_open(CS_ARCH_ARM64, CS_MODE_LITTLE_ENDIAN, &cs) != CS_ERR_OK)
	{
		Logger::Error("Failed to load capstone");
		return false;
	}

	cs_option(cs, CS_OPT_DETAIL, CS_OPT_ON);

	uc_err ucErr;
	if ((ucErr = uc_open(UC_ARCH_ARM64, UC_MODE_LITTLE_ENDIAN, &uc)) != UC_ERR_OK) {
		Logger::Error("uc_open() failed");
		Logger::Error(uc_strerror(ucErr));
		return false;
	}

	return true;
}

bool XrefScannerBindings::Utils::HasGroup(uint8_t groups[8], size_t groupCount, uint8_t group)
{
	for (int i = 0; i < groupCount; i++)
		if (groups[i] == group)
			return true;
	return false;
}

bool XrefScannerBindings::Utils::RegInteracted(uint16_t registers[20], size_t reg_count, uint16_t reg)
{
	for (int i = 0; i < reg_count; i++)
		if (registers[i] == reg)
			return true;
	return false;
}

void* XrefScannerBindings::Utils::ExtractTargetAddress(disassemblyInstance* dis, cs_insn& instruction)
{
	if (instruction.detail->arm64.op_count == 0)
	{
		Debug::Msg("Not enough operands to extract target address");
		return NULL;
	}

	auto detail = instruction.detail->arm64;
	auto lastOp = detail.operands[detail.op_count - 1];

	if (lastOp.type != ARM64_OP_IMM)
	{
		Debug::Msg("Non IMM OP");
		return NULL;
	}

	auto dest =  (void*)lastOp.imm;
	return dest;
}

void XrefScannerBindings::CheckEntry(void* codeStart)
{
	if (disMap.find(codeStart) == disMap.end())
	{
		disMap[codeStart] = (disassemblyInstance*)malloc(sizeof(disassemblyInstance));
		memset(disMap[codeStart], 0, sizeof(disassemblyInstance));
		disMap[codeStart]->codeStart = codeStart;
	}
	else {
		Debug::Msg("entry exists");
	}
}

bool XrefScannerBindings::CheckCapstone(void* codeStart)
{
	CheckEntry(codeStart);

	if (!disMap[codeStart]->use_cs) {
		disMap[codeStart]->cs_len = cs_disasm(cs, (uint8_t*)codeStart, 1000, (uint64_t)codeStart, 0, &(disMap[codeStart]->cs_ins));

		if (disMap[codeStart]->cs_len == 0) {
			free(disMap[codeStart]);
			Logger::Error("Failed to setup capstone");
			return false;
		}

		disMap[codeStart]->use_cs = true;
	}

	return true;
}

void XrefScannerBindings::CleanupCapstoneInstance(void* codeStart)
{
	cs_free(disMap[codeStart]->cs_ins, disMap[codeStart]->cs_len);
	disMap[codeStart]->use_cs = false;
}

void XrefScannerBindings::Cleanup(void* codeStart)
{
	if (disMap.find(codeStart) == disMap.end())
	{
		Debug::Msg("Not Found");
		return;
	}

	if (disMap[codeStart]->use_cs)
		CleanupCapstoneInstance(codeStart);

	free(disMap[codeStart]);
	disMap.erase(codeStart);
}

void XrefScannerBindings::PartialCleanup(void* codeStart)
{
	if (disMap.find(codeStart) == disMap.end())
	{
		Debug::Msg("Not Found");
		return;
	}

	if (disMap[codeStart]->use_cs)
		CleanupCapstoneInstance(codeStart);
}

void XrefScannerBindings::XrefScanner::XrefScanImplNative(void* codeStart, bool skipClassCheck, XrefScanImplNativeRes& res)
{
	if (!CheckCapstone(codeStart))
	{
		res.complete = true;
		Cleanup(codeStart);
		return;
	}

	res.target = codeStart;
	res.codeStart = codeStart;

	disassemblyInstance* dis = disMap[codeStart];
	
	if (!dis->exit)
		for (; dis->counter[1] < 1000; dis->counter[1]++) {
			cs_insn instruction = dis->cs_ins[dis->counter[1]];
			//funchook_disasm_log_instruction(&instruction);

			if (dis->exit)
				break;

			if (HAS_GROUP(ARM64_GRP_RET))
				break;

			if (HAS_GROUP(ARM64_GRP_INT))
				break;

			if (HAS_GROUP(ARM64_GRP_JUMP) || HAS_GROUP(ARM64_GRP_CALL))
			{
				//Debug::Msg("Jump");
				dis->counter[1]++;
				//return NULL;
				res.type = XrefType::Method;
				res.target = Utils::ExtractTargetAddress(dis, instruction);
				PartialCleanup(codeStart);
				return;
			}

			if (instruction.id == ARM64_INS_MOV) {
				funchook_disasm_log_instruction(&instruction);
			}
		}


	res.complete = true;
	Cleanup(codeStart);
}

void* XrefScannerBindings::XrefScannerLowLevel::JumpTargetsImpl(void* codeStart)
{
	if (!CheckCapstone(codeStart))
	{
		Cleanup(codeStart);
		return NULL;
	}

	disassemblyInstance* dis = disMap[codeStart];

	for (; dis->counter[0] < 1024*1024; dis->counter[0]++) {
		cs_insn instruction = dis->cs_ins[dis->counter[0]];
		//funchook_disasm_log_instruction(&instruction);

		if (dis->exit)
			break;

		if (HAS_GROUP(ARM64_GRP_RET))
			break;

		if (HAS_GROUP(ARM64_GRP_JUMP) || HAS_GROUP(ARM64_GRP_CALL))
		{
			if (HAS_GROUP(ARM64_GRP_JUMP))
				dis->exit = true;

			//Debug::Msg("Jump");
			dis->counter[0]++;
			//return NULL;
			auto res = Utils::ExtractTargetAddress(dis, instruction);
			PartialCleanup(codeStart);
			return res;
		}
	}
	
	Cleanup(codeStart);
	return NULL;
}

void* XrefScannerBindings::XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo(void* codeStart, void* callTarget)
{
	//if (!CheckCapstone(codeStart))
	//{
	//	Cleanup(codeStart);
	//	return NULL;
	//}

	//Cleanup(codeStart);
	return NULL;
}

void* XrefScannerBindings::XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo(void* codeStart, void* callTarget)
{
	//if (!CheckCapstone(codeStart))
	//{
	//	Cleanup(codeStart);
	//	return NULL;
	//}

	//Cleanup(codeStart);
	return NULL;
}

#undef HAS_GROUP
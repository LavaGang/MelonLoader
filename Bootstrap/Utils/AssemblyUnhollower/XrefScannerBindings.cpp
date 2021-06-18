#include "XrefScannerBindings.h"
#include "../Console/Logger.h"
#include "../Console/Debug.h"
#include "../../Managers/Il2Cpp.h"
#include <android/log.h>
#include <string>
#include <math.h>
#include <funchook.h>

#pragma region Constants
// 4kb
#define MIN_PAGE_SIZE 0x1000
// 4mb
#define EMU_MEM_SIZE MIN_PAGE_SIZE
#define EMU_DEFAULT_ADDRESS 0x1000000

#define HAS_GROUP(x) Utils::HasGroup(instruction.detail->groups, instruction.detail->groups_count, x)
#define HEX(x) ((x) < 10 ? (x) + '0' : (x) - 10 + 'A')
#define PRIxPTR ""
#define ADDR_FMT "%08" PRIxPTR

#define DECODER_DEFAULT_LIMIT 1000

#pragma endregion

csh XrefScannerBindings::cs = NULL;
std::unordered_map<uint64_t, XrefScannerBindings::DisassemblyInstance*> XrefScannerBindings::disassemble;
std::unordered_map<uint64_t, size_t*> XrefScannerBindings::transactions;

#pragma region FunchookHelper
void funchook_log(const char* fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	Logger::vMsgf(Console::Color::DarkCyan, fmt, ap);
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
#pragma endregion

bool XrefScannerBindings::Init()
{
	if (cs_open(CS_ARCH_ARM64, CS_MODE_LITTLE_ENDIAN, &cs) != CS_ERR_OK)
	{
		Logger::Error("Failed to load capstone");
		return false;
	}

	cs_option(cs, CS_OPT_DETAIL, CS_OPT_ON);

	return true;
}

#pragma region Utils
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

void* XrefScannerBindings::Utils::ExtractTargetAddress(DisassemblyInstance* dis, cs_insn& instruction)
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
#pragma endregion

#pragma region Lifecycle
XrefScannerBindings::DisassemblyInstance* XrefScannerBindings::CheckEntry(const DecoderSettings* decoder)
{
	if (disassemble.find(decoder->transaction) == disassemble.end())
	{
		disassemble[decoder->transaction] = (DisassemblyInstance*)malloc(sizeof(DisassemblyInstance));
		memset(disassemble[decoder->transaction], 0, sizeof(DisassemblyInstance));
		disassemble[decoder->transaction]->codeStart = decoder->codeStart;
	}

	if (transactions.find(decoder->transaction) == transactions.end()) {
		// Technically this is error prone, if this gets called more than once per itteration
		disassemble[decoder->transaction]->dependants++;
		transactions[decoder->transaction] = (size_t*)malloc(sizeof(size_t));
		*(transactions[decoder->transaction]) = 0;
	}

	return disassemble[decoder->transaction];
}

bool XrefScannerBindings::CheckCapstone(const DecoderSettings* decoder)
{
	auto dis = CheckEntry(decoder);
	if (dis == NULL)
		return false;

	if (dis->use_cs)
		return true;

	dis->cs_len = decoder->limit;
	dis->cs_ins = cs_malloc(cs);
	dis->cs_addr = (uint64_t)decoder->codeStart;

	//if (dis->cs_len >= decoder->limit)
	//	return true;

//if (dis->cs_len != 0)
//	cs_free(dis->cs_ins, dis->cs_len);

	//if (
	//	(dis->cs_len = cs_disasm(
	//		cs,
	//		(const uint8_t*)(dis->codeStart),
	//		decoder->limit, // FIXME: This can cause a seg fault
	//		(uint64_t)dis->codeStart,
	//		0,
	//		(cs_insn**)(&(dis->cs_ins))
	//	)) == 0
	//) {
	//	Logger::Errorf("Failed to setup capstone %d", dis->cs_len);
	//	Cleanup(decoder);
	//	return false;
	//}

	dis->use_cs = true;

	return true;
}

void XrefScannerBindings::CleanupCapstoneInstance(const DecoderSettings* decoder)
{
	auto dis = GetDisassembly(decoder);
	if (dis == NULL || !dis->use_cs)
		return;

	cs_free(dis->cs_ins, 1);
	dis->use_cs = false;
}

void XrefScannerBindings::Cleanup(const DecoderSettings* decoder)
{
	auto dis = GetDisassembly(decoder);
	if (dis == NULL)
		return;

	CleanupCapstoneInstance(decoder);
	free(dis);

	auto counter = GetCounter(decoder);
	if (counter != NULL)
		free(counter);

	transactions.erase(decoder->transaction);
	disassemble.erase(decoder->transaction);
}

void XrefScannerBindings::PartialCleanup(const DecoderSettings* decoder)
{
	if (disassemble.find(decoder->transaction) == disassemble.end())
	{
		Debug::Msg("Not Found");
		return;
	}

	CleanupCapstoneInstance(decoder);
}

XrefScannerBindings::DisassemblyInstance* XrefScannerBindings::GetDisassembly(const DecoderSettings* decoder)
{
	if (disassemble.find(decoder->transaction) != disassemble.end())
	{
		return disassemble[decoder->transaction];
	}
	Debug::Msg("Not found");
	return NULL;
}

size_t* XrefScannerBindings::GetCounter(const DecoderSettings* decoder)
{
	if (transactions.find(decoder->transaction) != transactions.end())
		return transactions[decoder->transaction];

	return NULL;
}

#pragma endregion

#pragma region Native Implementations
void XrefScannerBindings::XrefScanner::XrefScanImplNative(const DecoderSettings* decoder, bool skipClassCheck, XrefScanImplNativeRes& res)
{
	if (!CheckCapstone(decoder))
	{
		res.complete = true;
		Cleanup(decoder);
		return;
	}

	DisassemblyInstance* dis = GetDisassembly(decoder);
	size_t* counter = GetCounter(decoder);

	res.codeStart = dis->codeStart;
	uint8_t* startingIns = ((uint8_t*)dis->cs_addr);

	if (!dis->exit)
		//for (; *counter < dis->cs_len && *counter < decoder->limit; (*counter)++) {
		while(cs_disasm_iter(cs, (const uint8_t**)(&startingIns), &dis->cs_len, &dis->cs_addr, dis->cs_ins)) {
			cs_insn instruction = dis->cs_ins[0];
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
				(*counter)++;
				//return NULL;
				res.type = XrefType::Method;
				res.target = Utils::ExtractTargetAddress(dis, instruction);

				//PartialCleanup(decoder);
				return;
			}

			if (instruction.id == ARM64_INS_MOV) {
				//funchook_disasm_log_instruction(&instruction);
				continue;
			}
		}


	res.complete = true;
	Cleanup(decoder);
}

void* XrefScannerBindings::XrefScannerLowLevel::JumpTargetsImpl(const DecoderSettings* decoder)
{
	if (!CheckCapstone(decoder))
	{
		Cleanup(decoder);
		return NULL;
	}

	DisassemblyInstance* dis = GetDisassembly(decoder);
	size_t* counter = GetCounter(decoder);

	//Debug::Msgf("%d %p %p", dis->cs_len, dis->cs_addr, decoder->codeStart);
	uint8_t* startingIns = ((uint8_t*)dis->cs_addr);

	//for (; (*counter) < decoder->limit; (*counter)++) {
	while (cs_disasm_iter(cs, (const uint8_t**)(&startingIns), &dis->cs_len, &dis->cs_addr, dis->cs_ins)) {
		cs_insn instruction = dis->cs_ins[0];
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
			*counter = *counter + 1;
			//return NULL;
			auto res = Utils::ExtractTargetAddress(dis, instruction);

			PartialCleanup(decoder);
			return res;
		}
	}
	
	Cleanup(decoder);
	return NULL;
}

void* XrefScannerBindings::XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo(const DecoderSettings* decoder, void* callTarget)
{
	if (!CheckCapstone(decoder))
	{
		Cleanup(decoder);
		return NULL;
	}

	void* lastReadReg_x = NULL;

	DisassemblyInstance* dis = GetDisassembly(decoder);
	size_t* counter = GetCounter(decoder);

	//for (; (*counter) < decoder->limit; (*counter)++) {
	while (cs_disasm_iter(cs, (const uint8_t**)(&dis->cs_ins), &dis->cs_len, &dis->cs_addr, dis->cs_ins)) {
		cs_insn instruction = dis->cs_ins[0];

		if (dis->exit)
			break;

		if (HAS_GROUP(ARM64_GRP_RET))
			return NULL;

		if (HAS_GROUP(ARM64_GRP_JUMP))
			continue;

		if (HAS_GROUP(ARM64_GRP_INT))
			return NULL;

		if (HAS_GROUP(ARM64_GRP_CALL))
		{
			*counter = *counter + 1;
			auto target = Utils::ExtractTargetAddress(dis, instruction);
			if (target == callTarget)
			{
				Cleanup(decoder);
				return lastReadReg_x;
			}
		}

		if (instruction.id == ARM64_INS_MOV)
		{
			Debug::Msg("FindLastRcxReadAddressBeforeCallTo");
			funchook_disasm_log_instruction(&instruction);
		}
	}

	Cleanup(decoder);
	return NULL;
}

void* XrefScannerBindings::XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo(const DecoderSettings* decoder, void* callTarget)
{
	if (!CheckCapstone(decoder))
	{
		Cleanup(decoder);
		return NULL;
	}

	void* lastReadReg_x = NULL;
	bool seenCall = false;

	DisassemblyInstance* dis = GetDisassembly(decoder);
	size_t* counter = GetCounter(decoder);

	//for (; (*counter) < decoder->limit; (*counter)++) {
	while (cs_disasm_iter(cs, (const uint8_t**)(&dis->cs_ins), &dis->cs_len, &dis->cs_addr, dis->cs_ins)) {
		cs_insn instruction = dis->cs_ins[0];

		if (dis->exit)
			break;

		if (HAS_GROUP(ARM64_GRP_RET))
			return NULL;

		if (HAS_GROUP(ARM64_GRP_JUMP))
			continue;

		if (HAS_GROUP(ARM64_GRP_INT))
			return NULL;

		if (HAS_GROUP(ARM64_GRP_CALL))
		{
			*counter = *counter + 1;
			auto target = Utils::ExtractTargetAddress(dis, instruction);
			if (target == callTarget)
			{
				seenCall = true;
				continue;
			}
		}

		if (instruction.id == ARM64_INS_MOV && seenCall)
		{
			Debug::Msg("FindByteWriteTargetRightAfterCallTo");
			funchook_disasm_log_instruction(&instruction);
		}
	}

	Cleanup(decoder);
	return NULL;
}
#pragma endregion

#undef HAS_GROUP
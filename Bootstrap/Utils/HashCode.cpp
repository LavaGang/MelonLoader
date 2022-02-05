#ifndef PORT_DISABLE
#include "HashCode.h"
#include "Debug.h"
#include "../Managers/Game.h"
#include "../Base/Core.h"
#include "AssemblyGenerator.h"
#include "../Managers/BaseAssembly.h"
#include <string>
#include <wincrypt.h>
#include "Assertion.h"
#include "Logger.h"

DWORD HashCode::Hash = NULL;
char* HashCode::Path_SM_Il2Cpp = NULL;
char* HashCode::Path_SM_Mono = NULL;
char* HashCode::Path_SM_Mono_Pre2017 = NULL;
char* HashCode::Path_SM_Mono_Pre5 = NULL;

bool HashCode::Initialize()
{
	if (!SetupPaths())
		return false;
	AddHash(Core::Path);
	AddHash(BaseAssembly::Path);
	AddHash(AssemblyGenerator::Path);
	AddHash(Path_SM_Il2Cpp);
	AddHash(Path_SM_Mono);
	AddHash(Path_SM_Mono_Pre2017);
	AddHash(Path_SM_Mono_Pre5);
	return true;
}

bool HashCode::SetupPaths()
{
	std::string CorePath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\Bootstrap.dll";
	Core::Path = new char[CorePath.size() + 1];
	std::copy(CorePath.begin(), CorePath.end(), Core::Path);
	Core::Path[CorePath.size()] = '\0';

	std::string BaseAssemblyPath = std::string(Game::BasePath) + "\\MelonLoader\\MelonLoader.dll";
	if (!Core::FileExists(BaseAssemblyPath.c_str()))
	{
		Assertion::ThrowInternalFailure("MelonLoader.dll Does Not Exist!");
		return false;
	}
	BaseAssembly::Path = new char[BaseAssemblyPath.size() + 1];
	std::copy(BaseAssemblyPath.begin(), BaseAssemblyPath.end(), BaseAssembly::Path);
	BaseAssembly::Path[BaseAssemblyPath.size()] = '\0';

	std::string AssemblyGeneratorPath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\AssemblyGenerator\\AssemblyGenerator.dll";
	if (Game::IsIl2Cpp && !Core::FileExists(AssemblyGeneratorPath.c_str()))
	{
		Assertion::ThrowInternalFailure("AssemblyGenerator.dll Does Not Exist!");
		return false;
	}
	AssemblyGenerator::Path = new char[AssemblyGeneratorPath.size() + 1];
	std::copy(AssemblyGeneratorPath.begin(), AssemblyGeneratorPath.end(), AssemblyGenerator::Path);
	AssemblyGenerator::Path[AssemblyGeneratorPath.size()] = '\0';

	std::string SM_Il2CppPath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Il2Cpp.dll";
	Path_SM_Il2Cpp = new char[SM_Il2CppPath.size() + 1];
	std::copy(SM_Il2CppPath.begin(), SM_Il2CppPath.end(), Path_SM_Il2Cpp);
	Path_SM_Il2Cpp[SM_Il2CppPath.size()] = '\0';

	std::string SM_MonoPath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Mono.dll";
	Path_SM_Mono = new char[SM_MonoPath.size() + 1];
	std::copy(SM_MonoPath.begin(), SM_MonoPath.end(), Path_SM_Mono);
	Path_SM_Mono[SM_MonoPath.size()] = '\0';

	std::string SM_Mono_Pre2017Path = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Mono.Pre-2017.dll";
	Path_SM_Mono_Pre2017 = new char[SM_Mono_Pre2017Path.size() + 1];
	std::copy(SM_Mono_Pre2017Path.begin(), SM_Mono_Pre2017Path.end(), Path_SM_Mono_Pre2017);
	Path_SM_Mono_Pre2017[SM_Mono_Pre2017Path.size()] = '\0';

	std::string SM_Mono_Pre5Path = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Mono.Pre-5.dll";
	Path_SM_Mono_Pre5 = new char[SM_Mono_Pre5Path.size() + 1];
	std::copy(SM_Mono_Pre5Path.begin(), SM_Mono_Pre5Path.end(), Path_SM_Mono_Pre5);
	Path_SM_Mono_Pre5[SM_Mono_Pre5Path.size()] = '\0';

	return true;
}

void HashCode::AddHash(const char* path)
{
	if ((path == NULL) || (!Core::FileExists(path)))
		return;
	HANDLE filehandle = CreateFileA(path, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	if (filehandle == INVALID_HANDLE_VALUE)
		return;
	HCRYPTPROV cryptprov = 0;
	if (!CryptAcquireContextA(&cryptprov, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
	{
		CloseHandle(filehandle);
		return;
	}
	HCRYPTHASH crypthash = 0;
	if (!CryptCreateHash(cryptprov, CALG_MD5, 0, 0, &crypthash))
	{
		CloseHandle(filehandle);
		CryptReleaseContext(cryptprov, 0);
		return;
	}
	DWORD filebufsize = 2048;
	BYTE filebuf[2048];
	DWORD readoffset = NULL;
	bool readsuccess = false;
	while (readsuccess = ReadFile(filehandle, filebuf, filebufsize, &readoffset, NULL))
	{
		if (readoffset == NULL)
			break;
		if (!CryptHashData(crypthash, filebuf, readoffset, 0))
		{
			readsuccess = false;
			break;
		}
	}
	if (!readsuccess)
	{
		CryptReleaseContext(cryptprov, 0);
		CryptDestroyHash(crypthash);
		CloseHandle(filehandle);
		return;
	}
	DWORD dhash = 16;
	BYTE hashbuf[16];
	CHAR chartbl[] = "0123456789abcdef";
	std::string hashout;
	if (!CryptGetHashParam(crypthash, HP_HASHVAL, hashbuf, &dhash, 0))
	{
		CryptReleaseContext(cryptprov, 0);
		CryptDestroyHash(crypthash);
		CloseHandle(filehandle);
		return;
	}
	for (DWORD i = 0; i < dhash; i++)
		hashout += std::to_string(chartbl[hashbuf[i] >> 4]) + std::to_string(chartbl[hashbuf[i] & 0xf]);
	for (int i = 0; i < hashout.size(); i++)
		Hash += strtoul(std::to_string(hashout[i]).c_str(), NULL, 0);
	CryptDestroyHash(crypthash);
	CryptReleaseContext(cryptprov, 0);
	CloseHandle(filehandle);
}
#endif
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

bool HashCode::Initialize()
{
    return (SetupPaths()
        && AddHash(BaseAssembly::Path)
        && AddHash(Core::Path)
        && (!Game::IsIl2Cpp || AddHash(AssemblyGenerator::Path)));
}

bool HashCode::SetupPaths()
{
    std::string BaseAssemblyPath = std::string(Game::BasePath) + "\\MelonLoader\\MelonLoader.dll";
    if (!Core::FileExists(BaseAssemblyPath.c_str()))
    {
        Assertion::ThrowInternalFailure("MelonLoader.dll Does Not Exist!");
        return false;
    }
    BaseAssembly::Path = new char[BaseAssemblyPath.size() + 1];
    std::copy(BaseAssemblyPath.begin(), BaseAssemblyPath.end(), BaseAssembly::Path);
    BaseAssembly::Path[BaseAssemblyPath.size()] = '\0';
    std::string CorePath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\Bootstrap.dll";
	Core::Path = new char[CorePath.size() + 1];
	std::copy(CorePath.begin(), CorePath.end(), Core::Path);
	Core::Path[CorePath.size()] = '\0';
    if (Game::IsIl2Cpp)
    {
        std::string AssemblyGeneratorPath = std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\AssemblyGenerator\\AssemblyGenerator.dll";
        if (!Core::FileExists(AssemblyGeneratorPath.c_str()))
        {
            Assertion::ThrowInternalFailure("AssemblyGenerator.dll Does Not Exist!");
            return false;
        }
        AssemblyGenerator::Path = new char[AssemblyGeneratorPath.size() + 1];
        std::copy(AssemblyGeneratorPath.begin(), AssemblyGeneratorPath.end(), AssemblyGenerator::Path);
        AssemblyGenerator::Path[AssemblyGeneratorPath.size()] = '\0';
    }
	return true;
}

bool HashCode::AddHash(const char* path)
{
    HANDLE filehandle = CreateFileA(path, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
    if (filehandle == INVALID_HANDLE_VALUE)
    {
        // Throw Internal Error Here
        return false;
    }
    HCRYPTPROV cryptprov = 0;
    if (!CryptAcquireContextA(&cryptprov, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
    {
        // Throw Internal Error Here
        CloseHandle(filehandle);
        return false;
    }
    HCRYPTHASH crypthash = 0;
    if (!CryptCreateHash(cryptprov, CALG_MD5, 0, 0, &crypthash))
    {
        // Throw Internal Error Here
        CloseHandle(filehandle);
        CryptReleaseContext(cryptprov, 0);
        return false;
    }
    BYTE filebuf[1024];
    DWORD readoffset = NULL;
    bool readsuccess = false;
    while (readsuccess = ReadFile(filehandle, filebuf, 1024, &readoffset, NULL))
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
        // Throw Internal Error Here
        CryptReleaseContext(cryptprov, 0);
        CryptDestroyHash(crypthash);
        CloseHandle(filehandle);
        return false;
    }
    DWORD dhash = 16;
    BYTE hashbuf[16];
    CHAR chartbl[] = "0123456789abcdef";
    std::string hashout;
    if (!CryptGetHashParam(crypthash, HP_HASHVAL, hashbuf, &dhash, 0))
    {
        // Throw Internal Error Here
        CryptReleaseContext(cryptprov, 0);
        CryptDestroyHash(crypthash);
        CloseHandle(filehandle);
        return false;
    }
    for (DWORD i = 0; i < dhash; i++)
        hashout += std::to_string(chartbl[hashbuf[i] >> 4]) + std::to_string(chartbl[hashbuf[i] & 0xf]);
    Hash += strtoul(hashout.c_str(), NULL, 0);
    CryptDestroyHash(crypthash);
    CryptReleaseContext(cryptprov, 0);
    CloseHandle(filehandle);
    return true;
}
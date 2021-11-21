#include "HashCode.h"
#include "Debug.h"
#include "../Managers/Game.h"
#include "../Core.h"
#include "Il2CppAssemblyGenerator.h"
#include "../Managers/BaseAssembly.h"
#include "../Utils/Encoding.h"
#include <wincrypt.h>
#include "Assertion.h"
#include "Logging/Logger.h"
#include <sstream>

std::string HashCode::Hash;

bool HashCode::Initialize()
{
    if (!SetupPaths())
        return false;
    return GenerateHash(Core::Path);
}

bool HashCode::SetupPaths()
{
    std::string CorePath = std::string(Core::BasePath) + "\\MelonLoader\\Dependencies\\Bootstrap.dll";
    Core::Path = new char[CorePath.size() + 1];
    std::copy(CorePath.begin(), CorePath.end(), Core::Path);
    Core::Path[CorePath.size()] = '\0';

    std::string BaseAssemblyPath = std::string(Core::BasePath) + "\\MelonLoader\\MelonLoader.dll";
    if (!Core::FileExists(BaseAssemblyPath.c_str()))
    {
        Assertion::ThrowInternalFailure("MelonLoader.dll Does Not Exist!");
        return false;
    }
    BaseAssembly::PathMono = new char[BaseAssemblyPath.size() + 1];
    std::copy(BaseAssemblyPath.begin(), BaseAssemblyPath.end(), BaseAssembly::PathMono);
    BaseAssembly::PathMono[BaseAssemblyPath.size()] = '\0';

    if (!Game::IsIl2Cpp)
        return true;
    std::string AssemblyGeneratorPath = std::string(Core::BasePath) + "\\MelonLoader\\Dependencies\\Il2CppAssemblyGenerator\\Il2CppAssemblyGenerator.dll";
    if (!Core::FileExists(AssemblyGeneratorPath.c_str()))
    {
        Assertion::ThrowInternalFailure("AssemblyGenerator.dll Does Not Exist!");
        return false;
    }
    Il2CppAssemblyGenerator::PathMono = new char[AssemblyGeneratorPath.size() + 1];
    std::copy(AssemblyGeneratorPath.begin(), AssemblyGeneratorPath.end(), Il2CppAssemblyGenerator::PathMono);
    Il2CppAssemblyGenerator::PathMono[AssemblyGeneratorPath.size()] = '\0';

#define TO_UTF8(s) ((s) = Encoding::OsToUtf8((s)))
    
    TO_UTF8(Il2CppAssemblyGenerator::PathMono);
    TO_UTF8(BaseAssembly::PathMono);

#undef TO_UTF8

    return true;
}

bool HashCode::GenerateHash(const char* path)
{
    if ((path == NULL) || (!Core::FileExists(path)))
        return false;
    HCRYPTPROV cryptprov = 0;
    if (!CryptAcquireContextA(&cryptprov, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
        return false;
    HCRYPTHASH crypthash = 0;
    if (!CryptCreateHash(cryptprov, CALG_MD5, 0, 0, &crypthash))
    {
        CryptReleaseContext(cryptprov, 0);
        return false;
    }
    HANDLE filehandle = CreateFileA(path, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
    if (filehandle == INVALID_HANDLE_VALUE)
    {
        CryptReleaseContext(cryptprov, 0);
        return false;
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
    CloseHandle(filehandle);
    if (!readsuccess)
    {
        CryptReleaseContext(cryptprov, 0);
        CryptDestroyHash(crypthash);
        return false;
    }
    DWORD dhash = 16;
    BYTE hashbuf[16];
    CHAR chartbl[] = "0123456789abcdef";
    std::string hashout;
    if (!CryptGetHashParam(crypthash, HP_HASHVAL, (BYTE*)&hashbuf, &dhash, 0))
    {
        CryptReleaseContext(cryptprov, 0);
        CryptDestroyHash(crypthash);
        return false;
    }
    for (DWORD i = 0; i < dhash; i++)
        hashout += std::to_string(chartbl[hashbuf[i] >> 4]) + std::to_string(chartbl[hashbuf[i] & 0xf]);
    Hash = hashout;
    CryptDestroyHash(crypthash);
    CryptReleaseContext(cryptprov, 0);
    return true;
}
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
    return SetupPaths();
}

bool HashCode::SetupPaths()
{
    std::string CorePath = std::string(Core::BasePath) + "\\MelonLoader\\Dependencies\\Bootstrap.dll";
    Core::Path = new char[CorePath.size() + 1];
    std::copy(CorePath.begin(), CorePath.end(), Core::Path);
    Core::Path[CorePath.size()] = '\0';

    std::string BaseAssemblyPath;

    if (!Game::IsIl2Cpp)
    {
        BaseAssemblyPath = std::string(Core::BasePath) + "\\MelonLoader\\net35\\MelonLoader.dll";
    }
    else
    {
        BaseAssemblyPath = std::string(Core::BasePath) + "\\MelonLoader\\net6\\MelonLoader.dll";
    }
    
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
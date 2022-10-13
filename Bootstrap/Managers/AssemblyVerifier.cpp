#include "AssemblyVerifier.h"
#include "Hook.h"
#include <unordered_map>
#include <math.h>
#include <algorithm>
#include "../Utils/Debug.h"
#include "Game.h"
#pragma warning(disable:4244)

AssemblyVerifier::callOriginalLoadFrom_t AssemblyVerifier::callOriginalLoadFrom;
AssemblyVerifier::callOriginalLoadRaw_t AssemblyVerifier::callOriginalLoadRaw;

__forceinline bool IsNameValid(const char* name)
{
    if (name == NULL)
        return false;

    while (const char c = *name)
    {
        if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '<' || c == '>'
            || c == '`' || c == '.' || c == '=' || c == '-' || c == '|' || c == ',' || c == '[' || c == ']' || c == '$'
            || c == ':' || c == '@' || c == ')' || c == '(' || c == '?' || c == '{' || c == '}' || c == '!' || c == '/'))
            return false;
        name++;
    }

    return true;
}

__forceinline void CountChars(const char* str, std::unordered_map<char, int>& map)
{
    while (const auto c = *str)
    {
        map[c]++;

        str++;
    }
}

__forceinline bool CheckAssembly(Mono::Image* image)
{
    const char* imagename = Mono::Exports::mono_image_get_name(image);

    auto moduleCount = Mono::Exports::mono_image_get_table_rows(image, Mono::MONO_TABLE_MODULE);
    if (moduleCount != 1)
        return false;

    auto numTypeDefs = Mono::Exports::mono_image_get_table_rows(image, Mono::MONO_TABLE_TYPEDEF);
    auto numTypeRefs = Mono::Exports::mono_image_get_table_rows(image, Mono::MONO_TABLE_TYPEREF);
    auto numMethodDefs = Mono::Exports::mono_image_get_table_rows(image, Mono::MONO_TABLE_METHOD);
    auto numFieldDefs = Mono::Exports::mono_image_get_table_rows(image, Mono::MONO_TABLE_FIELD);

    int delegateIndex = -2;

    for (int i = 0; i < numTypeRefs; i++)
    {
        auto nsIndex = Mono::Exports::mono_metadata_decode_table_row_col(
            image, Mono::MONO_TABLE_TYPEREF, i, Mono::MONO_TYPEREF_NAMESPACE);
        auto nameIndex = Mono::Exports::mono_metadata_decode_table_row_col(
            image, Mono::MONO_TABLE_TYPEREF, i, Mono::MONO_TYPEREF_NAME);

        auto nsStr = Mono::Exports::mono_metadata_string_heap(image, nsIndex);
        auto nameStr = Mono::Exports::mono_metadata_string_heap(image, nameIndex);

        if (strcmp(nameStr, "MulticastDelegate") == 0 && strcmp(nsStr, "System") == 0)
        {
            delegateIndex = i;
            break;
        }
    }

    auto symbolCounts = std::unordered_map<char, int>();

    for (int i = 0; i < numTypeDefs; i++)
    {
        auto typeNsIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_NAMESPACE);
        auto typeNameIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_NAME);

        auto typeNsStr = Mono::Exports::mono_metadata_string_heap(image, typeNsIndex);
        auto typeNameStr = Mono::Exports::mono_metadata_string_heap(image, typeNameIndex);

        const auto baseTypeRef = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_EXTENDS);
        const auto baseType = ((baseTypeRef & 3) == 1) ? (baseTypeRef >> 2) - 1 : -1;
        if (baseType == delegateIndex)
        {
            const auto fieldIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_FIELD_LIST);
            const auto nextFieldIndex = i == numTypeDefs - 1 ? (numFieldDefs + 1) : Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i + 1, Mono::MONO_TYPEDEF_FIELD_LIST);
            if (fieldIndex != nextFieldIndex)
                return false;
        }

        if (!IsNameValid(typeNsStr) || !IsNameValid(typeNameStr) || strcmp(typeNameStr, "EvaluationAttribute") == 0)
            return false;

        if (strstr(typeNameStr, "<Module>") != NULL)
        {
            const auto fieldIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_FIELD_LIST);
            const auto nextFieldIndex = (i == numTypeDefs - 1) ? (numFieldDefs + 1) : Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i + 1, Mono::MONO_TYPEDEF_FIELD_LIST);
            if (fieldIndex != nextFieldIndex)
                return false;

            const auto methodIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i, Mono::MONO_TYPEDEF_METHOD_LIST);
            const auto nextMethodIndex = (i == numTypeDefs - 1) ? (numMethodDefs + 1) : Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_TYPEDEF, i + 1, Mono::MONO_TYPEDEF_METHOD_LIST);
            if (methodIndex != nextMethodIndex)
                return false;
        }

        CountChars(typeNameStr, symbolCounts);
    }

    for (int i = 0; i < numMethodDefs; i++)
    {
        auto nameIndex = Mono::Exports::mono_metadata_decode_table_row_col(image, Mono::MONO_TABLE_METHOD, i, Mono::MONO_METHOD_NAME);
        auto nameStr = Mono::Exports::mono_metadata_string_heap(image, nameIndex);

        if (!IsNameValid(nameStr))
            return false;

        CountChars(nameStr, symbolCounts);
    }
    
    // exclude small assemblies from this check as they often get false positives
    if (numTypeDefs + numMethodDefs < 25)
        return true;

    double totalChars = 0;
    for (auto pair : symbolCounts)
        totalChars += pair.second;

    double totalEntropy = 0;

    for (auto pair : symbolCounts)
        totalEntropy += pair.second * log2(pair.second / totalChars);

    totalEntropy /= -totalChars;

    if (totalEntropy < 4 || totalEntropy > 5.25)
        return false;

    return true;
}

Mono::Object** AssemblyVerifier::LoadFromPatch(Mono::String** path, int refonly, void* stackMark, void* error)
{
    if (path != NULL)
    {
        Mono::MonoImageOpenStatus status;

        auto pathUtf = Mono::Exports::mono_string_to_utf8(*path);
        auto image = Mono::Exports::mono_image_open_full(pathUtf, &status, true);
        Mono::Free(pathUtf);
        if (image == NULL)
            Mono::Exports::mono_raise_exception(Mono::Exports::mono_get_exception_bad_image_format("Unable to load image"));
        
        auto checkResult = CheckAssembly(image);
        Mono::Exports::mono_image_close(image);
        if (!checkResult)
            Mono::Exports::mono_raise_exception(Mono::Exports::mono_get_exception_bad_image_format("Invalid assembly"));
    }
    return callOriginalLoadFrom(path, refonly, stackMark, error);
}

Mono::Object** AssemblyVerifier::LoadRawPatch(Mono::Object** appDomain, Mono::Object** bytes,
                                              Mono::Object** symbolStore, Mono::Object** evidence, int refonly,
                                              void* stackMark, void* error)
{
    if (bytes != NULL)
    {
        Mono::MonoImageOpenStatus status;

        auto length = Mono::Exports::mono_array_length(*bytes);
        auto rawPointer = Mono::Exports::mono_array_addr_with_size(*bytes, 0, 0);

        auto image = Mono::Exports::mono_image_open_from_data_full(rawPointer, length, false, &status, 1);
        if (image == NULL)
            Mono::Exports::mono_raise_exception(Mono::Exports::mono_get_exception_bad_image_format("Unable to load image"));
        auto checkResult = CheckAssembly(image);
        Mono::Exports::mono_image_close(image);
        if (!checkResult)
            Mono::Exports::mono_raise_exception(Mono::Exports::mono_get_exception_bad_image_format("Invalid assembly"));
    }

    return callOriginalLoadRaw(appDomain, bytes, symbolStore, evidence, refonly, stackMark, error);
}

void* LookupInternalCall(const char* ns, const char* typeName, const char* methodName, int paramCount)
{
    auto assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, "mscorlib");
    if (assembly == NULL)
    {
        Debug::Msg("Can't open mscorlib assembly");
        return NULL;
    }
    auto image = Mono::Exports::mono_assembly_get_image(assembly);
    auto klass = Mono::Exports::mono_class_from_name(image, ns, typeName);
    if (klass == NULL)
    {
        Debug::Msg("Can't find klass");
        return NULL;
    }
    auto method = Mono::Exports::mono_class_get_method_from_name(klass, methodName, paramCount);

    if (method == NULL)
    {
        Debug::Msg("Can't find method");
        return NULL;
    }
    return Mono::Exports::mono_lookup_internal_call(method);
}

void AssemblyVerifier::InstallHooks()
{
    callOriginalLoadFrom = static_cast<callOriginalLoadFrom_t>(LookupInternalCall(
        "System.Reflection", "Assembly", "LoadFrom", 2));
    if (callOriginalLoadFrom != NULL)
        Hook::Attach(reinterpret_cast<void**>(&callOriginalLoadFrom), &LoadFromPatch);
    else
    {
        Debug::Msg("Can't hook LoadFrom");
    }

    callOriginalLoadRaw = static_cast<callOriginalLoadRaw_t>(LookupInternalCall(
        "System", "AppDomain", "LoadAssemblyRaw", 4));
    if (callOriginalLoadRaw != NULL)
        Hook::Attach(reinterpret_cast<void**>(&callOriginalLoadRaw), &LoadRawPatch);
    else
    {
        Debug::Msg("Can't hook LoadRaw");
    }
}

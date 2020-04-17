#include <string>
#include "HookManager.h"
#include "ModHandler.h"
#include "MonoUnityPlayer.h"
#include "IL2CPPUnityPlayer.h"
#include "MelonLoader.h"
#include "Console.h"
#include "detours/detours.h"
#include "AssertionManager.h"
#include "Logger.h"

#pragma region Core
std::vector<HookManager_Hook*>HookManager::HookTbl;
HookManager_Hook* HookManager::FindHook(void** target, void* detour)
{
	HookManager_Hook* returnval = NULL;
	size_t HookTblSize = HookTbl.size();
	if (HookTblSize > 0)
	{
		for (size_t i = 0; i < HookTblSize; i++)
		{
			HookManager_Hook* hook = HookTbl[i];
			if ((hook != NULL) && (hook->Target == target) && (hook->Detour == detour))
			{
				returnval = hook;
				break;
			}
		}
	}
	return returnval;
}

void HookManager::Hook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		HookManager_Hook* hook = FindHook(target, detour);
		if (hook == NULL)
		{
			hook = new HookManager_Hook(target, detour);
			HookTbl.push_back(hook);
			INTERNAL_Hook(target, detour);
		}
	}
}

void HookManager::Unhook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		HookManager_Hook* hook = FindHook(target, detour);
		if (hook != NULL)
		{
			HookTbl.erase(std::find(HookManager::HookTbl.begin(), HookManager::HookTbl.end(), hook));
			delete hook;
			INTERNAL_Unhook(target, detour);
		}
	}
}

void HookManager::UnhookAll()
{
	size_t HookTblSize = HookTbl.size();
	if (HookTblSize < 0)
		return;
	for (size_t i = 0; i < HookTblSize; i++)
	{
		HookManager_Hook* hook = HookTbl[i];
		if (hook != NULL)
		{
			INTERNAL_Unhook(hook->Target, hook->Detour);
			delete hook;
		}
	}
	HookTbl.clear();
	LoadLibraryW_Unhook();
}

void HookManager::INTERNAL_Hook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(target, detour);
		DetourTransactionCommit();
	}
}

void HookManager::INTERNAL_Unhook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(target, detour);
		DetourTransactionCommit();
	}
}
#pragma endregion


#pragma region LoadLibraryW
LoadLibraryW_t HookManager::Original_LoadLibraryW = NULL;
void HookManager::LoadLibraryW_Hook()
{
	if (Original_LoadLibraryW == NULL)
	{
		Original_LoadLibraryW = LoadLibraryW;
		HookManager::INTERNAL_Hook(&(LPVOID&)Original_LoadLibraryW, HookManager::Hooked_LoadLibraryW);
	}
}
void HookManager::LoadLibraryW_Unhook()
{
	if (Original_LoadLibraryW != NULL)
	{
		HookManager::INTERNAL_Unhook(&(LPVOID&)Original_LoadLibraryW, HookManager::Hooked_LoadLibraryW);
		Original_LoadLibraryW = NULL;
	}
}
HMODULE __stdcall HookManager::Hooked_LoadLibraryW(LPCWSTR lpLibFileName)
{
	HMODULE lib = Original_LoadLibraryW(lpLibFileName);
	if (MelonLoader::IsGameIl2Cpp)
	{
		if (wcsstr(lpLibFileName, L"GameAssembly.dll"))
		{
			IL2CPPUnityPlayer::Module = AssertionManager::GetModuleHandlePtr("UnityPlayer");
			IL2CPP::Module = lib;
			if (IL2CPP::Setup() && IL2CPPUnityPlayer::Setup())
			{
				if (!MelonLoader::MupotMode)
				{
					Mono::CreateDomain();
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::PlayerCleanup, Hooked_PlayerCleanup);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::BaseBehaviourManager_Update, Hooked_BaseBehaviourManager_Update);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::BaseBehaviourManager_FixedUpdate, Hooked_BaseBehaviourManager_FixedUpdate);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::BaseBehaviourManager_LateUpdate, Hooked_BaseBehaviourManager_LateUpdate);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::GUIManager_DoGUIEvent, Hooked_GUIManager_DoGUIEvent);
					HookManager::Hook(&(LPVOID&)IL2CPPUnityPlayer::EndOfFrameCallbacks_DequeAll, Hooked_EndOfFrameCallbacks_DequeAll);
				}
				HookManager::Hook(&(LPVOID&)IL2CPP::il2cpp_init, Hooked_il2cpp_init);
				HookManager::Hook(&(LPVOID&)IL2CPP::il2cpp_add_internal_call, Hooked_il2cpp_add_internal_call);
				//HookManager::Hook(&(LPVOID&)IL2CPP::MetadataLoader_LoadMetadataFile, Hooked_MetadataLoader_LoadMetadataFile);
				//HookManager::Hook(&(LPVOID&)IL2CPP::il2cpp_class_from_system_type, Hooked_il2cpp_class_from_system_type);
			}
			LoadLibraryW_Unhook();
		}
	}
	else
	{
		ModHandler::Is35 = wcsstr(lpLibFileName, L"mono.dll");
		if (ModHandler::Is35 || wcsstr(lpLibFileName, L"mono-2.0-bdwgc.dll") || wcsstr(lpLibFileName, L"mono-2.0-sgen.dll") || wcsstr(lpLibFileName, L"mono-2.0-boehm.dll"))
		{
			MonoUnityPlayer::Module = AssertionManager::GetModuleHandlePtr("UnityPlayer");
			Mono::Module = lib;
			if (Mono::Setup() && MonoUnityPlayer::Setup())
			{
				LoadLibraryW_Unhook();

				HookManager::Hook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
				HookManager::Hook(&(LPVOID&)MonoUnityPlayer::PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
			}
			LoadLibraryW_Unhook();
		}
	}
	return lib;
}
#pragma endregion


#pragma region il2cpp_init
Il2CppDomain* HookManager::Hooked_il2cpp_init(const char* name)
{
	IL2CPP::Domain = IL2CPP::il2cpp_init(name);
	if (MelonLoader::MupotMode && MonoUnityPlayer::Load() && MonoUnityPlayer::Setup())
	{
		HookManager::Hook(&(LPVOID&)MonoUnityPlayer::PlayerLoadFirstScene, HookManager::Hooked_PlayerLoadFirstScene);
		HookManager::Hook(&(LPVOID&)MonoUnityPlayer::SingleAppInstance_FindOtherInstance, HookManager::Hooked_SingleAppInstance_FindOtherInstance);
		MonoUnityPlayer::UnityMain();
	}
	HookManager::Unhook(&(LPVOID&)IL2CPP::il2cpp_init, Hooked_il2cpp_init);
	return IL2CPP::Domain;
}
#pragma endregion


#pragma region mono_jit_init_version
MonoDomain* HookManager::Hooked_mono_jit_init_version(const char* name, const char* version)
{
	HookManager::Unhook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
	Mono::Domain = Mono::mono_jit_init_version(name, version);
	return Mono::Domain;
}
#pragma endregion


#pragma region il2cpp_add_internal_call
void HookManager::Hooked_il2cpp_add_internal_call(const char* name, void* method)
{
	if (!MelonLoader::MupotMode)
	{
		IL2CPP::il2cpp_add_internal_call(name, method);
		Mono::mono_add_internal_call(name, method);
		if (strstr(name, "UnityEngine.Application::get_companyName"))
			Mono::mono_add_internal_call("MelonLoader.Imports::GetCompanyName", method);
		else if (strstr(name, "UnityEngine.Application::get_productName"))
			Mono::mono_add_internal_call("MelonLoader.Imports::GetProductName", method);
		else if (strstr(name, "UnityEngine.Application::get_unityVersion"))
			Mono::mono_add_internal_call("MelonLoader.Imports::GetUnityVersion", method);
	}
}
#pragma endregion


#pragma region mono_lookup_internal_call_full
void* HookManager::Hooked_mono_lookup_internal_call_full(MonoMethod* method, int* uses_handles)
{
	void* returnmethod = Mono::mono_lookup_internal_call_full(method, uses_handles);
	if (returnmethod == NULL)
	{
		if ((IL2CPP::Domain != NULL) && (Mono::Domain != NULL))
		{
			MonoClass* mono_klass = Mono::mono_method_get_class(method);
			if (mono_klass != NULL)
			{
				MonoImage* mono_image = Mono::mono_class_get_image(mono_klass);
				if (mono_image != NULL)
				{
					const char* mono_image_name = Mono::mono_image_get_name(mono_image);
					if ((Mono::Domain != NULL) && (IL2CPP::Domain != NULL))
					{
						Il2CppAssembly* target_asm = IL2CPP::il2cpp_domain_assembly_open(IL2CPP::Domain, mono_image_name);
						if (target_asm != NULL)
						{
							Il2CppImage* target_image = IL2CPP::il2cpp_assembly_get_image(target_asm);
							if (target_image != NULL)
							{
								const char* reflection_name = Mono::mono_method_get_reflection_name(method);
								Il2CppClass* parent_klass = NULL;
								Il2CppClass* klass = IL2CPP::il2cpp_class_from_name(target_image, mono_klass->name_space, mono_klass->name);
								if (klass == NULL)
								{
									MonoClass* parent_mono_klass = Mono::mono_class_get_parent(mono_klass);
									if (parent_mono_klass != NULL)
									{
										parent_klass = IL2CPP::il2cpp_class_from_name(target_image, parent_mono_klass->name_space, parent_mono_klass->name);
										if (parent_klass == NULL)
										{
											void* nestedtype_iter = NULL;
											Il2CppClass* nestedtype = NULL;
											while ((klass == NULL) && ((nestedtype = IL2CPP::il2cpp_class_get_nested_types(parent_klass, &nestedtype_iter)) != NULL))
											{
												const char* nestedtype_name = IL2CPP::il2cpp_class_get_name(nestedtype);
												if (strstr(nestedtype_name, mono_klass->name))
												{
													klass = nestedtype;
													break;
												}
											}
										}
									}
								}
								if (klass != NULL)
								{
									unsigned int method_param_count = MelonLoader::CountSubstring(",", reflection_name);
									if (method_param_count >= 1)
										method_param_count += 1;
									const char* method_name = Mono::mono_method_get_name(method);
									Il2CppMethod* il2cpp_method = IL2CPP::il2cpp_class_get_method_from_name(klass, method_name, method_param_count);
									if (il2cpp_method != NULL)
									{
										std::string method_name_str = method_name;
										std::string completed_str = "";
										if (parent_klass != NULL)
											completed_str += std::string(parent_klass->name_space) + "." + std::string(parent_klass->name) + "." + std::string(klass->name);
										else
											completed_str += std::string(klass->name_space) + "." + std::string(klass->name);
										completed_str += "::" + std::string(method_name);

										Mono::mono_add_internal_call(completed_str.c_str(), il2cpp_method->targetMethod);
										returnmethod = Mono::mono_lookup_internal_call_full(method, uses_handles);
									}
								}
							}
						}
					}
				}
			}
		}
	}
	return returnmethod;
}
#pragma endregion


/*
#pragma region il2cpp_class_from_system_type
Il2CppClass* HookManager::Hooked_il2cpp_class_from_system_type(Il2CppReflectionType* type)
{
	if ((Mono::Domain != NULL) && (IL2CPP::Domain != NULL) && (IL2CPP::s_GlobalMetadataHeader != NULL) && (type != NULL) && (type->type != NULL))
	{
		int32_t index = type->type->data.klassIndex;
		if (index < 0)
			return NULL;
		if ((static_cast<uint32_t>(index) >= (IL2CPP::s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition))))
		{
			MonoType* mono_type = (MonoType*)type->type;
			if (mono_type != NULL)
			{
				MonoClass* mono_klass = mono_type->data.klass;
				if (mono_klass != NULL)
				{
					MonoImage* mono_image = Mono::mono_class_get_image(mono_klass);
					if (mono_image != NULL)
					{
						const char* mono_image_name = Mono::mono_image_get_name(mono_image);
						Il2CppAssembly* target_asm = IL2CPP::il2cpp_domain_assembly_open(IL2CPP::Domain, mono_image_name);
						if (target_asm != NULL)
						{
							Il2CppImage* target_image = IL2CPP::il2cpp_assembly_get_image(target_asm);
							if (target_image != NULL)
							{
								Il2CppClass* klass = IL2CPP::il2cpp_class_from_name(target_image, mono_klass->name_space, mono_klass->name);
								if (klass == NULL)
								{
									MonoClass* parent_mono_klass = Mono::mono_class_get_parent(mono_klass);
									if (parent_mono_klass != NULL)
									{
										Il2CppClass* parent_klass = IL2CPP::il2cpp_class_from_name(target_image, parent_mono_klass->name_space, parent_mono_klass->name);
										if (parent_klass == NULL)
										{
											void* nestedtype_iter = NULL;
											Il2CppClass* nestedtype = NULL;
											while ((klass == NULL) && ((nestedtype = IL2CPP::il2cpp_class_get_nested_types(parent_klass, &nestedtype_iter)) != NULL))
											{
												const char* nestedtype_name = IL2CPP::il2cpp_class_get_name(nestedtype);
												if (strstr(nestedtype_name, mono_klass->name))
												{
													klass = nestedtype;
													break;
												}
											}
										}
									}
								}
								if (klass != NULL)
									return klass;
							}
						}
					}
				}
			}
			return NULL;
		}
	}
	return IL2CPP::il2cpp_class_from_system_type(type);
}
#pragma endregion


#pragma region MetadataLoader_LoadMetadataFile
Il2CppGlobalMetadataHeader* HookManager::Hooked_MetadataLoader_LoadMetadataFile(const char* fileName)
{
	IL2CPP::s_GlobalMetadataHeader = IL2CPP::MetadataLoader_LoadMetadataFile(fileName);
	return IL2CPP::s_GlobalMetadataHeader;
}
#pragma endregion
*/


#pragma region SingleAppInstance_FindOtherInstance
bool __stdcall HookManager::Hooked_SingleAppInstance_FindOtherInstance(LPARAM lParam) { return false; }
#pragma endregion


#pragma region PlayerCleanup
bool HookManager::Hooked_PlayerCleanup(bool dopostquitmsg)
{
	MelonLoader::UNLOAD();
	return IL2CPPUnityPlayer::PlayerCleanup(dopostquitmsg);
}
#pragma endregion


#pragma region PlayerLoadFirstScene
void* HookManager::Hooked_PlayerLoadFirstScene(bool unknown)
{
	ModHandler::Initialize();
	if (MelonLoader::IsGameIl2Cpp && !MelonLoader::MupotMode)
		return IL2CPPUnityPlayer::PlayerLoadFirstScene(unknown);
	return MonoUnityPlayer::PlayerLoadFirstScene(unknown);
}
#pragma endregion


#pragma region BaseBehaviourManager_Update
__int64 HookManager::Hooked_BaseBehaviourManager_Update(void* behaviour_manager)
{
	__int64 returnval = IL2CPPUnityPlayer::BaseBehaviourManager_Update(behaviour_manager);
	ModHandler::OnUpdate();
	return returnval;
}
#pragma endregion


#pragma region BaseBehaviourManager_FixedUpdate
__int64 HookManager::Hooked_BaseBehaviourManager_FixedUpdate(void* behaviour_manager)
{
	__int64 returnval = IL2CPPUnityPlayer::BaseBehaviourManager_FixedUpdate(behaviour_manager);
	ModHandler::OnFixedUpdate();
	return returnval;
}
#pragma endregion


#pragma region BaseBehaviourManager_LateUpdate
__int64 HookManager::Hooked_BaseBehaviourManager_LateUpdate(void* behaviour_manager)
{
	__int64 returnval = IL2CPPUnityPlayer::BaseBehaviourManager_LateUpdate(behaviour_manager);
	ModHandler::OnLateUpdate();
	return returnval;
}
#pragma endregion


#pragma region GUIManager_DoGUIEvent
void HookManager::Hooked_GUIManager_DoGUIEvent(void* __0, void* __1, bool __2)
{
	ModHandler::OnGUI();
	IL2CPPUnityPlayer::GUIManager_DoGUIEvent(__0, __1, __2);
}
#pragma endregion

#pragma region EndOfFrameCallbacks_DequeAll
void HookManager::Hooked_EndOfFrameCallbacks_DequeAll()
{
	ModHandler::MelonCoroutines_ProcessWaitForEndOfFrame();
	IL2CPPUnityPlayer::EndOfFrameCallbacks_DequeAll();
}
#pragma endregion
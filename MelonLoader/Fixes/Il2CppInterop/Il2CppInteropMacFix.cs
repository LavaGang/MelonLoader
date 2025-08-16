#if NET6_0_OR_GREATER && OSX
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.HarmonySupport;
using Il2CppInterop.Runtime.Injection;

namespace MelonLoader.Fixes.Il2CppInterop
{
    internal class Il2CppInteropMacFix
    {
        private static MelonLogger.Instance _logger = new("Il2CppInterop");

        private static void LogMsg(string msg)
            => _logger.Msg(msg);
        private static void LogError(Exception ex)
            => _logger.Error(ex);
        private static void LogError(string msg, Exception ex)
            => _logger.Error(msg, ex);
        private static void LogDebugMsg(string msg)
        {
            if (!MelonDebug.IsEnabled())
                return;
            _logger.Msg(msg);
        }

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropMacFix);
                Type classInjectorType = typeof(ClassInjector);

                Type injectorHelpersType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers");
                if (injectorHelpersType == null)
                    throw new Exception("Failed to get InjectorHelpers");

                LogDebugMsg("Patching Il2CppInterop InjectorHelpers.Setup...");
                Core.HarmonyInstance.Patch(AccessTools.Method(injectorHelpersType, "Setup"),
                    null, null,
                    AccessTools.Method(thisType, nameof(InjectorHelpersSetup_Transpiler)).ToNewHarmonyMethod());
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        // This hook isn't reliable on macOS due to potential inlining by the player's compiler resulting in il2cppinterop
        // thinking it found the right function, but it actually found one that takes a pointer which it would then incorrectly hook
        // resulting in a crash. While it can hinder class injection functionality, it isn't needed for the game to boot,
        // and it didn't prevent UnityExplorer to function
        private static IEnumerable<CodeInstruction> InjectorHelpersSetup_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var injectorHelpersType = typeof(ClassInjector).Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers");
            var codeMatcher = new CodeMatcher(instructions);
            codeMatcher.MatchStartForward([
                    new(i => i.LoadsField(AccessTools.Field(injectorHelpersType, "GetTypeInfoFromTypeDefinitionIndexHook")))
                ]).RemoveInstructions(2);
            return codeMatcher.Instructions();
        }
    }
}
#endif
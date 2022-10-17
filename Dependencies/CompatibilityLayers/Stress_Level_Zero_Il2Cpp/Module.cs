using MelonLoader.Modules;
using System.Collections.Generic;

namespace MelonLoader.CompatibilityLayers
{
    internal class SLZ_Module : MelonModule
    {
        private static bool HasGotLoadingSceneIndex = false;
        private static int LoadingSceneIndex = -9;

        private static Dictionary<string, int> CompatibleGames = new Dictionary<string, int>
        {
            ["BONELAB"] = 0,
            ["BONEWORKS"] = 0
        };

        public override void OnInitialize()
        {
            if (!CompatibleGames.ContainsKey(InternalUtils.UnityInformationHandler.GameName))
                return;

            MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneLoad, int.MinValue);
            MelonEvents.OnSceneWasInitialized.Subscribe(OnSceneInit, int.MinValue);
        }

        private static void OnSceneLoad(int buildIndex, string name)
        {
            if (HasGotLoadingSceneIndex)
            {
                if (buildIndex == LoadingSceneIndex)
                    PreSceneEvent();
                return;
            }

            if (buildIndex == 0)
                return;

            HasGotLoadingSceneIndex = true;
            LoadingSceneIndex = buildIndex;
            PreSceneEvent();
        }

        private static void OnSceneInit(int buildIndex, string name)
        {
            if (!HasGotLoadingSceneIndex
                || (buildIndex != LoadingSceneIndex))
                return;

            PostSceneEvent();
            MelonBase.SendMessageAll("OnLoadingScreen");
            MelonBase.SendMessageAll($"{InternalUtils.UnityInformationHandler.GameName}_OnLoadingScreen");
        }

        private static MelonEvent<int, string>.MelonEventSubscriber[] SceneInitBackup;
        private static void PreSceneEvent()
        {
            SceneInitBackup = MelonEvents.OnSceneWasInitialized.GetSubscribers();
            MelonEvents.OnSceneWasInitialized.UnsubscribeAll();
            MelonEvents.OnSceneWasInitialized.Subscribe(OnSceneInit, int.MinValue);
        }
        private static void PostSceneEvent()
        {
            MelonEvents.OnSceneWasInitialized.UnsubscribeAll();
            foreach (var sub in SceneInitBackup)
                MelonEvents.OnSceneWasInitialized.Subscribe(sub.del, sub.priority, sub.unsubscribeOnFirstInvocation);
            SceneInitBackup = null;
        }
    }
}
namespace MelonLoader
{
    internal class SupportModule_From : ISupportModule_From
    {
        public void OnApplicationLateStart()
            => MelonEvents.OnApplicationLateStart.Invoke();

        public void OnSceneWasLoaded(int buildIndex, string sceneName)
            => MelonEvents.OnSceneWasLoaded.Invoke(buildIndex, sceneName);

        public void OnSceneWasInitialized(int buildIndex, string sceneName)
            => MelonEvents.OnSceneWasInitialized.Invoke(buildIndex, sceneName);

        public void OnSceneWasUnloaded(int buildIndex, string sceneName)
            => MelonEvents.OnSceneWasUnloaded.Invoke(buildIndex, sceneName);

        public void Update()
            => MelonEvents.OnUpdate.Invoke();

        public void FixedUpdate()
            => MelonEvents.OnFixedUpdate.Invoke();

        public void LateUpdate()
            => MelonEvents.OnLateUpdate.Invoke();

        public void OnGUI()
            => MelonEvents.OnGUI.Invoke();

        public void Quit()
            => MelonEvents.OnApplicationQuit.Invoke();

        public void DefiniteQuit()
        {
            MelonEvents.OnApplicationDefiniteQuit.Invoke();
            Core.Quit();
        }

        public void SetInteropSupportInterface(InteropSupport.Interface interop)
        {
            if (InteropSupport.SMInterface == null)
                InteropSupport.SMInterface = interop;
        }
    }
}
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static MelonLoaderComponent comp = null;
        private static ISupportModule Initialize()
        {
            MelonLoaderComponent.CreateComponent();
            return new Module();
        }
    }
    public class MelonLoaderComponent : MonoBehaviour
    {
        internal static void CreateComponent()
        {
            GameObject obj = new GameObject("MelonLoader");
            DontDestroyOnLoad(obj);
            Main.comp = obj.AddComponent<MelonLoaderComponent>();
            obj.transform.SetAsLastSibling();
            Main.comp.transform.SetAsLastSibling();
        }
        void Start() => transform.SetAsLastSibling();
        void Update()
        {
            transform.SetAsLastSibling();

            // Run Level Check Here

            MelonLoader.Main.OnUpdate();
        }
        void FixedUpdate() => MelonLoader.Main.OnFixedUpdate();
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() => CreateComponent();
        void OnApplicationQuit() => MelonLoader.Main.OnApplicationQuit();
    }
}
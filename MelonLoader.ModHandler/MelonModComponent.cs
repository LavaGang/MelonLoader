using UnityEngine;

namespace MelonLoader
{
    internal class MelonModComponent : MonoBehaviour
    {
        internal static void Create()
        {
            if (Imports.IsIl2CppGame())
                Main.OnApplicationStart();
            else
                CreateComponent();
        }

        private static void CreateComponent()
        {
            GameObject obj = new GameObject("MelonLoader");
            GameObject.DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<MelonModComponent>();
        }

        internal static MelonModComponent Instance = null;
        static bool has_started = false;
        void Start() { if (!has_started) { Main.OnApplicationStart(); has_started = true; } }
        //void OnLevelWasLoaded(int level) { transform.SetAsLastSibling(); Main.OnLevelWasLoaded(level); }
        void Update() => Main.OnUpdate();
        void FixedUpdate() => Main.OnFixedUpdate();
        void LateUpdate() => Main.OnLateUpdate();
        void OnGUI() => Main.OnGUI();
        void OnDestroy() => CreateComponent();
    }
}
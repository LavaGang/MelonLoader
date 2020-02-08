using System.Threading;
using UnityEngine;

namespace MelonLoader
{
    internal class MelonModComponent : MonoBehaviour
    {
        internal static void Create()
        {
            if (Imports.melonloader_is_il2cpp_game())
            {
                if (Imports.melonloader_is_mupot_mode())
                    CreateComponent();
                else
                    Main.OnApplicationStart();
            }
            else
                CreateComponent();
        }

        private static void CreateComponent()
        {
            GameObject obj = new GameObject("MelonLoader");
            GameObject.DontDestroyOnLoad(obj);
            obj.AddComponent<MelonModComponent>();
        }

        static bool has_started = false;
        void Start() { if (!has_started) { Main.OnApplicationStart(); has_started = true; } }
        void OnLevelWasLoaded(int level) { transform.SetAsLastSibling(); Main.OnLevelWasLoaded(level); }
        void Update() => Main.OnUpdate();
        void FixedUpdate() => Main.OnFixedUpdate();
        void LateUpdate() => Main.OnLateUpdate();
        void OnGUI() => Main.OnGUI();
        void OnDestroy() => CreateComponent();
        void OnApplicationQuit() => Main.OnApplicationQuit();
    }
}
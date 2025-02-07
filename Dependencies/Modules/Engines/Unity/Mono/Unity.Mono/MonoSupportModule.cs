using MelonLoader.Modules;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Mono
{
    internal class MonoSupportModule : MelonSupportModule
    {
        internal MonoSceneHandler sceneHandler;

        internal GameObject obj;
        internal MonoSupportComponent component;

        public override void Initialize()
        {
            // Initialize Tomlet Unity Object Serialization
            MonoTomletProvider.Initialize();

            // Create Scene Handler
            sceneHandler = new(this);

            // Create GameObject and Component
            if (component == null)
                CreateGameObject();
        }

        internal void CreateGameObject()
        {
            // Create Support GameObject
            obj = new GameObject();
            GameObject.DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.DontSave;

            // Create Support Component
            component = obj.AddComponent<MonoSupportComponent>();
            component.SiblingFix();

            // Create Interop for Coroutine Management
            MelonCoroutines.Interop = new MonoCoroutineInterop(component);
        }
    }
}

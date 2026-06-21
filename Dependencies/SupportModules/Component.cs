using UnityEngine;

#if SM_Il2Cpp
using Il2CppInterop.Common.Attributes;
#endif

namespace MelonLoader.Support
{
#if SM_Il2Cpp
    [InjectedType]
#endif
    internal partial class SM_Component : MonoBehaviour
    {
#if SM_Il2Cpp
        [Il2CppField]
        private partial Il2CppSystem.Boolean isQuitting { get; set; }
#else
        private bool isQuitting;
#endif

        internal static void Create()
        {
            if (Main.component != null)
                return;

            MelonCoroutines._hasProcessed = false;

            Main.obj = new GameObject();
            DontDestroyOnLoad(Main.obj);
            Main.obj.hideFlags = HideFlags.DontSave;

#if SM_Il2Cpp
            Main.component = Main.obj.AddComponent<SM_Component>();
#else
            Main.component = (SM_Component)Main.obj.AddComponent(typeof(SM_Component));
#endif

            ComponentSiblingFix.SetAsLastSibling(Main.obj.transform);
        }

        private void ProcessCoroutineQueue()
        {
            MelonCoroutines._hasProcessed = true;

            if (MelonCoroutines._queue.Count <= 0)
                return;

            foreach (var queuedCoroutine in MelonCoroutines._queue)
#if SM_Il2Cpp
                StartCoroutine(new MonoEnumeratorWrapper(queuedCoroutine));
#else
                StartCoroutine(queuedCoroutine);
#endif

            MelonCoroutines._queue.Clear();
        }

        void Start()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            ComponentSiblingFix.SetAsLastSibling(transform);
            Main.Interface.OnApplicationLateStart();
        }

        void Awake()
        {
            ProcessCoroutineQueue();
        }

        void Update()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            isQuitting = false;
            ComponentSiblingFix.SetAsLastSibling(transform);

            SceneHandler.OnUpdate();
            Main.Interface.Update();
        }

        void OnDestroy()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            if (!isQuitting)
            {
                Create();
                return;
            }

            OnApplicationDefiniteQuit();
        }

        void OnApplicationQuit()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            isQuitting = true;
            Main.Interface.Quit();
        }

        void OnApplicationDefiniteQuit()
        {
            Main.Interface.DefiniteQuit();
        }

        void FixedUpdate()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            Main.Interface.FixedUpdate();
        }

        void LateUpdate()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            Main.Interface.LateUpdate();
        }

        void OnGUI()
        {
            if ((Main.component == null) || (Main.component != this))
                return;

            Main.Interface.OnGUI();
        }
    }
}
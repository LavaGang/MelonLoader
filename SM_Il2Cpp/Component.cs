using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SM_Component : MonoBehaviour
    {
        private static bool IsDestroying = false;
        private delegate bool SetAsLastSiblingDelegate(IntPtr transformptr);
        private static SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;
        public SM_Component(IntPtr value) : base(value) { }
        public SM_Component() : base(UnhollowerRuntimeLib.ClassInjector.DerivedConstructorPointer<SM_Component>()) { UnhollowerRuntimeLib.ClassInjector.DerivedConstructorBody(this); }
        static SM_Component() => SetAsLastSiblingDelegateField = IL2CPP.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");
        internal static void Create()
        {
            Main.obj = new GameObject();
            DontDestroyOnLoad(Main.obj);
            Main.component = Main.obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<SM_Component>()).TryCast<SM_Component>();
            Main.component.SiblingFix();
        }
        private void SiblingFix()
        {
            SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(gameObject.transform));
            SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(transform));
        }
        internal void Destroy()
        {
            IsDestroying = true;
            GameObject.Destroy(gameObject);
        }
        void Start() => SiblingFix();
        void Update()
        {
            SiblingFix();
            if (MelonUtils.IsBONEWORKS)
                BONEWORKS_SceneHandler.OnUpdate();
            Main.Interface.Update();
            Coroutines.Process();
        }
        void FixedUpdate()
        {
            Main.Interface.FixedUpdate();
            Coroutines.ProcessWaitForFixedUpdate();
        }
        void LateUpdate() => Main.Interface.LateUpdate();
        void OnGUI() => Main.Interface.OnGUI();
        void OnDestroy() { if (!IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); Main.Interface.Quit(); }
    }
}
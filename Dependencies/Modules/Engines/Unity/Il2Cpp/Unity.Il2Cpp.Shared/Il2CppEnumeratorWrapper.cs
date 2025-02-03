using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    public class Il2CppEnumeratorWrapper : Il2CppSystem.Object /*, IEnumerator */
    {
        internal unsafe static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<Il2CppEnumeratorWrapper>(new()
            {
                LogSuccess = true,
                Interfaces = new Type[] { typeof(Il2CppSystem.Collections.IEnumerator) }
            });

        private readonly IEnumerator enumerator;
        public Il2CppEnumeratorWrapper(IntPtr ptr) : base(ptr) { }
        public Il2CppEnumeratorWrapper(IEnumerator _enumerator) : base(ClassInjector.DerivedConstructorPointer<Il2CppEnumeratorWrapper>())
        {
            ClassInjector.DerivedConstructorBody(this);
            enumerator = _enumerator ?? throw new NullReferenceException("routine is null");
        }

        public Il2CppSystem.Object /*IEnumerator.*/Current
        {
            get => enumerator.Current switch
                {
                    IEnumerator next => new Il2CppEnumeratorWrapper(next),
                    Il2CppSystem.Object il2cppObject => il2cppObject,
                    null => null,
                    _ => throw new NotSupportedException($"{enumerator.GetType()}: Unsupported type {enumerator.Current.GetType()}"),
                };
        }

        public bool MoveNext()
        {
            try
            {
                return enumerator.MoveNext();
            } catch(Exception e)
            {
                var melon = MelonUtils.GetMelonFromStackTrace(new System.Diagnostics.StackTrace(e), true);

                if (melon != null)
                    melon.LoggerInstance.Error("Unhandled exception in coroutine. It will not continue executing.", e);
                else
                    MelonLogger.Error("[Error: Could not identify source] Unhandled exception in coroutine. It will not continue executing.", e);

                return false;
            }
        }
        
        public void Reset() => enumerator.Reset();
    }
}

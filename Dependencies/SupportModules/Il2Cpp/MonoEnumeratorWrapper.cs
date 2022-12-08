using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;

namespace MelonLoader.Support
{
    internal class MonoEnumeratorWrapper : Il2CppSystem.Object /*, IEnumerator */
    {
        public unsafe static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<MonoEnumeratorWrapper>(new()
            {
                LogSuccess = true,
                Interfaces = new Type[] { typeof(Il2CppSystem.Collections.IEnumerator) }
            });

        private readonly IEnumerator enumerator;
        public MonoEnumeratorWrapper(IntPtr ptr) : base(ptr) { }
        public MonoEnumeratorWrapper(IEnumerator _enumerator) : base(ClassInjector.DerivedConstructorPointer<MonoEnumeratorWrapper>())
        {
            ClassInjector.DerivedConstructorBody(this);
            enumerator = _enumerator ?? throw new NullReferenceException("routine is null");;
        }

        public Il2CppSystem.Object /*IEnumerator.*/Current
        {
            get => enumerator.Current switch
                {
                    IEnumerator next => new MonoEnumeratorWrapper(next),
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

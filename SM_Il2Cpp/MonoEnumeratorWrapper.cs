using System;
using System.Collections;
using UnhollowerRuntimeLib;

namespace MelonLoader.Support
{
    internal class MonoEnumeratorWrapper : Il2CppSystem.Object /*, IEnumerator */
    {
        public unsafe static void Register()
            => ClassInjector.RegisterTypeInIl2CppWithInterfaces<MonoEnumeratorWrapper>(true, typeof(Il2CppSystem.Collections.IEnumerator));

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

        public bool MoveNext() => enumerator.MoveNext();
        public void Reset() => enumerator.Reset();
    }
}

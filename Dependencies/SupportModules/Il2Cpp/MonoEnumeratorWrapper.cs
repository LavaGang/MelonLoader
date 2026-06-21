using Il2CppInterop.Common;
using Il2CppInterop.Common.Attributes;
using System;
using System.Collections;

namespace MelonLoader.Support
{
    [InjectedType]
    public partial class MonoEnumeratorWrapper : Il2CppSystem.Object, Il2CppSystem.Collections.IEnumerator
    {
        [ManagedField]
        private partial IEnumerator enumerator { get; set; }

        public MonoEnumeratorWrapper(IEnumerator _enumerator) : this(ObjectPointer.New<MonoEnumeratorWrapper>())
        {
            enumerator = _enumerator ?? throw new NullReferenceException("routine is null");
        }

        public Il2CppSystem.IObject Current
        {
            get => enumerator.Current switch
                {
                    IEnumerator next => new MonoEnumeratorWrapper(next),
                    Il2CppSystem.IObject il2cppObject => il2cppObject,
                    null => null,
                    _ => throw new NotSupportedException($"{enumerator.GetType()}: Unsupported type {enumerator.Current.GetType()}"),
                };
        }

        public Il2CppSystem.Boolean MoveNext()
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

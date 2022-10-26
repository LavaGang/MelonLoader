using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MelonLoader
{
    public abstract class MelonTypeBase<T> : MelonBase where T : MelonTypeBase<T>
    {
        /// <summary>
        /// List of registered <typeparamref name="T"/>s.
        /// </summary>
        new public static ReadOnlyCollection<T> RegisteredMelons => _registeredMelons.AsReadOnly();
        new internal static List<T> _registeredMelons = new();

        /// <summary>
        /// A Human-Readable Name for <typeparamref name="T"/>.
        /// </summary>
        public static string TypeName { get; protected internal set; }

        static MelonTypeBase()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // To make sure that the type initializer of T was triggered.
        }

        public sealed override string MelonTypeName => TypeName;

        protected private override bool RegisterInternal()
        {
            if (!base.RegisterInternal())
                return false;
            _registeredMelons.Add((T)this);
            return true;
        }

        protected private override void UnregisterInternal()
        {
            _registeredMelons.Remove((T)this);
        }

        public static void ExecuteAll(LemonAction<T> func, bool unregisterOnFail = false, string unregistrationReason = null)
            => ExecuteList(func, _registeredMelons, unregisterOnFail, unregistrationReason);
    }
}

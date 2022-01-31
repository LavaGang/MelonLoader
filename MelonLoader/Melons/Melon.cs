using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    public abstract class Melon<T> : MelonBase where T : Melon<T>
    {
        /// <summary>
        /// List of registered <typeparamref name="T"/>s.
        /// </summary>
        new public static List<T> RegisteredMelons => _registeredMelons.AsReadOnly().ToList();
        new internal static List<T> _registeredMelons = new List<T>();

        /// <summary>
        /// A Human-Readable Name for <typeparamref name="T"/>.
        /// </summary>
        public static string TypeName { get; protected internal set; }

        static Melon()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // to make sure the type initializer was triggered
        }

        public sealed override string MelonTypeName => TypeName;

        public sealed override bool Register()
        {
            if (!base.Register())
                return false;

            _registeredMelons.Add((T)this);
            return true;
        }

        public sealed override bool Unregister(string reason = null)
        {
            if (!base.Unregister(reason))
                return false;

            _registeredMelons.Remove((T)this);
            return true;
        }

        public static void ExecuteAll(LemonAction<T> func, bool unregisterOnFail = false, string unregistrationReason = null)
            => ExecuteList(func, _registeredMelons, unregisterOnFail, unregistrationReason);
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader
{
    public abstract class MelonEventBase<T> where T : Delegate
    {
        private readonly List<MelonAction> actions = new List<MelonAction>();

        public void Subscribe(T action, bool unsubscribeOnFirstInvokation = false)
        {
            lock (actions)
            {
                var acts = MelonAction.Get(action, unsubscribeOnFirstInvokation);
                foreach (var a in acts)
                {
                    if (a.Melon != null)
                    {
                        a.Melon.OnUnregister.Subscribe(() => Unsubscribe(a.method, a.obj), true);
                    }

                    actions.Add(a);
                }
            }
        }

        private void Unsubscribe(MethodInfo method, object obj = null)
        {
            lock (actions)
            {
                if (method.IsStatic)
                    obj = null;

                actions.RemoveAll(x => x.method == method && (obj == null || x.obj == obj));
            }
        }

        protected void Invoke(bool unregisterFailedMelons, params object[] args)
        {
            MelonAction[] copy;
            lock (actions)
                copy = actions.ToArray();

            LemonEnumerator<MelonAction> enumerator = new LemonEnumerator<MelonAction>(copy);
            while (enumerator.MoveNext())
            {
                var del = enumerator.Current;
                if (del.Melon != null && !del.Melon.Registered)
                    continue;

                try { del.Invoke(args); }
                catch (Exception ex)
                {
                    if (del.Melon == null)
                        MelonLogger.Error(ex.ToString());
                    else
                        del.Melon.LoggerInstance.Error(ex.ToString());

                    if (unregisterFailedMelons && del.Melon != null)
                        del.Melon.Unregister($"Failed to properly execute action: {del.method.DeclaringType.FullName}::{del.method.Name}");
                }

                if (del.unsubscribeOnFirstInvokation)
                    Unsubscribe(del.method, del.obj);
            }
        }

        public void Dispose()
        {
            actions.Clear();
        }
    }

    #region Param Children
    public class MelonEvent : MelonEventBase<LemonAction>
    {
        public void Invoke(bool unregisterFailedMelons = false)
            => base.Invoke(unregisterFailedMelons);
    }
    public class MelonEvent<T1> : MelonEventBase<LemonAction<T1>>
    {
        public void Invoke(T1 arg1, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1);
    }
    public class MelonEvent<T1, T2> : MelonEventBase<LemonAction<T1, T2>>
    {
        public void Invoke(T1 arg1, T2 arg2, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2);
    }
    public class MelonEvent<T1, T2, T3> : MelonEventBase<LemonAction<T1, T2, T3>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3);
    }
    public class MelonEvent<T1, T2, T3, T4> : MelonEventBase<LemonAction<T1, T2, T3, T4>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3, arg4);
    }
    public class MelonEvent<T1, T2, T3, T4, T5> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3, arg4, arg5);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3, arg4, arg5, arg6);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7, T8> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, bool unregisterFailedMelons = false)
            => Invoke(unregisterFailedMelons, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
    #endregion
}

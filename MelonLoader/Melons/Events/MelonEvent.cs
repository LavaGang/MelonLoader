using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader
{
    public abstract class MelonEventBase<T> where T : Delegate
    {
        private readonly List<MelonAction> actions = new List<MelonAction>();
        public readonly bool oneTimeUse;

        public bool Disposed { get; private set; }

        public MelonEventBase(bool oneTimeUse = false)
        {
            this.oneTimeUse = oneTimeUse;
        }

        public bool CheckIfSubscribed(MethodInfo method, object obj = null)
            => actions.Exists(x => x.method == method && (obj == null || x.obj == obj));

        public void Subscribe(T action, bool unsubscribeOnFirstInvokation = false)
        {
            if (Disposed)
                return;

            lock (actions)
            {
                var acts = MelonAction.Get(action, unsubscribeOnFirstInvokation);
                foreach (var a in acts)
                {
                    if (CheckIfSubscribed(a.method, a.obj))
                        continue;

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
            if (Disposed)
                return;

            lock (actions)
            {
                if (method.IsStatic)
                    obj = null;

                actions.RemoveAll(x => x.method == method && (obj == null || x.obj == obj));
            }
        }

        protected void Invoke(params object[] args)
        {
            if (Disposed)
                return;

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
                }

                if (del.unsubscribeOnFirstInvokation)
                    Unsubscribe(del.method, del.obj);
            }

            if (oneTimeUse)
                Dispose();
        }

        public void Dispose()
        {
            actions.Clear();
            Disposed = true;
        }
    }

    #region Param Children
    public class MelonEvent : MelonEventBase<LemonAction>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke()
            => base.Invoke();
    }
    public class MelonEvent<T1> : MelonEventBase<LemonAction<T1>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1)
            => Invoke(arg1);
    }
    public class MelonEvent<T1, T2> : MelonEventBase<LemonAction<T1, T2>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2)
            => Invoke(arg1, arg2);
    }
    public class MelonEvent<T1, T2, T3> : MelonEventBase<LemonAction<T1, T2, T3>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
            => Invoke(arg1, arg2, arg3);
    }
    public class MelonEvent<T1, T2, T3, T4> : MelonEventBase<LemonAction<T1, T2, T3, T4>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => Invoke(arg1, arg2, arg3, arg4);
    }
    public class MelonEvent<T1, T2, T3, T4, T5> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => Invoke(arg1, arg2, arg3, arg4, arg5);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7, T8> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
    #endregion
}

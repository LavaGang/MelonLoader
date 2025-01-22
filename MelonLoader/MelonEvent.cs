using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader
{
    public abstract class MelonEventBase<T> where T : Delegate
    {
        private readonly List<MelonAction<T>> actions = new();
        private MelonAction<T>[] cachedActionsArray = new MelonAction<T>[0];
        public readonly bool oneTimeUse;

        public bool Disposed { get; private set; }

        public MelonEventBase(bool oneTimeUse = false)
        {
            this.oneTimeUse = oneTimeUse;
        }

        public bool CheckIfSubscribed(MethodInfo method, object obj = null)
        {
            lock (actions)
            {
                return actions.Exists(x => x.del.Method == method && (obj == null || x.del.Target == obj));
            }
        }

        public void Subscribe(T action, int priority = 0, bool unsubscribeOnFirstInvocation = false)
        {
            if (Disposed)
                return;

            lock (actions)
            {
                var acts = MelonAction<T>.Get(action, priority, unsubscribeOnFirstInvocation);
                foreach (var a in acts)
                {
                    if (CheckIfSubscribed(a.del.Method, a.del.Target))
                        continue;

                    if (a.melonAssembly != null)
                    {
                        MelonDebug.Msg($"MelonAssembly '{a.melonAssembly.Assembly.GetName().Name}' subscribed with {a.del.Method.Name}");
                        a.melonAssembly.OnUnregister.Subscribe(() => Unsubscribe(a.del), unsubscribeOnFirstInvocation: true);
                    }

                    for (var b = 0; b < actions.Count; b++)
                    {
                        var act = actions[b];
                        if (a.priority < act.priority)
                        {
                            actions.Insert(b, a);
                            UpdateEnumerator();
                            return;
                        }
                    }

                    actions.Add(a);
                    UpdateEnumerator();
                }
            }
        }

        public void Unsubscribe(T action)
        {
            foreach (var inv in action.GetInvocationList())
            {
                Unsubscribe(inv.Method, inv.Target);
            }
        }

        public void UnsubscribeAll()
        {
            lock (actions)
                actions.Clear();

            UpdateEnumerator();
        }

        public void Unsubscribe(MethodInfo method, object obj = null)
        {
            if (Disposed)
                return;

            lock (actions)
            {
                if (method.IsStatic)
                    obj = null;

                var any = false;
                for (var a = 0; a < actions.Count; a++)
                {
                    var act = actions[a];
                    if (act.del.Method != method || (obj != null && act.del.Target != obj))
                        continue;

                    any = true;
                    actions.RemoveAt(a);
                    if (act.melonAssembly != null)
                        MelonDebug.Msg($"MelonAssembly '{act.melonAssembly.Assembly.GetName().Name}' unsubscribed with {act.del.Method.Name}");
                }

                if (any)
                    UpdateEnumerator();
            }
        }

        private void UpdateEnumerator()
        {
            cachedActionsArray = actions.ToArray();
        }

        public class MelonEventSubscriber
        {
            public T del;
            public bool unsubscribeOnFirstInvocation;
            public int priority;
            public MelonAssembly melonAssembly;
        }
        public MelonEventSubscriber[] GetSubscribers()
        {
            List<MelonEventSubscriber> allSubs = new List<MelonEventSubscriber>();
            foreach (var act in actions)
                allSubs.Add(new MelonEventSubscriber
                {
                    del = act.del,
                    unsubscribeOnFirstInvocation = act.unsubscribeOnFirstInvocation,
                    priority = act.priority,
                    melonAssembly = act.melonAssembly
                });
            return allSubs.ToArray();
        }

        protected void Invoke(Action<T> delegateInvoker)
        {
            if (Disposed)
                return;

            var actionsArray = cachedActionsArray;
            for (var a = 0; a < actionsArray.Length; a++)
            {
                var del = actionsArray[a];
                try { delegateInvoker(del.del); }
                catch (Exception ex)
                {
                    if (del.melonAssembly == null || del.melonAssembly.LoadedMelons.Count == 0)
                        MelonLogger.Error(ex.ToString());
                    else
                        del.melonAssembly.LoadedMelons[0].LoggerInstance.Error(ex.ToString());
                }

                if (del.unsubscribeOnFirstInvocation)
                    Unsubscribe(del.del);
            }

            if (oneTimeUse)
                Dispose();
        }

        public void Dispose()
        {
            UnsubscribeAll();
            Disposed = true;
        }
    }

    #region Param Children
    public class MelonEvent : MelonEventBase<LemonAction>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke()
        {
            Invoke(x => x());
        }
    }
    public class MelonEvent<T1> : MelonEventBase<LemonAction<T1>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1)
        {
            Invoke(x => x(arg1));
        }
    }
    public class MelonEvent<T1, T2> : MelonEventBase<LemonAction<T1, T2>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Invoke(x => x(arg1, arg2));
        }
    }
    public class MelonEvent<T1, T2, T3> : MelonEventBase<LemonAction<T1, T2, T3>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Invoke(x => x(arg1, arg2, arg3));
        }
    }
    public class MelonEvent<T1, T2, T3, T4> : MelonEventBase<LemonAction<T1, T2, T3, T4>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4));
        }
    }
    public class MelonEvent<T1, T2, T3, T4, T5> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5));
        }
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6));
        }
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }
    }
    public class MelonEvent<T1, T2, T3, T4, T5, T6, T7, T8> : MelonEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public MelonEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
        }
    }
    #endregion
}

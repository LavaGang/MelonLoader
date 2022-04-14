using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader
{
    internal class MelonAction
    {
        internal readonly MethodInfo method;
        internal readonly object obj;
        internal readonly bool unsubscribeOnFirstInvocation;
        internal readonly int priority;

        internal MelonBase Melon => obj is MelonBase melon ? melon : null;

        private MelonAction(Delegate singleDel, int priority, bool unsubscribeOnFirstInvocation)
        {
            method = singleDel.Method;
            obj = singleDel.Target;
            this.priority = priority;
            this.unsubscribeOnFirstInvocation = unsubscribeOnFirstInvocation;
        }

        internal void Invoke(params object[] args)
        {
            method.Invoke(obj, args);
        }

        internal static List<MelonAction> Get(Delegate del, int priority = 0, bool unsubscribeOnFirstInvocation = false)
        {
            var mets = del.GetInvocationList();
            var result = new List<MelonAction>();
            foreach (var met in mets)
            {
                if (met.Target != null && met.Target is MelonBase melon && !melon.Registered)
                    continue;

                result.Add(new MelonAction(met, priority, unsubscribeOnFirstInvocation));
            }
            return result;
        }
    }
}

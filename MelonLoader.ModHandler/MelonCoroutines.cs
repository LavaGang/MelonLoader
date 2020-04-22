using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MelonLoader
{
    public class MelonCoroutines
    {
        public class CoroD
        {
            private System.Type CoroutineType;
            internal object Coroutine;
            private MethodInfo MoveNextMethod;
            private PropertyInfo CurrentProp;

            internal CoroD(System.Type coroutineType, object coroutine)
            {
                CoroutineType = coroutineType;
                Coroutine = coroutine;
            }

            internal object GetCurrent()
            {
                if (CurrentProp == null)
                    CurrentProp = CoroutineType.GetProperty("Current", BindingFlags.Instance | BindingFlags.Public);
                if (CurrentProp != null)
                    return CurrentProp.GetGetMethod().Invoke(Coroutine, new object[] { });
                return null;
            }

            internal bool MoveNext()
            {
                if (MoveNextMethod == null)
                    MoveNextMethod = CoroutineType.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public);
                if (MoveNextMethod != null)
                    return (bool)MoveNextMethod.Invoke(Coroutine, new object[] { });
                return true;
            }
        }

        private struct CoroTuple
        {
            public object WaitCondition;
            public CoroD Coroutine;
        }
        private static List<CoroTuple> ourCoroutinesStore = new List<CoroTuple>();
        private static List<CoroD> ourNextFrameCoroutines = new List<CoroD>();

        private static MethodInfo StartCoroutineMethod = null;
        private static MethodInfo StopCoroutineMethod = null;

        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine. Usually an IEnumerator</param>
        /// <returns></returns>
        public static CoroD Start<T>(T routine)
        {
            if (routine != null)
            {
                if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                {
                    CoroD corod = new CoroD(typeof(T), routine);
                    ProcessNextOfCoroutine(corod);
                    return corod;
                }
                else
                {
                    if (StartCoroutineMethod == null)
                        StartCoroutineMethod = typeof(MonoBehaviour).GetMethods(BindingFlags.Instance | BindingFlags.Public).First(x => (x.Name.Equals("StartCoroutine") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].GetType() == typeof(IEnumerator))));
                    if (StartCoroutineMethod != null)
                    {
                        object coroutine = StartCoroutineMethod.Invoke(MelonModComponent.Instance, new object[] { routine }) as Coroutine;
                        CoroD corod = new CoroD(coroutine.GetType(), coroutine);
                        return corod;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Stop a currently running coroutine
        /// </summary>
        /// <param name="corod">The coroutine to stop</param>
        public static void Stop(CoroD corod)
        {
            if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                StopIl2CppCoroD(corod);
            else
            {
                if (StopCoroutineMethod == null)
                    StopCoroutineMethod = typeof(MonoBehaviour).GetMethods(BindingFlags.Instance | BindingFlags.Public).First(x => (x.Name.Equals("StopCoroutine") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].GetType() == typeof(Coroutine))));
                if (StopCoroutineMethod != null)
                    StopCoroutineMethod.Invoke(MelonModComponent.Instance, new object[] { corod.Coroutine });
            }
        }

        private static void StopIl2CppCoroD(CoroD corod)
        {
            if (ourNextFrameCoroutines.Contains(corod)) // the coroutine is running itself
                ourNextFrameCoroutines.Remove(corod);
            else
            {
                int coroTupleIndex = ourCoroutinesStore.FindIndex(c => c.Coroutine == corod);
                if (coroTupleIndex != -1) // the coroutine is waiting for a subroutine
                {
                    object waitCondition = ourCoroutinesStore[coroTupleIndex].WaitCondition;
                    if (waitCondition is CoroD)
                        StopIl2CppCoroD(waitCondition as CoroD);

                    ourCoroutinesStore.RemoveAt(coroTupleIndex);
                }
            }
        }

        private static bool HasCheckWaitCondition = false;
        private static bool WaitConditionIsIl2Cpp = true;
        private static FieldInfo field_m_Seconds = null;
        private static PropertyInfo prop_m_Seconds = null;
        internal static void Process()
        {
            if (HasCheckWaitCondition == false)
            {
                WaitConditionIsIl2Cpp = (typeof(WaitForSeconds).GetProperty("m_Seconds", BindingFlags.Instance | BindingFlags.Public) != null);
                HasCheckWaitCondition = true;
            }
            for (var i = ourCoroutinesStore.Count - 1; i >= 0; i--)
            {
                var tuple = ourCoroutinesStore[i];
                switch (tuple.WaitCondition)
                {
                    case CustomYieldInstruction customYield:
                        if (!customYield.keepWaiting)
                        {
                            ourCoroutinesStore.RemoveAt(i);
                            ProcessNextOfCoroutine(tuple.Coroutine);
                        }
                        break;
                    case WaitForSeconds waitForSeconds:
                        float m_Seconds = 0;
                        if (WaitConditionIsIl2Cpp)
                        {
                            if (prop_m_Seconds == null)
                                prop_m_Seconds = typeof(WaitForSeconds).GetProperty("m_Seconds");
                            if (prop_m_Seconds != null)
                            {
                                m_Seconds = (float)prop_m_Seconds.GetValue(waitForSeconds);
                                prop_m_Seconds.SetValue(waitForSeconds, (m_Seconds - Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (field_m_Seconds == null)
                                field_m_Seconds = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic);
                            if (field_m_Seconds != null)
                            {
                               m_Seconds = (float)field_m_Seconds.GetValue(waitForSeconds);
                               field_m_Seconds.SetValue(waitForSeconds, (m_Seconds - Time.deltaTime));
                            }
                        }
                        if (m_Seconds <= 0)
                        {
                            ourCoroutinesStore.RemoveAt(i);
                            ProcessNextOfCoroutine(tuple.Coroutine);
                        }
                        break;
                }
            }
            if (ourNextFrameCoroutines.Count == 0)
                return;
            var oldCoros = ourNextFrameCoroutines.ToArray();
            ourNextFrameCoroutines.Clear();
            foreach (var nextFrameCoroutine in oldCoros)
                ProcessNextOfCoroutine(nextFrameCoroutine);
        }

        internal static void ProcessWaitForFixedUpdate()
        {
            for (var i = ourCoroutinesStore.Count - 1; i >= 0; i--)
            {
                var tuple = ourCoroutinesStore[i];
                switch (tuple.WaitCondition)
                {
                    case WaitForFixedUpdate waitForFixedUpdate:
                        ourCoroutinesStore.RemoveAt(i);
                        ProcessNextOfCoroutine(tuple.Coroutine);
                        break;
                }
            }
        }

        private static void ProcessWaitForEndOfFrame()
        {
            for (var i = ourCoroutinesStore.Count - 1; i >= 0; i--)
            {
                var tuple = ourCoroutinesStore[i];
                switch (tuple.WaitCondition)
                {
                    case WaitForEndOfFrame waitForEndOfFrame:
                        ourCoroutinesStore.RemoveAt(i);
                        ProcessNextOfCoroutine(tuple.Coroutine);
                        break;
                }
            }
        }

        private static void ProcessNextOfCoroutine(CoroD enumerator)
        {
            try
            {
                if (!enumerator.MoveNext()) // Run the next step of the coroutine. If it's done, restore the parent routine
                {
                    var indices = ourCoroutinesStore.Select((it, idx) => (idx, it)).Where(it => it.it.WaitCondition == enumerator).Select(it => it.idx).ToList();
                    for (var i = indices.Count() - 1; i >= 0; i--)
                    {
                        var index = indices[i];
                        ourNextFrameCoroutines.Add(ourCoroutinesStore[index].Coroutine);
                        ourCoroutinesStore.RemoveAt(index);
                    }
                    return;
                }
            }
            catch (System.Exception e)
            {
                MelonModLogger.LogError(e.ToString());
                StopIl2CppCoroD(FindOriginalCoroD(enumerator)); // We want the entire coroutine hierachy to stop when an error happen
            }

            var next = enumerator.GetCurrent();
            if (next == null)
                ourNextFrameCoroutines.Add(enumerator);
            else
            {
                if (next is IEnumerator)
                    next = new CoroD(typeof(IEnumerator), next); // Convert IEnumerators to CoroD, so we only have CoroDs ran, and no IEnumerators

                ourCoroutinesStore.Add(new CoroTuple() { WaitCondition = next, Coroutine = enumerator });

                if (next is CoroD nextCoro)
                    ProcessNextOfCoroutine(nextCoro);
            }
        }

        private static CoroD FindOriginalCoroD(CoroD enumerator)
        {
            int index = ourCoroutinesStore.FindIndex(ct => ct.WaitCondition == enumerator);
            if (index == -1)
                return enumerator;
            return FindOriginalCoroD(ourCoroutinesStore[index].Coroutine);
        }
    }
}

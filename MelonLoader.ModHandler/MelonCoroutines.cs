using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MelonLoader
{
    public class MelonCoroutines
    {
        private struct CoroTuple
        {
            public object WaitCondition;
            public IEnumerator Coroutine;
        }
        private static List<CoroTuple> ourCoroutinesStore = new List<CoroTuple>();
        private static List<IEnumerator> ourNextFrameCoroutines = new List<IEnumerator>();

        public static void Start(IEnumerator routine)
        {
            if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                ProcessNextOfCoroutine(routine);
            else
                MelonModComponent.Instance.StartCoroutine(routine);
        }

        static FieldInfo field_m_Seconds = null;
        internal static void Process()
        {
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
                        if (field_m_Seconds == null)
                            field_m_Seconds = typeof(WaitForSeconds).GetField("m_Seconds");
                        float m_Seconds = (float)field_m_Seconds.GetValue(waitForSeconds);
                        field_m_Seconds.SetValue(waitForSeconds, (m_Seconds - Time.deltaTime));
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

        private static void ProcessNextOfCoroutine(IEnumerator enumerator)
        {
            if (!enumerator.MoveNext())
            {
                var indices = ourCoroutinesStore.Select((it, idx) => (idx, it)).Where(it => it.it.WaitCondition == enumerator).Select(it => it.idx).ToList();
                for (var i = indices.Count - 1; i >= 0; i--)
                {
                    var index = indices[i];
                    ourNextFrameCoroutines.Add(ourCoroutinesStore[index].Coroutine);
                    ourCoroutinesStore.RemoveAt(index);
                }
                return;
            }
            var next = enumerator.Current;
            if (next == null)
                ourNextFrameCoroutines.Add(enumerator);
            else
            {
                if (next is IEnumerator nextCoro)
                    ProcessNextOfCoroutine(nextCoro);
                ourCoroutinesStore.Add(new CoroTuple() { WaitCondition = next, Coroutine = enumerator });
            }
        }
    }
}

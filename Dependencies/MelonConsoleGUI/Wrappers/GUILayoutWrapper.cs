using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Wrappers
{
    internal static class GUILayoutWrapper
    {
        private readonly static MethodInfo textFieldMethod;

        public static readonly LemonFunc<string, GUILayoutOption[], string> TextField;

        static GUILayoutWrapper()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                textFieldMethod = typeof(GUILayout).GetMethods().FirstOrDefault(x => x.Name == "TextField" && x.GetParameters().Length == 2);

                TextField = Il2CppTextField;
            }
            else
            {
                TextField = MonoTextField;
            }
        }

        private static string MonoTextField(string text, GUILayoutOption[] options)
            => GUILayout.TextField(text, options);

        private static string Il2CppTextField(string text, GUILayoutOption[] options)
            => (string)textFieldMethod.Invoke(null, new object[] { text, options.ToIl2CppReferenceArray() });
    }
}

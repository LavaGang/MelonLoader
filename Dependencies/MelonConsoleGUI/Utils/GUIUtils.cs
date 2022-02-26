using UnityEngine;

namespace MelonLoader.Console
{
    internal static class GUIUtils
    {
        public static bool IsFocusedOnConsole => GUI.GetNameOfFocusedControl() == Module.ConsoleFieldName;

        public static bool OnKeyDown(KeyCode key)
        {
            var ev = Event.current;
            return ev.type == EventType.KeyDown && ev.keyCode == key;
        }

        // This only exists cuz of stripping. Thx unity
        public static GUIStyle CopyStyle(GUIStyle style)
            => new GUIStyle(style);
    }
}

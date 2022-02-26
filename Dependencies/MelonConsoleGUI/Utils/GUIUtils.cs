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
    }
}

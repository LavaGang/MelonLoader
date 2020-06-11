using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MelonLoader.Support
{
    class RenderHelper : ModSettingsMenu.RenderHelper
    {
        public object new_Rect(float x, float y, float width, float height) => new Rect(x, y, width, height);
        public object GUI_Window(int id, object clientRect, Action<int> func, string title) => GUI.Window(id, (Rect)clientRect, func, new GUIContent(title), GUI.skin.window);
        public bool GUI_Button(object position, string text) => GUI.Button((Rect)position, text);
        public bool Input_GetKeyDown(int key) => Input.GetKeyDown((KeyCode)key);
    }

    class RenderHelperGUIFix
    {
        internal static GUIContent[] GUIContent_Temp(string[] texts)
        {
            GUIContent[] gUIContent = new GUIContent[(int)texts.Length];
            for (int i = 0; i < (int)texts.Length; i++)
                gUIContent[i] = new GUIContent(texts[i]);
            return gUIContent;
        }
    }
}

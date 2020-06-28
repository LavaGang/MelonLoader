using System;
using System.Collections.Generic;

namespace MelonLoader.ModSettingsMenu
{
    public interface RenderHelper
    {
        object new_Rect(float x, float y, float width, float height);
        object new_Vector2(float x, float y);
        float Rect_get_width(object rect);
        float Rect_get_height(object rect);
        bool Input_GetKeyDown(int key);
        object GUI_Window(int id, object clientRect, Action<int> func, string title);
        bool GUI_Button(object position, string text);
        object GUI_BeginScrollView(object position, object scrollPosition, object viewRect);
        void GUI_EndScrollView(bool handleScrollWheel = false);
        void GUILayout_Label(string text);
        bool GUILayout_Toggle(bool value, string text);
    }

    class Main
    {
        private static RenderHelper renderHelper = null;
        private static bool IsOpen = false;
        private static object WindowRect = null;
        private static object ToolbarRect = null;
        private static object ScrollViewRect = null;
        private static object ScrollViewVector = null;
        private static object ScrollViewViewRect = null;

        private static ToolbarPage CurrentPage = ToolbarPage.SETTINGS;
        private static string[] PageNames = { "ABOUT", "SETTINGS" };
        private enum ToolbarPage
        {
            ABOUT = 0,
            SETTINGS = 1,
        };

        internal static void Setup()
        {
            if (SupportModule.supportModule == null)
                throw new NotSupportedException("Support module must be initialized before Rendering Mod Settings");

            renderHelper = SupportModule.supportModule.GetModSettingsMenuRenderHelper();
            if (renderHelper == null)
                return;

            WindowRect = renderHelper.new_Rect(10, 10, 400, 400);
            ToolbarRect = renderHelper.new_Rect(6, 22, 388, 25);
            ScrollViewRect = renderHelper.new_Rect(6, 52, (renderHelper.Rect_get_width(WindowRect) - 12), (renderHelper.Rect_get_height(WindowRect) - 58));
            ScrollViewVector = renderHelper.new_Vector2(0, 0);
            ScrollViewViewRect = renderHelper.new_Rect(0, 0, renderHelper.Rect_get_width(ScrollViewRect), renderHelper.Rect_get_height(ScrollViewRect));
        }

        internal static void Render()
        {
            if ((SupportModule.supportModule == null) || (renderHelper == null))
                return;

            InputCheck();
            if (!IsOpen)
                return;

            WindowRect = renderHelper.GUI_Window(0, WindowRect, new Action<int>(x =>
            {
                for (int i = 0; i < PageNames.Length; i++)
                {
                    string page = PageNames[i];

                    // Draw Buttons
                }

                switch(CurrentPage)
                {
                    case ToolbarPage.ABOUT:
                        Render_About();
                        break;
                    case ToolbarPage.SETTINGS:
                        Render_Settings();
                        break;
                    default:
                        break;
                }
            }), "MelonLoader");
        }

        private static void Render_About()
        {

        }

        private static void Render_Settings()
        {
            ScrollViewVector = renderHelper.GUI_BeginScrollView(ScrollViewRect, ScrollViewVector, ScrollViewViewRect);
            Dictionary<string, Dictionary<string, ModPrefs.PrefDesc>> prefs = ModPrefs.GetPrefs();
            if (prefs.Count > 0)
            {
                /*
                for (int i = 0; i < prefs.Count; i++)
                {
                    string key = prefs.Keys.ElementAt(i);
                    string category_name = ModPrefs.GetCategoryDisplayName(key);
                    renderHelper.GUILayout_Label(category_name);

                    Dictionary<string, ModPrefs.PrefDesc> category = prefs.Values.ElementAt(i);
                    if (category.Count > 0)
                    {
                        for (int k = 0; k < category.Count; k++)
                        {
                            string prefkey = category.Keys.ElementAt(k);
                            ModPrefs.PrefDesc pref = category.Values.ElementAt(k);
                            string pref_name = pref.DisplayText;
                            if (pref.Type == ModPrefs.PrefType.BOOL)
                            {
                                if (bool.TryParse(pref.Value, out bool value))
                                    ModPrefs.SetBool(key, prefkey, renderHelper.GUILayout_Toggle(value, pref.DisplayText));
                            }
                        }
                    }
                }
                */
            }
            renderHelper.GUI_EndScrollView(true);
            renderHelper.GUILayout_Label("SETTINGS");
        }

        private static bool IsInputPressed = false;
        private static void InputCheck()
        {
            if (renderHelper.Input_GetKeyDown(283)) // F2
            {
                if (!IsInputPressed)
                {
                    IsInputPressed = true;
                    IsOpen = !IsOpen;
                }
            }
            else
                IsInputPressed = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

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
            if (renderHelper == null)
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

            // Render Settings

            renderHelper.GUI_EndScrollView(true);
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

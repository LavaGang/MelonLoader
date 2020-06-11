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
        object GUI_Window(int id, object clientRect, Action<int> func, string title);
        bool GUI_Button(object position, string text);
        bool Input_GetKeyDown(int key);
    }

    class Main
    {
        internal static RenderHelper renderHelper = null;
        private static bool IsOpen = false;
        private static object WindowRect = null;
        internal static object ToolbarRect = null;

        internal static Main.ToolbarPage CurrentPage = 0;
        private static string[] PageNames = { "ABOUT", "SETTINGS" };
        internal enum ToolbarPage
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
                // Render Toolbar Here

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

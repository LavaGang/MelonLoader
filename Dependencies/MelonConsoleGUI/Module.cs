using MelonLoader.Modules;
using MelonLoader.Wrappers;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Console
{
    internal class Module : MelonModule
    {
        private const float ConsoleFieldHeight = 40f;
        public const string ConsoleFieldName = "ConsoleText";

        private static Color consoleColor = Color.black;
        private static Color consoleTextColor = Color.white;
        private static Color selectionColor = new Color32(111, 234, 98, 120);

        public static KeyCode toggleKey = KeyCode.BackQuote;
        public static char toggleChar = '`';
        public static KeyCode sendKey = KeyCode.Return;

        private static bool enabled;

        private GUIStyle consoleStyle;
        private static bool justEnabled;
        private string consoleText = string.Empty;

        public static Module instance;

        public static MelonLogger.Instance Logger => instance.LoggerInstance;

        public override void OnInitialize()
        {
            instance = this;

            LoggerInstance.Msg("Initializing...");

            MelonEvents.OnGUI.Subscribe(OnGUI);

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            MelonConsole.RegisterCommand(new MelonCommand("exit", "Closes the application.", new LemonAction(Application.Quit)));
        }

        private void OnGUI()
        {
            if (GUIUtils.OnKeyDown(toggleKey))
            {
                Toggle();
            }

            if (enabled)
                DrawConsole();
        }

        private void DrawConsole()
        {
            try
            {
                if (Event.current.character == toggleChar)
                    return;

                if (GUIUtils.OnKeyDown(sendKey) && GUIUtils.IsFocusedOnConsole)
                {
                    ExecuteCommand();
                }

                if (consoleStyle == null || consoleStyle.normal.background == null) // Textures become null sometimes for no reason
                {
                    LoggerInstance.Msg($"Reloading the console style");

                    consoleStyle = GUIUtils.CopyStyle(GUI.skin.textField);

                    consoleStyle.richText = false;
                    consoleStyle.fontSize = (int)(ConsoleFieldHeight * 0.7f);

                    var normal = UnityUtils.CreateColorTexture(consoleColor.ChangeAlpha(0.5f));

                    consoleStyle.normal.background = normal;
                    consoleStyle.normal.textColor = consoleTextColor;

                    consoleStyle.focused.background = UnityUtils.CreateColorTexture(consoleColor.ChangeAlpha(0.7f));
                    consoleStyle.focused.textColor = consoleTextColor;

                    consoleStyle.hover.background = normal;
                    consoleStyle.hover.textColor = consoleTextColor;

                    consoleStyle.active.background = UnityUtils.CreateColorTexture(consoleColor.ChangeAlpha(0.7f));
                    consoleStyle.active.textColor = consoleTextColor;
                }

                var screenHeight = Screen.height;
                GUI.SetNextControlName(ConsoleFieldName);
                GUI.skin.settings.selectionColor = selectionColor;
                consoleText = GUI.TextField(new Rect(0f, screenHeight - ConsoleFieldHeight, Screen.width, ConsoleFieldHeight), consoleText, consoleStyle ?? GUI.skin.textField);
            }
            catch (Exception ex)
            {
                if (!(ex is NotSupportedException || ex is MissingMethodException))
                    throw;

                LoggerInstance.Warning("Some references have been stripped, disabling the GUI console.");
                MelonEvents.OnGUI.Unsubscribe(typeof(Module).GetMethod(nameof(OnGUI), BindingFlags.NonPublic | BindingFlags.Instance));
                return;
            }

            if (justEnabled)
            {
                justEnabled = false;

                GUI.FocusControl(ConsoleFieldName);
            }

            if (!GUIUtils.IsFocusedOnConsole)
            {
                Toggle();
            }
        }

        public static void Toggle()
        {
            enabled = !enabled;
            if (enabled)
            {
                justEnabled = true;
            }
        }

        public void ExecuteCommand()
        {
            var cmd = consoleText;
            consoleText = string.Empty;

            MelonConsole.ExecuteCommand(cmd);
        }
    }
}

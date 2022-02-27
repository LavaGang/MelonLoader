using MelonLoader.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Console
{
    internal class Module : MelonModule
    {
        private const float ConsoleFieldHeight = 30f;
        public const string ConsoleFieldName = "ConsoleText";

        private static Color consoleColor = Color.black;
        private static Color consoleTextColor = Color.white;
        private static Color selectionColor = new Color32(111, 234, 98, 120);

        public static KeyCode toggleKey = KeyCode.BackQuote;
        public static char toggleChar = '`';
        public static KeyCode sendKey = KeyCode.Return;
        public static KeyCode previousKey = KeyCode.UpArrow;
        public static KeyCode nextKey = KeyCode.DownArrow;

        private static bool enabled;

        private GUIStyle consoleStyle;
        private string consoleText = string.Empty;

        private readonly List<string> history = new List<string>();
        private int historyIndex = -1;

        public static Module instance;

        public static MelonLogger.Instance Logger => instance.LoggerInstance;

        public override void OnInitialize()
        {
            instance = this;

            LoggerInstance.Msg("Initializing...");

            MelonEvents.OnGUI.Subscribe(OnGUI, 10000);

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

                if (GUIUtils.OnKeyDown(sendKey))
                    ExecuteCommand();
                if (GUIUtils.OnKeyDown(previousKey))
                    Previous();
                if (GUIUtils.OnKeyDown(nextKey))
                    Next();

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

                GUI.Label(new Rect(15, 15, 100, 50), $"<b><color=#79ed68>Melon</color><color=#f03f6d>Loader</color> <color=grey>v{BuildInfo.Version}</color></b>");

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

            if (historyIndex >= 0 && history[historyIndex] != consoleText)
                historyIndex = -1;

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
                GUI.FocusControl(ConsoleFieldName);
            }
        }

        private void Previous()
        {
            if (historyIndex >= history.Count - 1)
                return;

            historyIndex++;
            consoleText = history[historyIndex];
        }

        private void Next()
        {
            if (historyIndex <= 0)
                return;

            historyIndex--;
            consoleText = history[historyIndex];
        }

        public void ExecuteCommand()
        {
            var cmd = consoleText;
            consoleText = string.Empty;

            if (cmd != string.Empty)
                history.Insert(0, cmd);
            MelonConsole.ExecuteCommand(cmd);
        }
    }
}

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

        private static Color consoleTextColor = Color.white;
        private static Color selectionColor = new Color32(111, 234, 98, 120);
        private static Color panelColor = new Color32(0, 0, 0, 200);

        public static KeyCode toggleKey = KeyCode.BackQuote;
        public static char toggleChar = '`';
        public static KeyCode sendKey = KeyCode.Return;
        public static KeyCode previousKey = KeyCode.UpArrow;
        public static KeyCode nextKey = KeyCode.DownArrow;

        private static bool enabled;

        private Texture2D panelTexture;
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
            var onToggle = GUIUtils.OnKeyDown(toggleKey);
            if (onToggle)
                enabled = !enabled;

            if (enabled)
            {
                Draw();

                if (onToggle)
                    GUI.FocusControl(ConsoleFieldName);
                else if (!GUIUtils.IsFocusedOnConsole)
                    enabled = false;
            }
        }

        private void Draw()
        {
            try
            {
                if (Event.current.character == toggleChar)
                    Event.current.character = default;

                if (GUIUtils.OnKeyDown(sendKey))
                    ExecuteCommand();
                if (GUIUtils.OnKeyDown(previousKey))
                {
                    Previous();
                    Event.current.keyCode = default;
                }
                if (GUIUtils.OnKeyDown(nextKey))
                {
                    Next();
                    Event.current.keyCode = default;
                }

                if (consoleStyle == null || consoleStyle.normal.background == null) // Textures become null sometimes for no reason
                {
                    LoggerInstance.Msg($"Reloading the console style");

                    consoleStyle = GUIUtils.CopyStyle(GUI.skin.textField);

                    consoleStyle.richText = false;
                    consoleStyle.fontSize = (int)(ConsoleFieldHeight * 0.7f);

                    panelTexture = UnityUtils.CreateColorTexture(panelColor);

                    consoleStyle.normal.background = panelTexture;
                    consoleStyle.normal.textColor = consoleTextColor;

                    consoleStyle.focused.background = panelTexture;
                    consoleStyle.focused.textColor = consoleTextColor;

                    consoleStyle.hover.background = panelTexture;
                    consoleStyle.hover.textColor = consoleTextColor;

                    consoleStyle.active.background = panelTexture;
                    consoleStyle.active.textColor = consoleTextColor;
                }

                DrawMelonLoaderPanel();

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
        }

        private void DrawMelonLoaderPanel()
        {
            // I'm not using GUILayout here cuz all those calculations aren't needed (and cuz i'm too lazy to set it up)

            var x = 20;
            var y = 20;
            GUI.DrawTexture(new Rect(x, y, 340, 240), panelTexture);

            x += 10;
            y += 4;
            GUI.Label(new Rect(x, y, 300, 400), $"<b><size=20><color=#79ed68>Melon</color><color=#f03f6d>Loader</color>\n<color=grey>v{BuildInfo.Version}</color></size>\n\n\n" +
                $"<size=18>You have discovered the MelonConsole!</size>\n\n" +
                $"<size=14>Use the '<i>commands</i>' command to log all the available commands.\n\n" +
                $"Use the '<i>help <command></i>' command to log all info about a specific command.</size></b>");
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

using MelonLoader.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MelonLoader
{
    public static class MelonConsole
    {
        private static readonly List<MelonCommand> commands = new List<MelonCommand>();
        private static readonly MelonModule.Info guiModuleInfo = new MelonModule.Info($"MelonLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}MelonConsoleGUI.dll");

        public static MelonModule GUIModule { get; private set; }

        internal static void Load()
        {
            GUIModule = MelonModule.Load(guiModuleInfo);
            if (GUIModule == null)
            {
                MelonLogger.Msg("MelonConsole GUI module not loaded.");
                return;
            }
        }

        public static void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            var splitCommands = command.Contains(';') ? command.Split(';') : new string[] { command };
            foreach (var cmd in splitCommands)
            {
                List<string> filteredCommand = new List<string>();
                var idx = 0;
                var inQuotes = false;
                for (var a = 0; a < cmd.Length; a++)
                {
                    var c = cmd[a];
                    var isQuote = c == '"';

                    if ((!inQuotes && c == ' ') || isQuote)
                    {
                        if (a != idx)
                            filteredCommand.Add(cmd.Substring(idx, a - idx));
                        else if (inQuotes)
                            filteredCommand.Add(string.Empty);

                        idx = a + 1;
                    }

                    if (isQuote)
                    {
                        inQuotes = !inQuotes;
                    }
                }

                if (cmd.Length != idx)
                    filteredCommand.Add(cmd.Substring(idx, cmd.Length - idx));

                ProcessFilteredCommand(filteredCommand);
            }
        }

        private static void ProcessFilteredCommand(List<string> splitCmd)
        {
            if (splitCmd.Count == 0)
                return;

            var cmdName = splitCmd[0];
            splitCmd.RemoveAt(0);
            var cmd = FindCommand(cmdName);
            if (cmd == null)
            {
                MelonLogger.Error($"Unknown command: '{cmdName}'");
                return;
            }

            cmd.Execute(splitCmd.ToArray());
        }


        #region Command Functions

        public static MelonCommand FindCommand(string name)
            => commands.Find(x => x.name == name.ToLower());

        public static bool RegisterCommand(MelonCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command), "You cannot register an empty command...");

            if (FindCommand(command.name) != null)
                return false;

            commands.Add(command);
            return true;
        }

        public static bool UnregisterCommand(string name)
        {
            // This should be faster than List`1::RemoveAll
            var idx = commands.FindIndex(x => x.name == name.ToLower());
            if (idx == -1)
                return false;

            commands.RemoveAt(idx);
            return true;
        }

        #endregion

        #region Old MelonConsole Members
        [Obsolete("MelonLoader.MelonConsole.SetTitle is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.SetConsoleTitle instead.")]
        public static void SetTitle(string title) => MelonUtils.SetConsoleTitle(title);
        #endregion
    }
}

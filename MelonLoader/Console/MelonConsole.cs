using MelonLoader.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MelonLoader
{
    public static class MelonConsole
    {
        private static readonly MelonLogger.Instance logger = new MelonLogger.Instance("MelonConsole", ConsoleColor.Green);
        private static readonly List<MelonCommand> commands = new List<MelonCommand>();
        private static readonly MelonModule.Info guiModuleInfo = new MelonModule.Info($"MelonLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}MelonConsoleGUI.dll");

        public static MelonModule GUIModule { get; private set; }

        internal static void Load()
        {
            GUIModule = MelonModule.Load(guiModuleInfo);
            if (GUIModule == null)
            {
                logger.Msg("MelonConsole GUI module not loaded.");
            }

            AddDefaultCommands();
        }

        public static void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            var lastSplitIdx = 0;
            while (command.Length > lastSplitIdx)
            {
                List<string> filteredCommand = new List<string>();
                var inQuotes = false;
                var a = lastSplitIdx;
                for (; a < command.Length; a++)
                {
                    var c = command[a];
                    var isQuote = c == '"';
                    var isSplit = c == ';';

                    if ((!inQuotes && (c == ' ' || isSplit)) || isQuote)
                    {
                        if (a != lastSplitIdx)
                            filteredCommand.Add(command.Substring(lastSplitIdx, a - lastSplitIdx));
                        else if (inQuotes)
                            filteredCommand.Add(string.Empty);

                        lastSplitIdx = a + 1;

                        if (isSplit)
                        {
                            a++;
                            break;
                        }
                    }

                    if (isQuote)
                        inQuotes = !inQuotes;
                }

                if (a != lastSplitIdx)
                    filteredCommand.Add(command.Substring(lastSplitIdx, a - lastSplitIdx));

                lastSplitIdx = a;

                ProcessFilteredCommand(filteredCommand);
            }
        }

        private static void ProcessFilteredCommand(List<string> splitCmd)
        {
            if (splitCmd.Count == 0)
                return;

            logger.Msg(string.Join(" ", splitCmd.ToArray()));

            var cmdName = splitCmd[0];
            splitCmd.RemoveAt(0);
            var cmd = FindCommand(cmdName);
            if (cmd == null)
            {
                logger.Error($"Unknown command: '{cmdName}'");
                return;
            }

            cmd.Execute(splitCmd.ToArray());
        }

        private static void AddDefaultCommands()
        {
            RegisterCommand(new MelonCommand("commands", "Shows a list of all registered commands.", new LemonAction(LogCommands)));
            RegisterCommand(new MelonCommand("help", "Describes a command.", new LemonAction<string>(LogCommandUsage), new MelonCommand.Parameter("command", typeof(string))));
        }

        private static void LogCommandUsage(string commandName)
        {
            var command = FindCommand(commandName);
            if (command == null)
            {
                logger.Error($"Unknown command: '{commandName}'");
                return;
            }

            var args = string.Join(" ", command.parameters.Select(x => $"<{x.name}>").ToArray());
            logger.Msg($"Usage: '{command.name}{(args.Length == 0 ? "" : $" {args}")}'");
            logger.Msg($"Description: '{command.description}'");
        }

        private static void LogCommands()
        {
            logger.Msg($"All available commands:\n{string.Join(", ", commands.Select(x => x.name).ToArray())}");
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

using System;
using System.Collections.Generic;

namespace MelonLoader.Utils
{
    public static class MelonLaunchOptions
    {
        private static Dictionary<string, Action> WithoutArg = new Dictionary<string, Action>();
        private static Dictionary<string, Action<string>> WithArg = new Dictionary<string, Action<string>>();
        private static string[] _cmd;

        /// <summary>
        /// Dictionary of all Arguments with value (if found) that were not used by MelonLoader
        /// <para>
        /// <b>Key</b> is the argument, <b>Value</b> is the value for the argument, <c>null</c> if not found
        /// </para>
        /// </summary>
        public static Dictionary<string, string> ExternalArguments { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> InternalArguments { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Array of All Command Line Arguments
        /// </summary>
        public static string[] CommandLineArgs
        {
            get
            {
                if (_cmd == null)
                    _cmd = Environment.GetCommandLineArgs();
                return _cmd;
            }
        }

        internal static void Load()
        {
            string[] args = CommandLineArgs;
            int maxLen = args.Length;
            for (int i = 1; i < maxLen; i++)
            {
                string fullcmd = args[i];
                if (string.IsNullOrEmpty(fullcmd))
                    continue;

                // Parse Prefix
                string noPrefixCmd = fullcmd;
                if (noPrefixCmd.StartsWith("--"))
                    noPrefixCmd = noPrefixCmd.Remove(0, 2);
                else if (noPrefixCmd.StartsWith("-"))
                    noPrefixCmd = noPrefixCmd.Remove(0, 1);
                else
                {
                    // Unknown Command, Add it to Dictionary
                    ExternalArguments.Add(noPrefixCmd, null);
                    continue;
                }

                // Parse Argumentless Commands
                if (WithoutArg.TryGetValue(noPrefixCmd, out Action withoutArgFunc))
                {
                    InternalArguments.Add(noPrefixCmd, null);
                    withoutArgFunc();
                    continue;
                }

                // Parse Argument
                string cmdArg = null;
                if (noPrefixCmd.Contains("="))
                {
                    string[] split = noPrefixCmd.Split('=');
                    noPrefixCmd = split[0];
                    cmdArg = split[1];
                }

                if (string.IsNullOrEmpty(cmdArg)
                        && i + 1 >= maxLen
                    || string.IsNullOrEmpty(cmdArg)
                    || cmdArg.StartsWith("--")
                    || cmdArg.StartsWith("-"))
                {
                    // Unknown Command, Add it to Dictionary
                    ExternalArguments.Add(noPrefixCmd, null);
                    continue;
                }

                // Parse Argument Commands
                if (WithArg.TryGetValue(noPrefixCmd, out Action<string> withArgFunc))
                {
                    InternalArguments.Add(noPrefixCmd, cmdArg);
                    withArgFunc(cmdArg);
                    continue;
                }

                // Unknown Command with Argument, Add it to Dictionary
                ExternalArguments.Add(noPrefixCmd, cmdArg);
            }
        }
    }
}

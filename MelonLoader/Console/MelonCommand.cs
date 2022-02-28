using System;

namespace MelonLoader
{
    public class MelonCommand
    {
        public readonly string name;
        public readonly string description;
        public readonly Parameter[] parameters;
        private readonly Delegate action;

        public MelonCommand(string name, string description, Delegate action, params Parameter[] parameters)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "A command name cannot be null or empty.");

            this.name = name.ToLower();
            this.description = description;
            this.action = action ?? throw new ArgumentNullException(nameof(action), "A command action cannot be null.");
            this.parameters = parameters;
        }

        public void Execute(string[] arguments)
        {
            if (arguments.Length < parameters.Length)
            {
                MelonLogger.Error($"Failed to execute command '{name}': The amount of given arguments ({arguments.Length}) does not match the amount of parameters ({parameters.Length}).");
                return;
            }

            var newArgs = new object[parameters.Length];
            for (var a = 0; a < parameters.Length; a++)
            {
                var param = parameters[a];
                var rawArg = arguments[a];

                var arg = ConvertObj(rawArg, param.type);

                if (arg == null)
                {
                    MelonLogger.Error($"Failed to execute command '{name}': Could not convert argument '{param.name}' to '{param.type.FullName}'.");
                    return;
                }

                newArgs[a] = arg;
            }

            action.DynamicInvoke(newArgs);
        }

        private static object ConvertObj(string arg, Type type)
        {
            if (type == typeof(string))
                return arg;

            try { return Convert.ChangeType(arg, type); }
            catch { }
            return null;
        }

        public class Parameter
        {
            public readonly string name;
            public readonly string description;
            public readonly Type type;

            public Parameter(string name, Type type, string description = "")
            {
                this.name = name;
                this.description = description ?? string.Empty;
                this.type = type;
            }
        }
    }
}

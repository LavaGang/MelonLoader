using System;
using System.Reflection;

namespace MelonLoader
{
    /// <summary>
    /// An info class for broken Melons.
    /// </summary>
    public sealed class RottenMelon
    {
        public readonly Assembly assembly;
        public readonly Type type;
        public readonly string errorMessage;
        public readonly string exception;

        public RottenMelon(MelonAssembly assembly, string errorMessage)
            : this(assembly.Assembly, errorMessage, string.Empty) { }
        public RottenMelon(MelonAssembly assembly, string errorMessage, string exception)
            : this(assembly.Assembly, errorMessage, exception) { }
        public RottenMelon(MelonAssembly assembly, string errorMessage, Exception exception)
            : this(assembly.Assembly, errorMessage, ((exception != null) ? exception.ToString() : null)) { }

        public RottenMelon(Type type, string errorMessage)
            : this(type.Assembly, errorMessage, string.Empty)
            => this.type = type;
        public RottenMelon(Type type, string errorMessage, string exception)
            : this(type.Assembly, errorMessage, exception)
            => this.type = type;
        public RottenMelon(Type type, string errorMessage, Exception exception)
            : this(type.Assembly, errorMessage, ((exception != null) ? exception.ToString() : null))
            => this.type = type;

        public RottenMelon(Assembly assembly, string errorMessage)
            : this(assembly, errorMessage, string.Empty) { }
        public RottenMelon(Assembly assembly, string errorMessage, Exception exception)
            : this(assembly, errorMessage, ((exception != null) ? exception.ToString() : null)) { }
        public RottenMelon(Assembly assembly, string errorMessage, string exception)
        {
            this.assembly = assembly;
            this.errorMessage = errorMessage;
            this.exception = exception;
        }
    }
}

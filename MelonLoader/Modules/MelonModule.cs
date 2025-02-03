using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.Modules
{
    /// <summary>
    /// A base for external MelonLoader modules.
    /// </summary>
    public abstract class MelonModule
    {
        public string Name { get; private set; }
        protected MelonLogger.Instance LoggerInstance { get; private set; }
        protected MelonModule() { }

        internal static T Load<T>(string filePath)
            where T : MelonModule
        {
            if (!File.Exists(filePath))
            {
                MelonDebug.Msg($"MelonModule '{filePath}' doesn't exist, ignoring.");
                return null;
            }

            Assembly asm;
            try
            {
#if NET6_0_OR_GREATER
                asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
#else
                asm = Assembly.LoadFrom(filePath);
#endif
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to load Assembly of MelonModule '{filePath}':\n{ex}");
                return null;
            }

            var name = asm.GetName().Name;

            var type = asm.GetTypes().FirstOrDefault(x => typeof(MelonModule).IsAssignableFrom(x));
            if (type == null)
            {
                MelonLogger.Warning($"Failed to load MelonModule '{filePath}': No type deriving from MelonModule found.");
                return null;
            }

            T obj;
            try
            {
                obj = (T)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to initialize MelonModule '{filePath}':\n{ex}");
                return null;
            }

            obj.Name = name;
            obj.LoggerInstance = new MelonLogger.Instance(name, Color.Magenta); // Magenta cool :)
            return obj;
        }
    }
}

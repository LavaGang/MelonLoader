using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader.Modules
{
    /// <summary>
    /// A base for external MelonLoader modules.
    /// </summary>
    public abstract class MelonModule
    {
        private Type moduleType;

        public string Name { get; private set; }
        public Assembly Assembly { get; private set; }
        public Info ModuleInfo { get; private set; }
        protected MelonLogger.Instance LoggerInstance { get; private set; }

        protected MelonModule() { }

        public virtual void OnInitialize() { }

        internal static MelonModule Load(Info moduleInfo)
        {
            if (!File.Exists(moduleInfo.fullPath))
            {
                MelonDebug.Msg($"MelonModule '{moduleInfo.fullPath}' doesn't exist, ignoring.");
                return null;
            }

            if (moduleInfo.shouldBeRemoved != null && moduleInfo.shouldBeRemoved())
            {
                MelonDebug.Msg($"Removing MelonModule '{moduleInfo.fullPath}'...");
                try
                {
                    File.Delete(moduleInfo.fullPath);
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"Failed to remove MelonModule '{moduleInfo.fullPath}':\n{ex}");
                }
                return null;
            }

            if (moduleInfo.shouldBeIgnored != null && moduleInfo.shouldBeIgnored())
            {
                MelonDebug.Msg($"Ignoring MelonModule '{moduleInfo.fullPath}'...");
                return null;
            }

            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(moduleInfo.fullPath);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to load Assembly of MelonModule '{moduleInfo.fullPath}':\n{ex}");
                return null;
            }

            var name = asm.GetName().Name;

            var type = asm.GetTypes().FirstOrDefault(x => typeof(MelonModule).IsAssignableFrom(x));
            if (type == null)
            {
                MelonLogger.Warning($"Failed to load MelonModule '{moduleInfo.fullPath}': No type deriving from MelonModule found.");
                return null;
            }

            MelonModule obj;
            try
            {
                obj = (MelonModule)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to initialize MelonModule '{moduleInfo.fullPath}':\n{ex}");
                return null;
            }

            obj.moduleType = type;
            obj.Name = name;
            obj.Assembly = asm;
            obj.ModuleInfo = moduleInfo;
            obj.LoggerInstance = new MelonLogger.Instance(name, ConsoleColor.Magenta); // Magenta cool :)

            try
            {
                obj.OnInitialize();
            }
            catch (Exception ex)
            {
                obj.LoggerInstance.Error($"Local initialization failed:\n{ex}");
                return null;
            }

            return obj;
        }

        public object SendMessage(string name, params object[] arguments)
        {
            var msg = moduleType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (msg == null)
                return null;

            return msg.Invoke(msg.IsStatic ? null : this, arguments);
        }

        public class Info
        {
            public readonly string fullPath;
            internal readonly Func<bool> shouldBeRemoved;
            internal readonly Func<bool> shouldBeIgnored;

            internal Info(string path, Func<bool> shouldBeIgnored = null, Func<bool> shouldBeRemoved = null)
            {
                fullPath = Path.GetFullPath(path);
                this.shouldBeRemoved = shouldBeRemoved;
                this.shouldBeIgnored = shouldBeIgnored;
            }
        }
    }
}

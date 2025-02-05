using MelonLoader.Utils;
using System;

namespace MelonLoader.Modules
{
    public abstract class MelonEngineModule : MelonModule
    {
        public abstract bool Validate();
        public abstract void Initialize();

        public void SetEngineInfo(string name, string version, string variant = null)
            => MelonEnvironment.SetEngineInfo(name, version, variant);
        public void SetApplicationInfo(string developer, string name, string version)
            => MelonEnvironment.SetApplicationInfo(developer, name, version);
        public void PrintAppInfo()
            => MelonEnvironment.PrintAppInfo();
        public virtual void Stage2()
            => ModuleInterop.Stage2();
        public virtual void Stage3(string supportModulePath)
            => ModuleInterop.Stage3(supportModulePath);
    }
}

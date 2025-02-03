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

        public virtual void Stage2()
        {
            try
            {
                Core.Stage2();
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to run Stage2 of MelonLoader");
                MelonLogger.Error(ex);
                throw new("Error at Stage2");
            }
        }

        public virtual void Stage3(string supportModulePath)
        {
            try
            {
                Core.Stage3(supportModulePath);
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to run Stage3 of MelonLoader");
                MelonLogger.Error(ex);
                throw new("Error at Stage3");
            }
        }
    }
}

﻿using MelonLoader.Modules;

namespace MelonLoader.Runtime.Il2Cpp
{
    public class Il2CppRuntimeInfo : MelonRuntimeInfo
    {
        #region Public Members

        public string[] TriggerMethods { get; private set; }

        #endregion

        #region Constructors

        public Il2CppRuntimeInfo(
            string libPath,
            string supportModulePath,
            string[] triggerMethods)
       {
            LibPath = libPath;
            SupportModulePath = supportModulePath;
            TriggerMethods = triggerMethods;
        }

        #endregion
    }
}

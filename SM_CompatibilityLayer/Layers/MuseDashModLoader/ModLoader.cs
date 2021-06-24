using System;
using System.Collections.Generic;
using System.Reflection;
using ModHelper;

namespace ModLoader
{
    public class ModLoader
    {
        [Obsolete("ModLoader.ModLoader.mods is Only Here for Compatibility Reasons. Please use MelonHandler.Mods instead.")]
        private static List<IMod> mods = new List<IMod>();

        [Obsolete("ModLoader.ModLoader.depends is Only Here for Compatibility Reasons. ")]
        private static Dictionary<string, Assembly> depends = new Dictionary<string, Assembly>();
    }
}
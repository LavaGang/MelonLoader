using System;
using System.Collections.Generic;
using System.Reflection;
using ModHelper;

namespace ModLoader
{
    public class ModLoader
    {
        [Obsolete("ModLoader.ModLoader.mods is Only Here for Compatibility Reasons. Please use MelonHandler.Mods instead.")]
        internal static List<IMod> mods = new List<IMod>();

        [Obsolete("ModLoader.ModLoader.depends is Only Here for Compatibility Reasons. ")]
        internal static Dictionary<string, Assembly> depends = new Dictionary<string, Assembly>();
    }
}
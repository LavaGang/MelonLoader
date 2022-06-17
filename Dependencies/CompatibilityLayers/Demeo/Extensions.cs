using Boardgame.Modding;
using System.Reflection;
using HarmonyLib;

namespace MelonLoader.CompatibilityLayers
{
    internal static class Extensions
    {
        private static FieldInfo name_field;
        private static MethodInfo name_get_method;
        private static MethodInfo name_set_method;
        internal static string GetName(this ModdingAPI.ModInformation info)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (name_get_method == null)
                    name_get_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "name").GetGetMethod();
                if (name_get_method != null)
                    return (string)name_get_method.Invoke(info, new object[0]);
            }
            else
            {
                if (name_field == null)
                    name_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "name");
                if (name_field != null)
                    return (string)name_field.GetValue(info);
            }
            return null;
        }
        internal static void SetName(this ModdingAPI.ModInformation info, string name)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (name_set_method == null)
                    name_set_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "name").GetSetMethod();
                if (name_set_method != null)
                    name_set_method.Invoke(info, new object[] { name });
            }
            else
            {
                if (name_field == null)
                    name_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "name");
                if (name_field != null)
                    name_field.SetValue(info, name);
            }
        }

        private static FieldInfo version_field;
        private static MethodInfo version_method;
        internal static string GetVersion(this ModdingAPI.ModInformation info)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (version_method == null)
                    version_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "version").GetGetMethod();
                if (version_method != null)
                    return (string)version_method.Invoke(info, new object[0]);
            }
            else
            {
                if (version_field == null)
                    version_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "version");
                if (version_field != null)
                    return (string)version_field.GetValue(info);
            }
            return null;
        }
        internal static void SetVersion(this ModdingAPI.ModInformation info, string version)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (version_method == null)
                    version_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "version").GetSetMethod();
                if (version_method != null)
                    version_method.Invoke(info, new object[] { version });
            }
            else
            {
                if (version_field == null)
                    version_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "version");
                if (version_field != null)
                    version_field.SetValue(info, version);
            }
        }

        private static FieldInfo author_field;
        private static MethodInfo author_method;
        internal static string GetAuthor(this ModdingAPI.ModInformation info)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (author_method == null)
                    author_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "author").GetGetMethod();
                if (author_method != null)
                    return (string)author_method.Invoke(info, new object[0]);
            }
            else
            {
                if (author_field == null)
                    author_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "author");
                if (author_field != null)
                    return (string)author_field.GetValue(info);
            }
            return null;
        }
        internal static void SetAuthor(this ModdingAPI.ModInformation info, string author)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (author_method == null)
                    author_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "author").GetSetMethod();
                if (author_method != null)
                    author_method.Invoke(info, new object[] { author });
            }
            else
            {
                if (author_field == null)
                    author_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "author");
                if (author_field != null)
                    author_field.SetValue(info, author);
            }
        }

        private static FieldInfo description_field;
        private static MethodInfo description_method;
        internal static void SetDescription(this ModdingAPI.ModInformation info, string description)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (description_method == null)
                    description_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "description").GetSetMethod();
                if (description_method != null)
                    description_method.Invoke(info, new object[] { description });
            }
            else
            {
                if (description_field == null)
                    description_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "description");
                if (description_field != null)
                    description_field.SetValue(info, description);
            }
        }

        private static FieldInfo isNetworkCompatible_field;
        private static MethodInfo isNetworkCompatible_method;
        internal static void SetIsNetworkCompatible(this ModdingAPI.ModInformation info, bool isNetworkCompatible)
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                if (isNetworkCompatible_method == null)
                    isNetworkCompatible_method = AccessTools.Property(typeof(ModdingAPI.ModInformation), "isNetworkCompatible").GetSetMethod();
                if (isNetworkCompatible_method != null)
                    isNetworkCompatible_method.Invoke(info, new object[] { isNetworkCompatible });
            }
            else
            {
                if (isNetworkCompatible_field == null)
                    isNetworkCompatible_field = AccessTools.Field(typeof(ModdingAPI.ModInformation), "isNetworkCompatible");
                if (isNetworkCompatible_field != null)
                    isNetworkCompatible_field.SetValue(info, isNetworkCompatible);
            }
        }
    }
}

using MelonLoader;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IllusionPlugin;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ModPrefs
{
    public static string GetString(string section, string name, string defaultValue = "", bool autoSave = false)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<string>(name);
        entry ??= category.CreateEntry(name, defaultValue);
        return entry.Value;
    }

    public static int GetInt(string section, string name, int defaultValue = 0, bool autoSave = false)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<int>(name);
        entry ??= category.CreateEntry(name, defaultValue);
        return entry.Value;
    }

    public static float GetFloat(string section, string name, float defaultValue = 0f, bool autoSave = false)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<float>(name);
        entry ??= category.CreateEntry(name, defaultValue);
        return entry.Value;
    }

    public static bool GetBool(string section, string name, bool defaultValue = false, bool autoSave = false)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<bool>(name);
        entry ??= category.CreateEntry(name, defaultValue);
        return entry.Value;
    }

    public static bool HasKey(string section, string name) => MelonPreferences.HasEntry(section, name);

    public static void SetFloat(string section, string name, float value)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<float>(name);
        entry ??= category.CreateEntry(name, value);
        entry.Value = value;
    }

    public static void SetInt(string section, string name, int value)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<int>(name);
        entry ??= category.CreateEntry(name, value);
        entry.Value = value;
    }

    public static void SetString(string section, string name, string value)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<string>(name);
        entry ??= category.CreateEntry(name, value);
        entry.Value = value;
    }

    public static void SetBool(string section, string name, bool value)
    {
        var category = MelonPreferences.GetCategory(section);
        category ??= MelonPreferences.CreateCategory(section);
        var entry = category.GetEntry<bool>(name);
        entry ??= category.CreateEntry(name, value);
        entry.Value = value;
    }
}
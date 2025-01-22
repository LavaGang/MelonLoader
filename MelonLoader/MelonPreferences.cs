using MelonLoader.Preferences;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Tomlet.Exceptions;

namespace MelonLoader;

public static class MelonPreferences
{
    public static readonly List<MelonPreferences_Category> Categories = [];
    public static readonly List<MelonPreferences_ReflectiveCategory> ReflectiveCategories = [];
    public static readonly TomlMapper Mapper = new();

    /// <summary>
    /// Occurs when a Preferences File has been loaded.
    /// <para>
    /// <see cref="string"/>: Path of the Preferences File.
    /// </para>
    /// </summary>
    public static readonly MelonEvent<string> OnPreferencesLoaded = new();

    /// <summary>
    /// Occurs when a Preferences File has been saved.
    /// <para>
    /// <see cref="string"/>: Path of the Preferences File.
    /// </para>
    /// </summary>
    public static readonly MelonEvent<string> OnPreferencesSaved = new();

    internal static List<Preferences.IO.File> PrefFiles = [];
    internal static Preferences.IO.File DefaultFile = null;

    static MelonPreferences() => DefaultFile = new Preferences.IO.File(
        Path.Combine(MelonEnvironment.UserDataDirectory, "MelonPreferences.cfg"),
        Path.Combine(MelonEnvironment.UserDataDirectory, "modprefs.ini"));

    public static void Load()
    {
        try
        {
            DefaultFile.LegacyLoad();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error while Loading Legacy Preferences from {DefaultFile.LegacyFilePath}: {ex}");
            DefaultFile.WasError = true;
        }

        try
        {
            DefaultFile.Load();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error while Loading Preferences from {DefaultFile.FilePath}: {ex}");
            DefaultFile.WasError = true;
        }

        if (PrefFiles.Count >= 0)
        {
            foreach (var file in PrefFiles)
            {
                try
                {
                    file.Load();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error while Loading Preferences from {file.FilePath}: {ex}");
                    file.WasError = true;
                }
            }
        }

        if (Categories.Count > 0)
        {
            foreach (var category in Categories)
            {
                if (category.Entries.Count <= 0)
                    continue;
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                if (currentFile.WasError)
                    continue;
                foreach (var entry in category.Entries)
                    currentFile.SetupEntryFromRawValue(entry);
            }
        }

        if (ReflectiveCategories.Count > 0)
        {
            foreach (var category in ReflectiveCategories)
            {
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                if (currentFile.WasError)
                    continue;
                if (currentFile.TryGetCategoryTable(category.Identifier) is not
                    { } table)
                {
                    category.LoadDefaults();
                    continue;
                }

                category.Load(table);

                OnPreferencesLoaded.Invoke(currentFile.FilePath);
            }
        }

        MelonLogger.Msg("Preferences Loaded!");
    }

    public static void Save()
    {
        if (Categories.Count > 0)
        {
            foreach (var category in Categories)
            {
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                foreach (var entry in category.Entries)
                    if (!(entry.DontSaveDefault && entry.GetValueAsString() == entry.GetDefaultValueAsString()) && entry.GetValueAsString() != null)
                        currentFile.InsertIntoDocument(category.Identifier, entry.Identifier, entry.Save());
            }
        }
        if (ReflectiveCategories.Count > 0)
        {
            foreach (var category in ReflectiveCategories)
            {
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                currentFile.document.PutValue(category.Identifier, category.Save());
            }
        }

        try
        {
            DefaultFile.Save();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error while Saving Preferences to {DefaultFile.FilePath}: {ex}");
            DefaultFile.WasError = true;
        }

        if (PrefFiles.Count >= 0)
        {
            foreach (var file in PrefFiles)
            {
                try
                {
                    file.Save();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error while Saving Preferences to {file.FilePath}: {ex}");
                    file.WasError = true;
                    continue;
                }
                OnPreferencesSaved.Invoke(file.FilePath);
            }
        }

        MelonLogger.Msg("Preferences Saved!");
    }

    public static MelonPreferences_Category CreateCategory(string identifier) => CreateCategory(identifier, null, false);
    public static MelonPreferences_Category CreateCategory(string identifier, string display_name = null) => CreateCategory(identifier, display_name, false);

    public static MelonPreferences_Category CreateCategory(string identifier, string display_name = null, bool is_hidden = false, bool should_save = true)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new Exception("identifier is null or empty when calling CreateCategory");
        display_name ??= identifier;
        var category = GetCategory(identifier);
        return category != null ? category : new MelonPreferences_Category(identifier, display_name, is_hidden);
    }

    public static MelonPreferences_ReflectiveCategory CreateCategory<T>(string identifier, string display_name = null) where T : new() => MelonPreferences_ReflectiveCategory.Create<T>(identifier, display_name);

    [Obsolete]
    public static MelonPreferences_Entry CreateEntry<T>(string category_identifier, string entry_identifier,
        T default_value, string display_name, bool is_hidden)
        => CreateEntry(category_identifier, entry_identifier, default_value, display_name, null, is_hidden, false, null);

    public static MelonPreferences_Entry<T> CreateEntry<T>(string category_identifier, string entry_identifier, T default_value,
        string display_name = null, string description = null, bool is_hidden = false, bool dont_save_default = false,
        ValueValidator validator = null)
    {
        if (string.IsNullOrEmpty(category_identifier))
            throw new Exception("category_identifier is null or empty when calling CreateEntry");

        if (string.IsNullOrEmpty(entry_identifier))
            throw new Exception("entry_identifier is null or empty when calling CreateEntry");

        var category = GetCategory(entry_identifier);
        category ??= CreateCategory(category_identifier);

        return category.CreateEntry(entry_identifier, default_value, display_name, description, is_hidden, dont_save_default, validator);
    }

    public static MelonPreferences_Category GetCategory(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new Exception("identifier is null or empty when calling GetCategory");
        return Categories.Count <= 0 ? null : Categories.Find(x => x.Identifier.Equals(identifier));
    }

    public static T GetCategory<T>(string identifier) where T : new()
    {
        if (string.IsNullOrEmpty(identifier))
            throw new Exception("identifier is null or empty when calling GetCategory");
        if (ReflectiveCategories.Count <= 0)
            return default;
        var category = ReflectiveCategories.Find(x => x.Identifier.Equals(identifier));
        return category != null ? category.GetValue<T>() : default;
    }

    public static void SaveCategory<T>(string identifier, bool printmsg = true)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new Exception("identifier is null or empty when calling GetCategory");
        if (ReflectiveCategories.Count <= 0)
            return;
        var category = ReflectiveCategories.Find(x => x.Identifier.Equals(identifier));
        category?.SaveToFile(printmsg);
    }

    public static MelonPreferences_Entry GetEntry(string category_identifier, string entry_identifier) => GetCategory(category_identifier)?.GetEntry(entry_identifier);
    public static MelonPreferences_Entry<T> GetEntry<T>(string category_identifier, string entry_identifier) => GetCategory(category_identifier)?.GetEntry<T>(entry_identifier);
    public static bool HasEntry(string category_identifier, string entry_identifier) => GetEntry(category_identifier, entry_identifier) != null;

    public static void SetEntryValue<T>(string category_identifier, string entry_identifier, T value)
    {
        var entry = GetCategory(category_identifier)?.GetEntry<T>(entry_identifier);
        if (entry != null)
            entry.Value = value;
    }

    public static T GetEntryValue<T>(string category_identifier, string entry_identifier)
    {
        var cat = GetCategory(category_identifier);
        if (cat == null)
            return default;
        var entry = cat.GetEntry<T>(entry_identifier);
        return entry == null ? default : entry.Value;
    }

    internal static Preferences.IO.File GetPrefFileFromFilePath(string filepath)
    {
        if (PrefFiles.Count <= 0)
            return null;
        var filepathinfo = new FileInfo(filepath);
        foreach (var file in PrefFiles)
        {
            var filepathinfo2 = new FileInfo(file.FilePath);
            if (filepathinfo.FullName.Equals(filepathinfo2.FullName))
                return file;
        }

        return null;
    }

    internal static bool IsFileInUse(Preferences.IO.File file)
    {
        if (Categories.Count <= 0)
            return false;
        file ??= DefaultFile;
        if (file == DefaultFile)
            return true;
        foreach (var category in Categories)
        {
            var currentFile = category.File;
            currentFile ??= DefaultFile;
            if (currentFile == file)
                return true;
        }

        foreach (var category in ReflectiveCategories)
        {
            var currentFile = category.File;
            currentFile ??= DefaultFile;
            if (currentFile == file)
                return true;
        }

        return false;
    }

    internal static void LoadFileAndRefreshCategories(Preferences.IO.File file, bool printmsg = true)
    {
        try
        {
            file.Load();
        }
        catch (TomlUnescapedUnicodeControlCharException ex)
        {
            MelonLogger.Error($"Error while Loading Preferences from {file.FilePath}: {ex}");
            return;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error while Loading Preferences from {file.FilePath}: {ex}");
            file.WasError = true;
            return;
        }

        if (Categories.Count > 0)
            foreach (var category in Categories)
            {
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                if ((currentFile != file) || (category.Entries.Count <= 0))
                    continue;
                foreach (var entry in category.Entries)
                    currentFile.SetupEntryFromRawValue(entry);
            }

        if (ReflectiveCategories.Count > 0)
            foreach (var category in ReflectiveCategories)
            {
                var currentFile = category.File;
                currentFile ??= DefaultFile;
                if (currentFile != file)
                    continue;
                if (file.TryGetCategoryTable(category.Identifier) is not
                    { } table)
                {
                    category.LoadDefaults();
                    continue;
                }
                category.Load(table);
            }

        if (printmsg)
            MelonLogger.MsgDirect($"MelonPreferences Loaded from {file.FilePath}");

        OnPreferencesLoaded.Invoke(file.FilePath);
    }

    public static void RemoveCategoryFromFile(string filePath, string categoryName)
    {
        var currentFile = GetPrefFileFromFilePath(filePath);
        if (currentFile == null)
            return;
        currentFile.RemoveCategoryFromDocument(categoryName);
    }

    internal static bool IsFilePathDefault(string filepath) => new FileInfo(filepath).FullName.Equals(new FileInfo(DefaultFile.FilePath).FullName);
}
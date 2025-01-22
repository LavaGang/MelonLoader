using System;
using System.Diagnostics.CodeAnalysis;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace MelonLoader.Preferences;

[SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's public API")]
public class MelonPreferences_ReflectiveCategory
{
    private readonly Type SystemType;
    private object value;
    internal IO.File File = null;

    public string Identifier { get; internal set; }
    public string DisplayName { get; internal set; }

    internal static MelonPreferences_ReflectiveCategory Create<T>(string categoryName, string displayName) => new(typeof(T), categoryName, displayName);

    private MelonPreferences_ReflectiveCategory(Type type, string categoryName, string displayName)
    {
        SystemType = type;
        Identifier = categoryName;
        DisplayName = displayName;

        var currentFile = File;
        currentFile ??= MelonPreferences.DefaultFile;
        if (currentFile.TryGetCategoryTable(Identifier) is not
            { } table)
            LoadDefaults();
        else
            Load(table);

        MelonPreferences.ReflectiveCategories.Add(this);
    }

    internal void LoadDefaults() => value = Activator.CreateInstance(SystemType);

    internal void Load(TomlValue tomlValue)
    {
        try
        {
            value = TomletMain.To(SystemType, tomlValue);
        }
        catch (TomlTypeMismatchException)
        {
            return;
        }
        catch (TomlNoSuchValueException)
        {
            return;
        }
        catch (TomlEnumParseException)
        {
            return;
        }
    }

    internal TomlValue Save()
    {
        if (value == null)
            LoadDefaults();

        return TomletMain.ValueFrom(SystemType, value);
    }

    public T GetValue<T>() where T : new()
    {
        if (typeof(T) != SystemType)
            return default;
        if (value == null)
            LoadDefaults();
        return (T)value;
    }

    public void SetFilePath(string filepath, bool autoload = true, bool printmsg = true)
    {
        if (File != null)
        {
            var oldfile = File;
            File = null;
            if (!MelonPreferences.IsFileInUse(oldfile))
            {
                oldfile.FileWatcher.Destroy();
                MelonPreferences.PrefFiles.Remove(oldfile);
            }
        }

        if (!string.IsNullOrEmpty(filepath) && !MelonPreferences.IsFilePathDefault(filepath))
        {
            File = MelonPreferences.GetPrefFileFromFilePath(filepath);
            if (File == null)
            {
                File = new IO.File(filepath);
                MelonPreferences.PrefFiles.Add(File);
            }
        }

        if (autoload)
            MelonPreferences.LoadFileAndRefreshCategories(File, printmsg);
    }

    public void ResetFilePath()
    {
        if (File == null)
            return;
        var oldfile = File;
        File = null;
        if (!MelonPreferences.IsFileInUse(oldfile))
        {
            oldfile.FileWatcher.Destroy();
            MelonPreferences.PrefFiles.Remove(oldfile);
        }

        MelonPreferences.LoadFileAndRefreshCategories(MelonPreferences.DefaultFile);
    }

    public void SaveToFile(bool printmsg = true)
    {
        var currentfile = File;
        currentfile ??= MelonPreferences.DefaultFile;

        currentfile.document.PutValue(Identifier, Save());
        try
        {
            currentfile.Save();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error while Saving Preferences to {currentfile.FilePath}: {ex}");
            currentfile.WasError = true;
        }

        if (printmsg)
            MelonLogger.Msg($"MelonPreferences Saved to {currentfile.FilePath}");

        MelonPreferences.OnPreferencesSaved.Invoke(currentfile.FilePath);
    }

    public void LoadFromFile(bool printmsg = true)
    {
        var currentfile = File;
        currentfile ??= MelonPreferences.DefaultFile;
        MelonPreferences.LoadFileAndRefreshCategories(currentfile, printmsg);
    }

    public void DestroyFileWatcher() => File?.FileWatcher.Destroy();
}
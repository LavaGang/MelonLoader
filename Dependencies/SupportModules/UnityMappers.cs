using Tomlet;
using Tomlet.Models;
using UnityEngine;

namespace MelonLoader.Support.Preferences;

internal static class UnityMappers
{
    internal static void RegisterMappers()
    {
        TomletMain.RegisterMapper(WriteColor, ReadColor);
        TomletMain.RegisterMapper(WriteColor32, ReadColor32);
        TomletMain.RegisterMapper(WriteVector2, ReadVector2);
        TomletMain.RegisterMapper(WriteVector3, ReadVector3);
        TomletMain.RegisterMapper(WriteVector4, ReadVector4);
        TomletMain.RegisterMapper(WriteQuaternion, ReadQuaternion);
    }

    private static Color ReadColor(TomlValue value)
    {
        var floats = MelonPreferences.Mapper.ReadArray<float>(value);
        return floats == null || floats.Length != 4
            ? default
            : new Color(floats[0] / 255f, floats[1] / 255f, floats[2] / 255f, floats[3] / 255f);
    }

    private static TomlValue WriteColor(Color value)
    {
        var floats = new[] { value.r * 255, value.g * 255, value.b * 255, value.a * 255 };
        return MelonPreferences.Mapper.WriteArray(floats);
    }

    private static Color32 ReadColor32(TomlValue value)
    {
        var bytes = MelonPreferences.Mapper.ReadArray<byte>(value);
        return bytes == null || bytes.Length != 4 ? default : new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
    }

    private static TomlValue WriteColor32(Color32 value)
    {
        var bytes = new[] { value.r, value.g, value.b, value.a };
        return MelonPreferences.Mapper.WriteArray(bytes);
    }

    private static Vector2 ReadVector2(TomlValue value)
    {
        var floats = MelonPreferences.Mapper.ReadArray<float>(value);
        return floats == null || floats.Length != 2 ? default : new Vector2(floats[0], floats[1]);
    }

    private static TomlValue WriteVector2(Vector2 value)
    {
        var floats = new[] { value.x, value.y };
        return MelonPreferences.Mapper.WriteArray(floats);
    }

    private static Vector3 ReadVector3(TomlValue value)
    {
        var floats = MelonPreferences.Mapper.ReadArray<float>(value);
        return floats == null || floats.Length != 3 ? default : new Vector3(floats[0], floats[1], floats[2]);
    }

    private static TomlValue WriteVector3(Vector3 value)
    {
        var floats = new[] { value.x, value.y, value.z };
        return MelonPreferences.Mapper.WriteArray(floats);
    }

    private static Vector4 ReadVector4(TomlValue value)
    {
        var floats = MelonPreferences.Mapper.ReadArray<float>(value);
        return floats == null || floats.Length != 4 ? default : new Vector4(floats[0], floats[1], floats[2], floats[3]);
    }

    private static TomlValue WriteVector4(Vector4 value)
    {
        var floats = new[] { value.x, value.y, value.z, value.w };
        return MelonPreferences.Mapper.WriteArray(floats);
    }

    private static Quaternion ReadQuaternion(TomlValue value)
    {
        var floats = MelonPreferences.Mapper.ReadArray<float>(value);
        return floats == null || floats.Length != 4 ? default : new Quaternion(floats[0], floats[1], floats[2], floats[3]);
    }

    private static TomlValue WriteQuaternion(Quaternion value)
    {
        var floats = new[] { value.x, value.y, value.z, value.w };
        return MelonPreferences.Mapper.WriteArray(floats);
    }
}
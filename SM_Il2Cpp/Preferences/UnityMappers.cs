using MelonLoader.Tomlyn.Model;
using UnityEngine;

namespace MelonLoader.Support.Preferences
{
    public static class UnityMappers
    {
        internal static void RegisterMappers()
        {
            MelonPreferences.Mapper.RegisterMapper(ReadColor, WriteColor);
            MelonPreferences.Mapper.RegisterMapper(ReadColor32, WriteColor32);
            MelonPreferences.Mapper.RegisterMapper(ReadVector2, WriteVector2);
            MelonPreferences.Mapper.RegisterMapper(ReadVector3, WriteVector3);
            MelonPreferences.Mapper.RegisterMapper(ReadVector4, WriteVector4);
            MelonPreferences.Mapper.RegisterMapper(ReadQuaternion, WriteQuaternion);
        }

        private static Color ReadColor(TomlObject value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Color(floats[0] / 255f, floats[1] / 255f, floats[2] / 255f, floats[3] / 255f);
        }

        private static TomlObject WriteColor(Color value)
        {
            float[] floats = new[] { value.r * 255, value.g * 255, value.b * 255, value.a * 255};
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Color32 ReadColor32(TomlObject value)
        {
            byte[] bytes = MelonPreferences.Mapper.ReadArray<byte>(value);
            if (bytes == null || bytes.Length != 4)
                return default;
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        private static TomlObject WriteColor32(Color32 value)
        {
            byte[] bytes = new[] { value.r, value.g, value.b, value.a };
            return MelonPreferences.Mapper.WriteArray(bytes);
        }

        private static Vector2 ReadVector2(TomlObject value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 2)
                return default;
            return new Vector2(floats[0], floats[1]);
        }

        private static TomlObject WriteVector2(Vector2 value)
        {
            float[] floats = new[] { value.x, value.y };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Vector3 ReadVector3(TomlObject value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 3)
                return default;
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        private static TomlObject WriteVector3(Vector3 value)
        {
            float[] floats = new[] { value.x, value.y, value.z };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Vector4 ReadVector4(TomlObject value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        private static TomlObject WriteVector4(Vector4 value)
        {
            float[] floats = new[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Quaternion ReadQuaternion(TomlObject value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
        }

        private static TomlObject WriteQuaternion(Quaternion value)
        {
            float[] floats = new[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
    }
}
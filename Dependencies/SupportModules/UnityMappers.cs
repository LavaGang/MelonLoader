using Tomlet;
using Tomlet.Models;
using UnityEngine;

namespace MelonLoader.Support.Preferences
{
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
            TomletMain.RegisterMapper(WriteRect, ReadRect);
            
#if SM_Il2Cpp
            TomletMain.RegisterMapper(WriteRectInt, ReadRectInt);
#endif
        }

        private static Color ReadColor(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Color(floats[0] / 255f, floats[1] / 255f, floats[2] / 255f, floats[3] / 255f);
        }

        private static TomlValue WriteColor(Color value)
        {
            float[] floats = new[] { value.r * 255, value.g * 255, value.b * 255, value.a * 255};
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Color32 ReadColor32(TomlValue value)
        {
            byte[] bytes = MelonPreferences.Mapper.ReadArray<byte>(value);
            if (bytes == null || bytes.Length != 4)
                return default;
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        private static TomlValue WriteColor32(Color32 value)
        {
            byte[] bytes = new[] { value.r, value.g, value.b, value.a };
            return MelonPreferences.Mapper.WriteArray(bytes);
        }

        private static Vector2 ReadVector2(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 2)
                return default;
            return new Vector2(floats[0], floats[1]);
        }

        private static TomlValue WriteVector2(Vector2 value)
        {
            float[] floats = new[] { value.x, value.y };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Vector3 ReadVector3(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 3)
                return default;
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        private static TomlValue WriteVector3(Vector3 value)
        {
            float[] floats = new[] { value.x, value.y, value.z };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Vector4 ReadVector4(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        private static TomlValue WriteVector4(Vector4 value)
        {
            float[] floats = new[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }

        private static Quaternion ReadQuaternion(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
        }

        private static TomlValue WriteQuaternion(Quaternion value)
        {
            float[] floats = new[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
        private static Rect ReadRect(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Rect(floats[0], floats[1], floats[2], floats[3]);
        }

        private static TomlValue WriteRect(Rect value)
        {
            float[] floats = new[] { value.x, value.y, value.width, value.height };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
#if SM_Il2Cpp
        private static RectInt ReadRectInt(TomlValue value)
        {
            int[] ints = MelonPreferences.Mapper.ReadArray<int>(value);
            if (ints == null || ints.Length != 4)
                return default;
            RectInt result = new RectInt();
            result.x = ints[0];
            result.y = ints[1];
            result.width = ints[2];
            result.height = ints[3];
            return result;
        }

        private static TomlValue WriteRectInt(RectInt value)
        {
            int[] ints = new[] { value.x, value.y, value.width, value.height };
            return MelonPreferences.Mapper.WriteArray(ints);
        }
#endif
    }
}
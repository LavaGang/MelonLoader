using MelonLoader.Tomlyn.Model;
using UnityEngine;

namespace MelonLoader.Support.Preferences
{
    public static class UnityMappers
    {
        public static void RegisterMappers()
        {
            MelonPreferences.Mapper.RegisterMapper(ReadColor, WriteColor);
            MelonPreferences.Mapper.RegisterMapper(ReadVector2, WriteVector2);
            MelonPreferences.Mapper.RegisterMapper(ReadVector3, WriteVector3);
            MelonPreferences.Mapper.RegisterMapper(ReadVector4, WriteVector4);
            MelonPreferences.Mapper.RegisterMapper(ReadQuaternion, WriteQuaternion);
        }

        public static Color ReadColor(TomlObject value)
        {
            var floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            
            return new Color(floats[0] / 255f, floats[1] / 255f, floats[2] / 255f, floats[3] / 255f);
        }

        public static TomlObject WriteColor(Color value)
        {
            var floats = new[] { value.r * 255, value.g * 255, value.b * 255, value.a * 255};
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
        public static Vector2 ReadVector2(TomlObject value)
        {
            var floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 2)
                return default;
            
            return new Vector2(floats[0], floats[1]);
        }

        public static TomlObject WriteVector2(Vector2 value)
        {
            var floats = new float[] { value.x, value.y };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
        public static Vector3 ReadVector3(TomlObject value)
        {
            var floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 3)
                return default;
            
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        public static TomlObject WriteVector3(Vector3 value)
        {
            var floats = new float[] { value.x, value.y, value.z };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
        public static Vector4 ReadVector4(TomlObject value)
        {
            var floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            
            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        public static TomlObject WriteVector4(Vector4 value)
        {
            var floats = new float[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
        
        public static Quaternion ReadQuaternion(TomlObject value)
        {
            var floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            
            return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
        }

        public static TomlObject WriteQuaternion(Quaternion value)
        {
            var floats = new float[] { value.x, value.y, value.z, value.w };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
    }
}
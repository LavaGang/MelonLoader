using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        internal List<MelonPreferences_Entry> prefstbl = new List<MelonPreferences_Entry>();
        public List<MelonPreferences_Entry> Prefs { get => prefstbl.AsReadOnly().ToList(); }
        internal MelonPreferences_Category(string name, string displayname)
        {
            Name = name;
            DisplayName = displayname;
            MelonPreferences.categorytbl.Add(this);
        }

        public MelonPreferences_Entry GetEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling GetEntry");
            if (prefstbl.Count <= 0)
                return null;
            return prefstbl.Find(x => x.Name.Equals(name));
        }
        public bool HasEntry(string name) => (GetEntry(name) != null);

        public MelonPreferences_Entry CreateEntry<T>(string name, T value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
            {
                entry = new MelonPreferences_Entry();
                entry.Setup(this, name, value, displayname, hidden);
                return entry;
            }
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            MelonPreferences_Entry.TypeEnum requestedType = MelonPreferences.TypeToTypeEnum<T>();
            if (requestedType != entry.Type)
            {
                switch (requestedType)
                {
                    case MelonPreferences_Entry.TypeEnum.STRING:
                        entry.DefaultValue_string = Expression.Lambda<Func<string>>(Expression.Convert(Expression.Constant(value), typeof(string))).Compile()();
                        break;
                    case MelonPreferences_Entry.TypeEnum.BOOL:
                        entry.DefaultValue_bool = Expression.Lambda<Func<bool>>(Expression.Convert(Expression.Constant(value), typeof(bool))).Compile()();
                        break;
                    case MelonPreferences_Entry.TypeEnum.INT:
                        entry.DefaultValue_int = Expression.Lambda<Func<int>>(Expression.Convert(Expression.Constant(value), typeof(int))).Compile()();
                        break;
                    case MelonPreferences_Entry.TypeEnum.FLOAT:
                        entry.DefaultValue_float = Expression.Lambda<Func<float>>(Expression.Convert(Expression.Constant(value), typeof(float))).Compile()();
                        break;
                    case MelonPreferences_Entry.TypeEnum.LONG:
                        entry.DefaultValue_long = Expression.Lambda<Func<long>>(Expression.Convert(Expression.Constant(value), typeof(long))).Compile()();
                        break;
                    case MelonPreferences_Entry.TypeEnum.DOUBLE:
                        entry.DefaultValue_double = Expression.Lambda<Func<double>>(Expression.Convert(Expression.Constant(value), typeof(double))).Compile()();
                        break;
                    default:
                        break;
                }
                entry.ConvertCurrentValueType(requestedType);
            }
            return entry;
        }
    }
}
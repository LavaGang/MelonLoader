using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace MelonLoader.TinyJSON;

public sealed class ProxyObject : Variant, IEnumerable<KeyValuePair<string, Variant>>
{
    public const string TypeHintKey = "@type";
    private readonly Dictionary<string, Variant> dict;

    public ProxyObject()
    {
        dict = [];
    }

    IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
    {
        return dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dict.GetEnumerator();
    }

    public void Add(string key, Variant item)
    {
        dict.Add(key, item);
    }

    public bool TryGetValue(string key, out Variant item)
    {
        return dict.TryGetValue(key, out item);
    }

    public string TypeHint
    {
        get
        {
            Variant item;
            return TryGetValue(TypeHintKey, out item) ? item.ToString(CultureInfo.InvariantCulture) : null;
        }
    }

    public override Variant this[string key]
    {
        get
        {
            return dict[key];
        }
        set
        {
            dict[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return dict.Count;
        }
    }

    public Dictionary<string, Variant>.KeyCollection Keys
    {
        get
        {
            return dict.Keys;
        }
    }

    // ReSharper disable once UnusedMember.Global
    public Dictionary<string, Variant>.ValueCollection Values
    {
        get
        {
            return dict.Values;
        }
    }
}

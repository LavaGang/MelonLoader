using System;

namespace MelonLoader.TinyJSON;

[Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
public sealed class ProxyBoolean : Variant
{
    private readonly bool value;

    public ProxyBoolean(bool value)
    {
        this.value = value;
    }

    public override bool ToBoolean(IFormatProvider provider)
    {
        return value;
    }

    public override string ToString(IFormatProvider provider)
    {
        return value ? "true" : "false";
    }
}

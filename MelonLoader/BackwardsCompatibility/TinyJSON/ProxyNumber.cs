using System;
using System.Globalization;

namespace MelonLoader.TinyJSON;

[Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
public sealed class ProxyNumber : Variant
{
    private static readonly char[] floatingPointCharacters = { '.', 'e' };
    private readonly IConvertible value;

    public ProxyNumber(IConvertible value)
    {
        this.value = value is string stringValue ? Parse(stringValue) : value;
    }

    private static IConvertible Parse(string value)
    {
        if (value.IndexOfAny(floatingPointCharacters) == -1)
        {
            if (value[0] == '-')
            {
                long parsedValue;
                if (long.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue))
                {
                    return parsedValue;
                }
            }
            else
            {
                ulong parsedValue;
                if (ulong.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue))
                {
                    return parsedValue;
                }
            }
        }

        decimal decimalValue;
        if (decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimalValue))
        {
            // Check for decimal underflow.
            if (decimalValue == decimal.Zero)
            {
                double parsedValue;
                if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue))
                {
                    if (Math.Abs(parsedValue) > double.Epsilon)
                    {
                        return parsedValue;
                    }
                }
            }

            return decimalValue;
        }

        double doubleValue;
        return double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue) ? doubleValue : (IConvertible)0;
    }

    public override bool ToBoolean(IFormatProvider provider)
    {
        return value.ToBoolean(provider);
    }

    public override byte ToByte(IFormatProvider provider)
    {
        return value.ToByte(provider);
    }

    public override char ToChar(IFormatProvider provider)
    {
        return value.ToChar(provider);
    }

    public override decimal ToDecimal(IFormatProvider provider)
    {
        return value.ToDecimal(provider);
    }

    public override double ToDouble(IFormatProvider provider)
    {
        return value.ToDouble(provider);
    }

    public override short ToInt16(IFormatProvider provider)
    {
        return value.ToInt16(provider);
    }

    public override int ToInt32(IFormatProvider provider)
    {
        return value.ToInt32(provider);
    }

    public override long ToInt64(IFormatProvider provider)
    {
        return value.ToInt64(provider);
    }

    public override sbyte ToSByte(IFormatProvider provider)
    {
        return value.ToSByte(provider);
    }

    public override float ToSingle(IFormatProvider provider)
    {
        return value.ToSingle(provider);
    }

    public override string ToString(IFormatProvider provider)
    {
        return value.ToString(provider);
    }

    public override ushort ToUInt16(IFormatProvider provider)
    {
        return value.ToUInt16(provider);
    }

    public override uint ToUInt32(IFormatProvider provider)
    {
        return value.ToUInt32(provider);
    }

    public override ulong ToUInt64(IFormatProvider provider)
    {
        return value.ToUInt64(provider);
    }
}

#if ANDROID
namespace MelonLoader.Java;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly struct JValue
{
    [FieldOffset(0)]
    public readonly byte Z;

    [FieldOffset(0)]
    public readonly sbyte B;

    [FieldOffset(0)]
    public readonly char C;

    [FieldOffset(0)]
    public readonly short S;

    [FieldOffset(0)]
    public readonly int I;

    [FieldOffset(0)]
    public readonly long J;

    [FieldOffset(0)]
    public readonly float F;

    [FieldOffset(0)]
    public readonly double D;

    [FieldOffset(0)]
    public readonly IntPtr L;

    public JValue(bool value)
    {
        this = new JValue();
        Z = Convert.ToByte(value);
    }

    public JValue(sbyte value)
    {
        this = new JValue();
        B = value;
    }

    public JValue(char value)
    {
        this = new JValue();
        C = value;
    }

    public JValue(short value)
    {
        this = new JValue();
        S = value;
    }

    public JValue(int value)
    {
        this = new JValue();
        I = value;
    }

    public JValue(long value)
    {
        this = new JValue();
        J = value;
    }

    public JValue(float value)
    {
        this = new JValue();
        F = value;
    }

    public JValue(double value)
    {
        this = new JValue();
        D = value;
    }

    public JValue(IntPtr value)
    {
        this = new JValue();
        L = value;
    }

    public JValue(JObject obj)
    {
        this = new JValue();
        this.L = obj.Handle;
    }

    public JValue(object value)
    {
        this = new JValue();
        switch (value)
        {
            case bool z:
                this.Z = Convert.ToByte(z);
                break;

            case sbyte b:
                this.B = b;
                break;

            case char c:
                this.C = c;
                break;

            case short s:
                this.S = s;
                break;

            case int i:
                this.I = i;
                break;

            case long j:
                this.J = j;
                break;

            case float f:
                this.F = f;
                break;

            case double d:
                this.D = d;
                break;

            case JObject obj:
                this.L = obj.Handle;
                break;
        }
    }

    public static implicit operator JValue(bool b) => new JValue(b);

    public static implicit operator JValue(sbyte b) => new JValue(b);

    public static implicit operator JValue(char c) => new JValue(c);

    public static implicit operator JValue(short s) => new JValue(s);

    public static implicit operator JValue(int i) => new JValue(i);

    public static implicit operator JValue(long j) => new JValue(j);

    public static implicit operator JValue(float f) => new JValue(f);

    public static implicit operator JValue(double d) => new JValue(d);

    public static implicit operator JValue(JObject obj) => new JValue(obj);
}
#endif

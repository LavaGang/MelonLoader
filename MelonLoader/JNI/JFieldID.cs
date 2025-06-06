#if ANDROID
using System;

namespace MelonLoader.Java;

public readonly struct JFieldID : IEquatable<JFieldID>
{
    public readonly IntPtr Handle { get; }

    internal JFieldID(IntPtr handle)
    {
        this.Handle = handle;
    }

    public static implicit operator IntPtr(JFieldID fieldID) => fieldID.Handle;

    public static implicit operator JFieldID(IntPtr pointer) => new(pointer);

    public bool Equals(JFieldID other)
    {
        return this.Handle == other.Handle;
    }
}
#endif

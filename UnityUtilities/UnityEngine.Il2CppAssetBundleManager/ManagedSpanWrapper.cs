using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime.InteropTypes;
using System.Runtime.InteropServices;

namespace UnityEngine;

// New struct needed for Unity 6 function calls.
[StructLayout(LayoutKind.Sequential)]
[InjectedType]
public partial struct ManagedSpanWrapper
{
    public Pointer<Il2CppSystem.Void> begin;
    public Il2CppSystem.Int32 length;
}

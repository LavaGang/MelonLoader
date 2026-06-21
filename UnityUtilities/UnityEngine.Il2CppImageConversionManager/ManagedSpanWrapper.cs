using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime.InteropTypes;
using System.Runtime.InteropServices;

namespace UnityEngine;

// Implemented class/struct needed for unmarshalling of data in Unity 6.
// New struct needed for Unity 6 function calls.
[StructLayout(LayoutKind.Sequential)]
[InjectedType]
internal partial struct ManagedSpanWrapper
{
    public Pointer<Il2CppSystem.Void> begin;
    public Il2CppSystem.Int32 length;
}

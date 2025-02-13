using MelonLoader.Bootstrap.Logging;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate void NativeHookFn(nint* target, nint detour);

[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogMsgFn(ColorRGB* msgColor, string msg, int msgLength, ColorRGB* sectionColor, string section, int sectionLength);

[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogErrorFn(string msg, int msgLength, string section, int sectionLength, bool warning);

[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogMelonInfoFn(ColorRGB* nameColor, string name, int nameLength, string info, int infoLength);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate nint PtrRetFn();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate nint CastManagedAssemblyPtrFn(nint ptr);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void ActionFn();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
[return: MarshalAs(UnmanagedType.U1)]
internal delegate bool BoolRetFn();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void GetLoaderConfigFn(ref LoaderConfig config);

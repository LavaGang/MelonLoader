using MelonLoader.Bootstrap.Logging;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate void NativeHookFn(nint* target, nint detour);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate void LogMsgFn(ColorRGB* msgColor, char* msg, int msgLength, ColorRGB* sectionColor, char* section, int sectionLength);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate void LogErrorFn(char* msg, int msgLength, char* section, int sectionLength, bool warning);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate void LogMelonInfoFn(ColorRGB* nameColor, char* name, int nameLength, char* info, int infoLength);

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

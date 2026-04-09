using MelonLoader.Logging;

using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal unsafe delegate void NativeHookFn(nint* target, nint detour);

[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogMsgFn(ColorARGB* msgColor, string msg, int msgLength, ColorARGB* sectionColor, string section, int sectionLength, string strippedMSg, int strippedMsgLength);

[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogErrorFn(string msg, int msgLength, string section, int sectionLength, bool warning);

[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
internal unsafe delegate void LogMelonInfoFn(ColorARGB* nameColor, string name, int nameLength, string info, int infoLength);

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate nint PtrRetFn();

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate nint CastManagedAssemblyPtrFn(nint ptr);

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate void ActionFn();

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
[return: MarshalAs(UnmanagedType.U1)]
internal delegate bool BoolRetFn();

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate void GetLoaderConfigFn(ref LoaderConfig config);
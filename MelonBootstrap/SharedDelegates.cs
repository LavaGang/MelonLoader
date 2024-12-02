using MelonLoader.Bootstrap.Logging;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void NativeHookFn(nint* target, nint detour);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void LogMsgFn(ColorRGB* msgColor, char* msg, int msgLength, ColorRGB* sectionColor, char* section, int sectionLength);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void LogErrorFn(char* msg, int msgLength, char* section, int sectionLength);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void LogMelonInfoFn(ColorRGB* nameColor, char* name, int nameLength, char* info, int infoLength);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate nint PtrRetFn();

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate nint CastManagedAssemblyPtrFn(nint ptr);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void ActionFn();
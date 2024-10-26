#if NET6_0_OR_GREATER
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap;

internal unsafe delegate void NativeHookFn(nint* target, nint detour);
internal unsafe delegate void LogMsgFn(ColorRGB* msgColor, char* msg, int msgLength, ColorRGB* sectionColor, char* section, int sectionLength);
internal unsafe delegate void LogErrorFn(char* msg, int msgLength, char* section, int sectionLength);
internal unsafe delegate void LogMelonInfoFn(ColorRGB* nameColor, char* name, int nameLength, char* info, int infoLength);
#endif
using System;
using System.Text;
#pragma warning disable 0649

namespace UnhollowerMini
{
    internal class Il2CppException : Exception
    {
        [ThreadStatic] private static byte[] ourMessageBytes;

        public static Func<IntPtr, string> ParseMessageHook;

        public Il2CppException(IntPtr exception) : base(BuildMessage(exception)) { }

        private static unsafe string BuildMessage(IntPtr exception)
        {
            if (ParseMessageHook != null) return ParseMessageHook(exception);
            if (ourMessageBytes == null) ourMessageBytes = new byte[65536];
            fixed (byte* message = ourMessageBytes)
                UnityInternals.format_exception(exception, message, ourMessageBytes.Length);
            string builtMessage = Encoding.UTF8.GetString(ourMessageBytes, 0, Array.IndexOf(ourMessageBytes, (byte)0));
            fixed (byte* message = ourMessageBytes)
                UnityInternals.format_stack_trace(exception, message, ourMessageBytes.Length);
            builtMessage +=
                "\n" + Encoding.UTF8.GetString(ourMessageBytes, 0, Array.IndexOf(ourMessageBytes, (byte)0));
            return builtMessage;
        }

        public static void RaiseExceptionIfNecessary(IntPtr returnedException)
        {
            if (returnedException == IntPtr.Zero) return;
            throw new Il2CppException(returnedException);
        }
    }
}

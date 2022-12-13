using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Utils
{
    internal static unsafe class ManagedAnalyticsBlocker
    {
        private const int WSA_TRYAGAIN = 11002;

        [DllImport("ws2_32", ExactSpelling = true)]
        private static extern void WSASetLastError(int error);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate hostent* gethostbyname_delegate(/* const char* */ byte* name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int getaddrinfo_delegate(byte* pNodeName, byte* pServiceName, addrinfo* pHints, addrinfo** ppResult);

        private static IntPtr wsock32;
        private static IntPtr ws2_32;

        private static gethostbyname_delegate original_gethostbyname;
        private static getaddrinfo_delegate original_getaddrinfo;

        private static List<string> _blockList = new()
        {
            "\u0068\u0061\u0074\u0065\u0062\u0069\u006e",
            "\u006f\u0063\u0075\u006c\u0075\u0073\u0063\u0064\u006e\u002e\u0063\u006f\u006d",
            "\u0070\u0061\u0073\u0074\u0065\u0062\u0069\u006e",
            "\u0061\u006e\u0061\u006c\u0079\u0074\u0069\u0063\u0073\u002e\u006e\u0069\u006e\u006a\u0061\u006b\u0069\u0077\u0069\u002e\u0063\u006f\u006d",
            "\u0066\u0061\u0063\u0065\u0062\u006f\u006f\u006b\u002d\u0068\u0061\u0072\u0064\u0077\u0061\u0072\u0065\u002e\u0063\u006f\u006d",
            "\u0067\u0061\u006d\u0065\u0061\u006e\u0061\u006c\u0079\u0074\u0069\u0063\u0073\u002e\u0063\u006f\u006d",
            "\u0066\u0062\u0063\u0064\u006e\u002e\u0063\u006f\u006d",
            "\u0069\u0063\u0065\u0062\u0075\u0072\u006e\u002e\u0078\u0079\u007a",
            "\u0066\u0062\u0073\u0062\u0078\u002e\u0063\u006f\u006d",
            "\u0066\u0062\u002e\u0063\u006f\u006d",
            "\u0067\u006f\u006f\u0067\u006c\u0065\u002d\u0061\u006e\u0061\u006c\u0079\u0074\u0069\u0063\u0073\u002e\u0063\u006f\u006d",
            "\u0070\u0075\u0062\u006c\u0069\u0063\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0070\u0065\u0072\u0066\u002d\u0065\u0076\u0065\u006e\u0074\u0073\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u006d\u0065\u0061\u0070\u002e\u0067\u0067",
            "\u0064\u0072\u006f\u0070\u0062\u006f\u0078",
            "\u0063\u006f\u006e\u0066\u0069\u0067\u002e\u0075\u0063\u0061\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0061\u006d\u0070\u006c\u0069\u0074\u0075\u0064\u0065\u002e\u0063\u006f\u006d",
            "\u0066\u0062\u0063\u0064\u006e\u002e\u006e\u0065\u0074",
            "\u0065\u0063\u006f\u006d\u006d\u0065\u0072\u0063\u0065\u002e\u0069\u0061\u0070\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0064\u0061\u0074\u0061\u002d\u006f\u0070\u0074\u006f\u0075\u0074\u002d\u0073\u0065\u0072\u0076\u0069\u0063\u0065\u002e\u0075\u0063\u0061\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0076\u0072\u006d\u006f\u0064\u0073\u002e\u0073\u0070\u0061\u0063\u0065",
            "\u0066\u0062\u002e\u006d\u0065",
            "\u0070\u0069\u0078\u0065\u006c\u0073\u0074\u0072\u0069\u006b\u0065\u0033\u0064\u0061\u0077\u0073\u002e\u0063\u006f\u006d",
            "\u0069\u006e\u0067\u0065\u0073\u0074\u002e\u0073\u0065\u006e\u0074\u0072\u0079\u002e\u0069\u006f",
            "\u0064\u0069\u0073\u0063\u006f\u0072\u0064",
            "\u0067\u006c\u0075\u0065\u0068\u0065\u006e\u0064\u0065\u0072\u002d\u0061\u006c\u0075\u0068\u0075\u0074\u002e\u0064\u0065",
            "\u0068\u0061\u0073\u0074\u0065\u0062\u0069\u006e",
            "\u0063\u0072\u0061\u0073\u0068\u006c\u0079\u0074\u0069\u0063\u0073\u002e\u0063\u006f\u006d",
            "\u0061\u0070\u0069\u002e\u0075\u0063\u0061\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0066\u0061\u0063\u0065\u0062\u006f\u006f\u006b\u002e\u006e\u0065\u0074",
            "\u0073\u006b\u0069\u0064\u002d\u0068\u0075\u0062",
            "\u0063\u0064\u0070\u002e\u0063\u006c\u006f\u0075\u0064\u002e\u0075\u006e\u0069\u0074\u0079\u0033\u0064\u002e\u0063\u006f\u006d",
            "\u0072\u0069\u0070\u0070\u0065\u0072\u002e\u0073\u0074\u006f\u0072\u0065",
            "\u0061\u007a\u0075\u0072\u0065\u0077\u0065\u0062\u0073\u0069\u0074\u0065\u0073\u002e\u006e\u0065\u0074",
            "\u0069\u0074\u0065\u006f\u002e\u0073\u0070\u0061\u0063\u0065",
            "\u0073\u006f\u0066\u0074\u006c\u0069\u0067\u0068\u0074\u002e\u0061\u0074\u002e\u0075\u0061",
            "\u0066\u0061\u0063\u0065\u0062\u006f\u006f\u006b\u002e\u0063\u006f\u006d",
        };

        private static List<string> _observedHostnames = new()
        {
            //Default ignored (as in, not logged) hostnames. I'm leaving these in cleartext cause it's easier.
            "ntp.org",
            "bonetome.com",
            "samboy.dev",
            "github.com",
            "ruby-core.com",
            "melonloader.com",
            "githubusercontent.com",
            "thetrueyoshifan.com"
        };

        private static bool CheckShouldBlock(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return false;

            hostname = hostname.Trim().ToLowerInvariant();

            var shouldBlock = _blockList.Any(b => hostname.Contains(b));

            if (MelonDebug.IsEnabled() || MelonLaunchOptions.Core.ShouldDisplayAnalyticsBlocker)
            {
                if (shouldBlock)
                    MelonDebug.Msg($"Host Name or IP blocked: {hostname}");
                else if (!_observedHostnames.Any(h => hostname.Contains(h)))
                {
                    MelonDebug.Msg($"Unique Host Name or IP Found: {hostname}");
                    _observedHostnames.Add(hostname);
                }
            }

            return shouldBlock;
        }

        public static void Install()
        {
            //Need to hook wsock32 gethostbyname
            //And, if on x64, ws2_32 getaddrinfo
            
            //TODO: is this doable on Unix?
            if (MelonUtils.IsMac || MelonUtils.IsUnix || MelonUtils.IsUnderWineOrSteamProton())
                return;
            
            MelonDebug.Msg("Initializing Analytics Blocker...");

            wsock32 = NativeLibrary.LoadLib("wsock32");

            var ghbnPtr = wsock32.GetNativeLibraryExport("gethostbyname");

#if NET6_0
            delegate* unmanaged[Cdecl]<byte*, hostent*> detourPtr = &gethostbyname_hook;
#else
            var detourPtr = Marshal.GetFunctionPointerForDelegate((gethostbyname_delegate)gethostbyname_hook);
#endif

            MelonDebug.Msg($"Hooking wsock32::gethostbyname (0x{ghbnPtr.ToInt64():X})...");
            MelonUtils.NativeHookAttachDirect((IntPtr) (&ghbnPtr), (IntPtr) detourPtr);

            original_gethostbyname = (gethostbyname_delegate)Marshal.GetDelegateForFunctionPointer(ghbnPtr, typeof(gethostbyname_delegate));

            if (MelonUtils.IsGame32Bit())
            {
                ws2_32 = IntPtr.Zero;
            } else 
            {
                ws2_32 = NativeLibrary.LoadLib("ws2_32");

                MelonDebug.Msg("Hooking ws2_32...");

                var gaiPtr = ws2_32.GetNativeLibraryExport("getaddrinfo");

#if NET6_0
                delegate* unmanaged[Cdecl]<byte*, byte*, addrinfo*, addrinfo**, int> detourPtr2 = &getaddrinfo_hook;
#else
                var detourPtr2 = Marshal.GetFunctionPointerForDelegate((getaddrinfo_delegate)getaddrinfo_hook);
#endif
                MelonUtils.NativeHookAttachDirect((IntPtr) (&gaiPtr), (IntPtr)detourPtr2);

                original_getaddrinfo = (getaddrinfo_delegate)Marshal.GetDelegateForFunctionPointer(gaiPtr, typeof(getaddrinfo_delegate));
            }
        }

#if NET6_0
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static hostent* gethostbyname_hook(byte* name)
        {
            if (name == null || (IntPtr) name == IntPtr.Zero)
                return original_gethostbyname(name);

            var hostname = Marshal.PtrToStringAnsi((IntPtr)name);

            var shouldBlock = CheckShouldBlock(hostname);
            if (shouldBlock)
                name = (byte*) Marshal.StringToHGlobalAnsi("localhost");

            hostent* ret;
            try
            {
                ret = original_gethostbyname(name);
            } catch
            {
                WSASetLastError(WSA_TRYAGAIN);
                ret = (hostent*) IntPtr.Zero;
            }

            if (shouldBlock)
                Marshal.FreeHGlobal((IntPtr)name);

            return ret;
        }

#if NET6_0
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static int getaddrinfo_hook(byte* pNodeName, byte* pServiceName, addrinfo* pHints, addrinfo** ppResult)
        {
            if (pNodeName == null || (IntPtr) pNodeName == IntPtr.Zero)
                return original_getaddrinfo(pNodeName, pServiceName, pHints, ppResult);

            var hostname = Marshal.PtrToStringAnsi((IntPtr)pNodeName);

            var shouldBlock = CheckShouldBlock(hostname);
            if (shouldBlock)
                pNodeName = (byte*)Marshal.StringToHGlobalAnsi("localhost");

            int ret;
            try
            {
                ret = original_getaddrinfo(pNodeName, pServiceName, pHints, ppResult);
            }
            catch
            {
                WSASetLastError(WSA_TRYAGAIN);
                ret = WSA_TRYAGAIN;
            }

            if (shouldBlock)
                Marshal.FreeHGlobal((IntPtr)pNodeName);

            return ret;
        }

#pragma warning disable CS0649 //Field XXX is never assigned to
        private struct hostent
        {
            public byte* h_name; //char*
            public byte** h_aliases; //char**
            public short h_addrtype;
            public short h_length;
            public byte** h_addr_list; //char**
        }

        private struct sockaddr
        {
            public ushort sa_family;
            public fixed char sa_data[14];
        }

        private struct addrinfo
        {
            public int ai_flags;
            public int ai_family;
            public int ai_socktype;
            public int ai_protocol;
            public void* ai_addrlen; //size_t
            public byte* ai_canonname; //char*
            public sockaddr* ai_addr;
            public addrinfo* ai_next;
        }
#pragma warning restore
    }
}
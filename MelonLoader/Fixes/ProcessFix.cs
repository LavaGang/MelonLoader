using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class ProcessFix
	{
		private static MainWindowFinder mainWindowFinder;

		internal static void Install()
		{
			mainWindowFinder = new MainWindowFinder();

			Type processFixType = typeof(ProcessFix);
			Type processType = typeof(Process);

			try
			{
				Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(processType, "MainWindowHandle"), AccessTools.Method(processFixType, "get_MainWindowHandle").ToNewHarmonyMethod());
				Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(processType, "MainWindowTitle"), AccessTools.Method(processFixType, "get_MainWindowTitle").ToNewHarmonyMethod());
			}
			catch (Exception ex) { MelonLogger.Warning($"ProcessFix Exception: {ex}"); }
		}

		// Taken and Modified from .NET Framework's System.dll
		private static bool get_MainWindowHandle(Process __instance, ref IntPtr __result)
        {
			__result = mainWindowFinder.FindMainWindow(__instance.Id);
			return false;
        }

		// Taken and Modified from .NET Framework's System.dll
		private static bool get_MainWindowTitle(Process __instance, ref string __result)
		{
			IntPtr intPtr = __instance.MainWindowHandle;
			if (intPtr == IntPtr.Zero)
				__result = string.Empty;
			else
			{
				int capacity = GetWindowTextLength(new HandleRef(__instance, intPtr)) * 2;
				StringBuilder stringBuilder = new StringBuilder(capacity);
				GetWindowText(new HandleRef(__instance, intPtr), stringBuilder, stringBuilder.Capacity);
				__result = stringBuilder.ToString();
			}
			return false;
		}

		// Taken and Modified from .NET Framework's System.dll
		[DllImport("user32.dll", BestFitMapping = true, CharSet = CharSet.Auto)]
		private static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

		// Taken and Modified from .NET Framework's System.dll
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetWindowTextLength(HandleRef hWnd);

		// Taken and Modified from .NET Framework's System.dll
		internal class MainWindowFinder
        {
            private IntPtr Handle;
            private int ID;

            private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
            {
                int num;
                GetWindowThreadProcessId(new HandleRef(this, handle), out num);
                if (num != ID || !IsMainWindow(handle))
                    return true;
                Handle = handle;
                return false;
            }

			internal IntPtr FindMainWindow(int processId)
            {
                Handle = (IntPtr)0;
                ID = processId;
                EnumThreadWindowsCallback enumThreadWindowsCallback = new EnumThreadWindowsCallback(EnumWindowsCallback);
                EnumWindows(enumThreadWindowsCallback, IntPtr.Zero);
                GC.KeepAlive(enumThreadWindowsCallback);
                return Handle;
            }

            private bool IsMainWindow(IntPtr handle)
                => (!(GetWindow(new HandleRef(this, handle), 4) != (IntPtr)0) && IsWindowVisible(new HandleRef(this, handle)));

			private delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
			private static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
			private static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			private static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			private static extern bool IsWindowVisible(HandleRef hWnd);
		}
    }
}
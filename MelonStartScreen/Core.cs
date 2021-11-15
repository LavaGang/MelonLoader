using MelonLoader.MelonStartScreen.NativeUtils;
using MelonLoader.MelonStartScreen.NativeUtils.PEParser;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnhollowerMini;
using Windows;

namespace MelonLoader.MelonStartScreen
{
    internal static class Core
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr User32SetTimerDelegate(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        private static bool initialized = false;

        private static User32SetTimerDelegate user32SetTimerOriginal;
        private static bool nextSetTimerIsUnity = false;
        private static IntPtr titleBarTimer;

        private static bool functionRunDone = false;
        private static int functionRunResult = 0;


        private static int LoadAndRun(LemonFunc<int> functionToWaitForAsync)
        {
            // We try to resolve all the signatures, which are available for Unity 2018.1.0+
            // If we can't find them (signatures changed or <2018.1.0), then we run the function and return.
            try
            {
                if (!NativeSignatureResolver.Apply())
                    return functionToWaitForAsync();

                if (!ApplyUser32SetTimerPatch())
                    return functionToWaitForAsync();

                MelonDebug.Msg("Initializing Screen Renderer");
                ScreenRenderer.Init();
                MelonDebug.Msg("Screen Renderer initialized");

                RegisterMessageCallbacks();

                // Initial render
                ScreenRenderer.Render();
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                return functionToWaitForAsync();
            }

            initialized = true;

            StartFunction(functionToWaitForAsync);
            MainLoop();

            return functionRunResult;
        }

        private static void RegisterMessageCallbacks()
        {
            MelonLogger.MsgCallbackHandler += (namesection_color, txt_color, namesection, txt) => ScreenRenderer.UpdateProgressFromLog(txt);
            MelonDebug.MsgCallbackHandler += (txt_color, txt) => ScreenRenderer.UpdateProgressFromLog(txt);
        }

        #region User32::SetTime Path

        private static unsafe bool ApplyUser32SetTimerPatch()
        {
            IntPtr original = PEUtils.GetExportedFunctionPointerForModule("USER32.dll", "SetTimer");
            MelonLogger.Msg($"User32::SetTimer original: 0x{(long)original:X}");

            if (original == IntPtr.Zero)
            {
                MelonLogger.Error("Failed to find USER32.dll::SetTimer");
                return false;
            }

            // We get a native function pointer to User32SetTimerDetour from our current class
            //IntPtr detourPtr = typeof(Core).GetMethod("User32SetTimerDetour", BindingFlags.NonPublic | BindingFlags.Static).MethodHandle.GetFunctionPointer();
            IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate((User32SetTimerDelegate)User32SetTimerDetour);

            if (detourPtr == IntPtr.Zero)
            {
                MelonLogger.Error("Failed to find User32SetTimerDetour");
                return false;
            }

            // And we patch SetTimer to replace it by our hook
            MelonLogger.Msg($"Applying USER32.dll::SetTimer Hook at 0x{original.ToInt64():X}");
            MelonUtils.NativeHookAttach((IntPtr)(&original), detourPtr);
            MelonLogger.Msg($"Creating delegate for original USER32.dll::SetTimer (0x{original.ToInt64():X})");
            user32SetTimerOriginal = (User32SetTimerDelegate)Marshal.GetDelegateForFunctionPointer(original, typeof(User32SetTimerDelegate));
            MelonLogger.Msg("Applied USER32.dll::SetTimer patch");

            return true;
        }

        private unsafe static IntPtr User32SetTimerDetour(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr timerProc)
        {
            if (nextSetTimerIsUnity)
            {
                nextSetTimerIsUnity = false;
                return IntPtr.Zero;
            }

            return user32SetTimerOriginal(hWnd, nIDEvent, uElapse, timerProc);
        }

        #endregion

        private static void StartFunction(LemonFunc<int> func)
        {
            new Thread(() =>
            {
                functionRunResult = func();
                functionRunDone = true;
            })
            {
                IsBackground = true,
                Name = "MelonStartScreen Function Thread"
            }.Start();
        }



        private static void MainLoop()
        {
            while (!functionRunDone) // WM_QUIT
            {
                ProcessEventsAndRender(true);
            }

            if (titleBarTimer != IntPtr.Zero)
            {
                User32.KillTimer(IntPtr.Zero, titleBarTimer);
                titleBarTimer = IntPtr.Zero;
            }
        }

        #region Event Processing and Rendering

        private static void ProcessEventsAndRender(bool isMainLoop = false)
        {
            ProcessMessages();

            if (titleBarTimer != IntPtr.Zero)
            {
                User32.KillTimer(IntPtr.Zero, titleBarTimer);
                titleBarTimer = IntPtr.Zero;
            }
            ScreenRenderer.Render();
            if (isMainLoop)
                Thread.Sleep(16); // ~60fps
            else
            {
                if (titleBarTimer != IntPtr.Zero)
                {
                    User32.KillTimer(IntPtr.Zero, titleBarTimer);
                    titleBarTimer = IntPtr.Zero;
                }
            }
        }

        private static void ProcessMessages()
        {
            while (true)
            {
                User32.PeekMessage(out Msg msg, IntPtr.Zero, 0, 0, 0);
                if (msg.message == WindowMessage.QUIT)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                else if (User32.PeekMessage(out msg, IntPtr.Zero, 0, 0, 1)) // If there a message pending
                {
                    if (msg.message == WindowMessage.NCLBUTTONDOWN || msg.message == (WindowMessage)0x242 /* NCPOINTERDOWN */)
                    {
                        if (titleBarTimer == IntPtr.Zero)
                            titleBarTimer = User32.SetTimer(IntPtr.Zero, IntPtr.Zero, 10, TitleBarTimerUpdateCallback);
                        nextSetTimerIsUnity = true;
                    }
                    else if (msg.message == WindowMessage.PAINT)
                        ScreenRenderer.Render();

                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
                else
                    return;
            }
        }

        private static void TitleBarTimerUpdateCallback(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime)
        {
            ScreenRenderer.Render();
        }

        #endregion

        #region Calls from MelonLoader

        internal static void OnApplicationStart_Plugins()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressState(ModLoadStep.OnApplicationStart_Plugins);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationStart_Plugin(string name)
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressFromMod(name);
            ProcessEventsAndRender();
        }

        internal static void LoadingMods()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressState(ModLoadStep.LoadMods);
            ProcessEventsAndRender();
        }

        internal static void DisplayModLoadIssuesIfNeeded()
        {
            // TODO Start a new locking thread with a display of the issues and buttons to either close the game or continue
        }

        internal static void OnApplicationStart_Mods()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressState(ModLoadStep.OnApplicationStart_Mods);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationStart_Mod(string name)
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressFromMod(name);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationLateStart_Plugins()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressState(ModLoadStep.OnApplicationLateStart_Plugins);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationLateStart_Plugin(string name)
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressFromMod(name);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationLateStart_Mods()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressState(ModLoadStep.OnApplicationLateStart_Mods);
            ProcessEventsAndRender();
        }

        internal static void OnApplicationLateStart_Mod(string name)
        {
            if (!initialized) return;

            ScreenRenderer.UpdateProgressFromMod(name);
            ProcessEventsAndRender();
        }

        internal static void Finish()
        {
            if (!initialized) return;

            ScreenRenderer.UpdateMainProgress("Starting game...", 1f);
            ScreenRenderer.Render(); // Final render, to set the progress bar to 100%
        }

        #endregion
    }
}

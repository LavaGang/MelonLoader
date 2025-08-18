#if ANDROID
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Proxy.Android;

internal static class StdRedirect
{
    private const int STDERR_FILENO = 2;
    private const int STDOUT_FILENO = 1;

    [DllImport("libc.so", SetLastError = true)]
    private static extern int pipe(int[] pipefd);

    [DllImport("libc.so", SetLastError = true)]
    private static extern int dup2(int oldfd, int newfd);

    [DllImport("libc.so", SetLastError = true)]
    private static extern IntPtr fdopen(int fd, string mode);

    [DllImport("libc.so", SetLastError = true)]
    private static extern IntPtr fgets(byte[] buffer, int size, IntPtr stream);

    [DllImport("liblog.so", SetLastError = true)]
    private static extern int __android_log_write(int prio, string tag, string text);

    public static void RedirectStdErr()
    {
        Environment.SetEnvironmentVariable("COREHOST_TRACE", "1");
        Environment.SetEnvironmentVariable("COREHOST_TRACE_VERBOSITY", "3");

        RedirectStream(STDERR_FILENO, "MelonLoader");
    }

    public static void RedirectStdOut()
    {
        Environment.SetEnvironmentVariable("COREHOST_TRACE", "1");
        Environment.SetEnvironmentVariable("COREHOST_TRACE_VERBOSITY", "3");

        RedirectStream(STDOUT_FILENO, "MelonLoader");
    }

    private static void RedirectStream(int fileno, string tag)
    {
        int[] pipes = new int[2];
        if (pipe(pipes) != 0)
        {
            Console.WriteLine("Failed to create pipe");
            return;
        }

        dup2(pipes[1], fileno);
        IntPtr inputFile = fdopen(pipes[0], "r");

        Thread logThread = new Thread(() =>
        {
            byte[] buffer = new byte[512];
            while (true)
            {
                IntPtr result = fgets(buffer, buffer.Length, inputFile);
                if (result == IntPtr.Zero)
                    break;

                string logMsg = Encoding.UTF8.GetString(buffer).TrimEnd('\0', '\n', '\r');
                __android_log_write(3, tag, logMsg); // debug
            }
        });

        logThread.IsBackground = true;
        logThread.Start();
    }
}
#endif

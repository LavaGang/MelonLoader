using MelonLoader.Shared.Utils;

namespace MelonLoader.Shared
{
    public class Core
    {
        public static void Startup()
        {
            MelonEnvironment.Initialize();
        }

        public static int OnAppPreStart()
        {
            return 0;
        }

        public static int OnAppStart()
        {
            return 0;
        }
    }
}
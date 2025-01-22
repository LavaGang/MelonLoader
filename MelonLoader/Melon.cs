namespace MelonLoader
{
    public static class Melon<T> where T : MelonBase
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var melon = MelonAssembly.FindMelonInstance<T>();
                if (melon == null)
                    return null;

                _instance = melon;
                return melon;
            }
        }

        public static MelonLogger.Instance Logger => Instance?.LoggerInstance;
    }
}

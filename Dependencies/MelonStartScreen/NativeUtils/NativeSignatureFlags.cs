namespace MelonLoader.MelonStartScreen.NativeUtils
{
    internal enum NativeSignatureFlags
    {
        None = 0,

        Il2Cpp = 1,
        Mono = 2,

        //Dev = 4,
        //NonDev = 8

        X86 = 16,
        X64 = 32,
        //ARMEABIV7A = 64,
    }
}

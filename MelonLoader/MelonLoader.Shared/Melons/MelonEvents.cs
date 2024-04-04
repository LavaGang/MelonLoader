using System;

namespace MelonLoader.Melons;

public static class MelonEvents
{
    public static event Action OnApplicationPreStart;
    public static event Action OnApplicationStart;
    
    internal static void InternalInvokeOnApplicationPreStart() => OnApplicationPreStart?.Invoke();
    internal static void InternalInvokeOnApplicationStart() => OnApplicationStart?.Invoke();
}
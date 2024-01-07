namespace MelonLoader.Melons;

public abstract class MelonTypeBase<T> : MelonBase where T : MelonTypeBase<T>
{
    public static string TypeName { get; protected internal set; }
    
    static MelonTypeBase() => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // To make sure that the type initializer of T was triggered.
    
}
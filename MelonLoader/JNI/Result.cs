#if ANDROID
namespace MelonLoader.Java;

public static partial class JNI
{
    /// <summary>
    /// Represents a JNI error.
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Success.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Unknown error occured.
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Thread detached from the JVM.
        /// </summary>
        Detached = -2,
        /// <summary>
        /// Version unsupported.
        /// </summary>
        Version = -3,
        /// <summary>
        /// JVM out of memory.
        /// </summary>
        OutOfMemory = -4,
        /// <summary>
        /// JVM already exists.
        /// </summary>
        AlreadyExists = -5,
        /// <summary>
        /// Invalid arguments passed.
        /// </summary>
        InvalidArguments = -6
    }
}
#endif

using System;
using System.Reflection;
using MonoMod.Utils;

namespace HarmonyLib
{
    /// <summary>
    ///     An exception thrown when a patch argument in a Harmony patch is invalid.
    /// </summary>
    public class InvalidHarmonyPatchArgumentException : Exception
    {
        /// <summary>
        ///     Original method to be patched.
        /// </summary>
        public MethodBase Original { get; }

        /// <summary>
        ///     Patch that was attempted to be applied.
        /// </summary>
        public MethodInfo Patch { get; }

        /// <inheritdoc />
        public override string Message => $"({Patch.FullDescription()}): {base.Message}";

        /// <summary>
        ///     Constructs a new exception instance.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="original">Original method to be patched.</param>
        /// <param name="patch">Patch that was attempted to be applied.</param>
        public InvalidHarmonyPatchArgumentException(string message, MethodBase original, MethodInfo patch) :
            base(message)
        {
            Original = original;
            Patch = patch;
        }
    }

    /// <summary>
    ///     An exception thrown when a reflection member is not found.
    /// </summary>
    public class MemberNotFoundException : Exception
    {
        /// <inheritdoc />
        public MemberNotFoundException(string message) : base(message) { }
    }
}

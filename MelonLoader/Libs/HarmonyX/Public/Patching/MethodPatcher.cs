using System.Reflection;
using HarmonyLib.Internal.Patching;
using MonoMod.Utils;

namespace HarmonyLib.Public.Patching
{
	/// <summary>
	/// A general method patcher for implementing custom Harmony patcher backends.
	/// </summary>
	///
	public abstract class MethodPatcher
	{
		/// <summary>
		/// Constructs a method patcher
		/// </summary>
		/// <param name="original">Original method to patch</param>
		///
		protected MethodPatcher(MethodBase original)
		{
			Original = original;
		}

		/// <summary>
		/// Original method to patch.
		/// </summary>
		///
		public MethodBase Original { get; }

		/// <summary>
		/// Prepares method body for the unpatched <see cref="DynamicMethodDefinition"/> that simply calls
		/// <see cref="Original"/> function.
		/// </summary>
		/// <returns>
		/// A <see cref="DynamicMethodDefinition"/> that contains a call to
		/// the original method to pass to the IL manipulator.
		/// If <b>null</b>, Harmony patches must be manually applied to the original via <see cref="HarmonyManipulator.Manipulate"/>.
		/// </returns>
		///
		public abstract DynamicMethodDefinition PrepareOriginal();

		/// <summary>
		/// Detours <see cref="Original"/> to the provided replacement function. If called multiple times,
		/// <see cref="Original"/> is re-detoured to the new method.
		/// </summary>
		/// <param name="replacement">
		/// Result of <see cref="HarmonyManipulator.Manipulate"/>
		/// if <see cref="PrepareOriginal"/> returned non-<b>null</b>.
		/// Otherwise, this will be <b>null</b>, in which case you must manually generate Harmony-patched method
		/// with <see cref="HarmonyManipulator.Manipulate"/>.
		/// </param>
		/// <returns><see cref="MethodBase"/> of the hook, if it's different from `replacement`.</returns>
		///
		public abstract MethodBase DetourTo(MethodBase replacement);

		/// <summary>
		/// Creates a copy of the original method. If not possible, creates a method that calls into the original method.
		/// </summary>
		/// <returns>Copy of the original method that is transpileable. If not possible, returns <b>null</b>.</returns>
		/// <remarks>
		/// This method creates a pure copy of the original method that is usable with transpilers. Currently, this
		/// method is used to generate reverse patchers.
		/// If a purse IL copy is not possible, a best approximation should be generated
		/// (e.g. a wrapper that calls original method).
		/// If no best approximation is possible, this method should return <b>null</b>, in which case generating reverse
		/// patchers for the method will fail.
		/// </remarks>
		///
		public abstract DynamicMethodDefinition CopyOriginal();
	}
}

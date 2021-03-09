using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib.Internal.RuntimeFixes;
using HarmonyLib.Public.Patching;
using HarmonyLib.Tools;

namespace HarmonyLib
{
	/// <summary>The Harmony instance is the main entry to Harmony. After creating one with an unique identifier, it is used to patch and query the current application domain</summary>
	///
	public class Harmony
	{
		/// <summary>Set to true before instantiating Harmony to debug Harmony or use an environment variable to set HARMONY_DEBUG to '1' like this: cmd /C "set HARMONY_DEBUG=1 &amp;&amp; game.exe"</summary>
		/// <remarks>This is for full debugging. To debug only specific patches, use the <see cref="HarmonyDebug"/> attribute</remarks>
		///
		[Obsolete("Use HarmonyFileLog.Enabled instead")]
		// ReSharper disable once InconsistentNaming
		public static bool DEBUG;

		static Harmony()
		{
			StackTraceFixes.Install();
		}

		/// <summary>Creates a new Harmony instance</summary>
		/// <param name="id">A unique identifier (you choose your own)</param>
		/// <returns>A Harmony instance</returns>
		///
		public Harmony(string id)
		{
#pragma warning disable 618
			if (string.IsNullOrEmpty(id)) throw new ArgumentException($"{nameof(id)} cannot be null or empty");

			try
			{
				var envDebug = Environment.GetEnvironmentVariable("HARMONY_DEBUG");
				if (envDebug is object && envDebug.Length > 0)
				{
					envDebug = envDebug.Trim();
					DEBUG = envDebug == "1" || bool.Parse(envDebug);
				}
			}
			catch
			{
			}

			if (DEBUG)
				HarmonyFileLog.Enabled = true;

			Logger.Log(Logger.LogChannel.Info, () =>
			{
				var sb = new StringBuilder();
				var assembly = typeof(Harmony).Assembly;
				var version = assembly.GetName().Version;
				var location = assembly.Location;
				var environment = Environment.Version.ToString();
				var platform = Environment.OSVersion.Platform.ToString();
				if (string.IsNullOrEmpty(location)) location = new Uri(assembly.CodeBase).LocalPath;

				sb.AppendLine($"### Harmony id={id}, version={version}, location={location}, env/clr={environment}, platform={platform}");
				var callingMethod = AccessTools.GetOutsideCaller();
				if (callingMethod.DeclaringType is object)
				{
					var callingAssembly = callingMethod.DeclaringType.Assembly;
					location = callingAssembly.Location;
					if (string.IsNullOrEmpty(location)) location = new Uri(callingAssembly.CodeBase).LocalPath;
					sb.AppendLine($"### Started from {callingMethod.FullDescription()}, location {location}");
					sb.Append($"### At {DateTime.Now:yyyy-MM-dd hh.mm.ss}");
				}

				return sb.ToString();
			});

			Id = id;
#pragma warning restore 618
		}

		public static Harmony Create(string id)
		{
			if (id == null) throw new Exception("id cannot be null");
			return new Harmony(id);
		}

		/// <summary>The unique identifier</summary>
		///
		public string Id { get; }

		/// <summary>Searches the current assembly for Harmony annotations and uses them to create patches</summary>
		///
		public void PatchAll()
		{
			var method = new StackTrace().GetFrame(1).GetMethod();
			var assembly = method.ReflectedType.Assembly;
			PatchAll(assembly);
		}

		/// <summary>Creates a empty patch processor for an original method</summary>
		/// <param name="original">The original method/constructor</param>
		/// <returns>A new <see cref="PatchProcessor"/> instance</returns>
		///
		public PatchProcessor CreateProcessor(MethodBase original)
		{
			return new PatchProcessor(this, original);
		}

		/// <summary>Creates a patch class processor from an annotated class</summary>
		/// <param name="type">The class/type</param>
		/// <returns>A new <see cref="PatchClassProcessor"/> instance</returns>
		///
		public PatchClassProcessor CreateClassProcessor(Type type)
		{
			return new PatchClassProcessor(this, type);
		}

		/// <summary>Creates a patch class processor from an annotated class</summary>
		/// <param name="type">The class/type</param>
		/// <param name="allowUnannotatedType">If <b>true</b>, the type doesn't need to have any <see cref="HarmonyPatch"/> attributes present for processing</param>
		/// <returns>A new <see cref="PatchClassProcessor"/> instance</returns>
		///
		public PatchClassProcessor CreateClassProcessor(Type type, bool allowUnannotatedType)
		{
			return new PatchClassProcessor(this, type, allowUnannotatedType);
		}

		/// <summary>Creates a reverse patcher for one of your stub methods</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="standin">The stand-in stub method as <see cref="HarmonyMethod"/></param>
		/// <returns>A new <see cref="ReversePatcher"/> instance</returns>
		///
		public ReversePatcher CreateReversePatcher(MethodBase original, HarmonyMethod standin)
		{
			return new ReversePatcher(this, original, standin);
		}

		/// <summary>Searches an assembly for Harmony annotations and uses them to create patches</summary>
		/// <param name="assembly">The assembly</param>
		///
		public void PatchAll(Assembly assembly)
		{
			if (assembly.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0) return;
			AccessTools.GetTypesFromAssembly(assembly).Do(type => CreateClassProcessor(type).Patch());
		}

		/// <summary>Searches the given type for Harmony annotation and uses them to create patches</summary>
		/// <param name="type">The type to search</param>
		///
		public void PatchAll(Type type)
		{ 
			CreateClassProcessor(type, true).Patch();
		}

		/// <summary>Creates patches by manually specifying the methods</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="prefix">An optional prefix method wrapped in a <see cref="HarmonyMethod"/> object</param>
		/// <param name="postfix">An optional postfix method wrapped in a <see cref="HarmonyMethod"/> object</param>
		/// <param name="transpiler">An optional transpiler method wrapped in a <see cref="HarmonyMethod"/> object</param>
		/// <param name="finalizer">An optional finalizer method wrapped in a <see cref="HarmonyMethod"/> object</param>
		/// <returns>The replacement method that was created to patch the original method</returns>
		///
		public MethodInfo Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
		{
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0)
							  || (original.DeclaringType.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0)
							  || (original.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0))
				return null;
			var processor = CreateProcessor(original);
			_ = processor.AddPrefix(prefix);
			_ = processor.AddPostfix(postfix);
			_ = processor.AddTranspiler(transpiler);
			_ = processor.AddFinalizer(finalizer);
			return processor.Patch();
		}

		/// <summary>Patches a foreign method onto a stub method of yours and optionally applies transpilers during the process</summary>
		/// <param name="original">The original method/constructor you want to duplicate</param>
		/// <param name="standin">Your stub method as <see cref="HarmonyMethod"/> that will become the original. Needs to have the correct signature (either original or whatever your transpilers generates)</param>
		/// <param name="transpiler">An optional transpiler as method that will be applied during the process</param>
		/// <returns>The replacement method that was created to patch the stub method</returns>
		///
		public static MethodInfo ReversePatch(MethodBase original, HarmonyMethod standin, MethodInfo transpiler = null)
		{
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0)
								 || (original.DeclaringType.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0)
								 || (original.GetCustomAttributes(typeof(HarmonyShield), false).Count() > 0))
				return null;
			return PatchFunctions.ReversePatch(standin, original, transpiler);
		}

		/// <summary>Unpatches methods by patching them with zero patches. Fully unpatching is not supported. Be careful, unpatching is global</summary>
		/// <param name="harmonyID">The optional Harmony ID to restrict unpatching to a specific Harmony instance</param>
		/// <remarks>This method could be static if it wasn't for the fact that unpatching creates a new replacement method that contains your harmony ID</remarks>
		///
		public void UnpatchAll(string harmonyID = null)
		{
			if (harmonyID == null)
				Logger.Log(Logger.LogChannel.Warn, () => "UnpatchAll has been called with harmonyID=null - This will remove ALL HARMONY PATCHES. " +
				                                         "If you want to only unpatch patches created by this instance (" + Id + "), use UnpatchSelf() instead.");

			bool IDCheck(Patch patchInfo)
			{
				return harmonyID is null || patchInfo.owner == harmonyID;
			}

			var originals = GetAllPatchedMethods().ToList(); // keep as is to avoid "Collection was modified"
			foreach (var original in originals)
			{
				var hasBody = original.HasMethodBody();
				var info = GetPatchInfo(original);
				if (hasBody)
				{
					info.Postfixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
					info.Prefixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
				}
				info.Transpilers.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
				if (hasBody)
					info.Finalizers.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
			}
		}

		public void UnpatchAll(MelonLoader.MelonBase melon)
		{
			bool IDCheck(Patch patchInfo) => (patchInfo.PatchMethod.Module.Assembly == melon.Assembly);
			var originals = GetPatchedMethods().ToList();
			foreach (var original in originals)
			{
				var info = GetPatchInfo(original);
				info.Postfixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
				info.Prefixes.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
				info.Transpilers.DoIf(IDCheck, patchInfo => Unpatch(original, patchInfo.PatchMethod));
			}
		}

		/// <summary>Unpatches all methods that were patched by this Harmony instance's ID. Unpatching is done by repatching methods without patches of this instance.</summary>
		///
		public void UnpatchSelf()
		{
			UnpatchAll(Id);
		}

		/// <summary>Unpatches a method by patching it with zero patches. Fully unpatching is not supported. Be careful, unpatching is global</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="type">The <see cref="HarmonyPatchType"/></param>
		/// <param name="harmonyID">The optional Harmony ID to restrict unpatching to a specific Harmony instance</param>
		///
		public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = null)
		{
			var processor = CreateProcessor(original);
			_ = processor.Unpatch(type, harmonyID);
		}

		/// <summary>Unpatches a method by patching it with zero patches. Fully unpatching is not supported. Be careful, unpatching is global</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="patch">The patch method as method to remove</param>
		///
		public void Unpatch(MethodBase original, MethodInfo patch)
		{
			var processor = CreateProcessor(original);
			_ = processor.Unpatch(patch);
		}

		/// <summary>Test for patches from a specific Harmony ID</summary>
		/// <param name="harmonyID">The Harmony ID</param>
		/// <returns>True if patches for this ID exist</returns>
		///
		public static bool HasAnyPatches(string harmonyID)
		{
			return GetAllPatchedMethods()
				.Select(original => GetPatchInfo(original))
				.Any(info => info.Owners.Contains(harmonyID));
		}

		/// <summary>Gets patch information for a given original method</summary>
		/// <param name="method">The original method/constructor</param>
		/// <returns>The patch information as <see cref="Patches"/></returns>
		///
		public static Patches GetPatchInfo(MethodBase method)
		{
			return PatchProcessor.GetPatchInfo(method);
		}

		/// <summary>Gets the methods this instance has patched</summary>
		/// <returns>An enumeration of original methods/constructors</returns>
		///
		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return GetAllPatchedMethods()
				.Where(original => GetPatchInfo(original).Owners.Contains(Id));
		}

		/// <summary>Gets all patched original methods in the appdomain</summary>
		/// <returns>An enumeration of patched original methods/constructors</returns>
		///
		public static IEnumerable<MethodBase> GetAllPatchedMethods()
		{
			return PatchProcessor.GetAllPatchedMethods();
		}

		/// <summary>Gets the original method from a given replacement method</summary>
		/// <param name="replacement">A replacement method, for example from a stacktrace</param>
		/// <returns>The original method/constructor or <c>null</c> if not found</returns>
		///
		public static MethodBase GetOriginalMethod(MethodInfo replacement)
		{
			if (replacement == null) throw new ArgumentNullException(nameof(replacement));
			return PatchManager.GetOriginal(replacement);
		}

		/// <summary>Tries to get the method from a stackframe including dynamic replacement methods</summary>
		/// <param name="frame">The <see cref="StackFrame"/></param>
		/// <returns>For normal frames, <c>frame.GetMethod()</c> is returned. For frames containing patched methods, the replacement method is returned or <c>null</c> if no method can be found</returns>
		///
		public static MethodBase GetMethodFromStackframe(StackFrame frame)
		{
			if (frame == null) throw new ArgumentNullException(nameof(frame));
			var method = frame.GetMethod();
			if (method != null) return method;
			return PatchManager.FindReplacement(frame);
		}

		/// <summary>Gets Harmony version for all active Harmony instances</summary>
		/// <param name="currentVersion">[out] The current Harmony version</param>
		/// <returns>A dictionary containing assembly versions keyed by Harmony IDs</returns>
		///
		public static Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			return PatchProcessor.VersionInfo(out currentVersion);
		}

		/// <summary>Creates a new Harmony instance and applies all patches specified in the type</summary>
		/// <param name="type">The type to scan for patches.</param>
		/// <param name="harmonyInstanceId">The ID for the Harmony instance to create, which will be used.</param>
		///
		public static Harmony CreateAndPatchAll(Type type, string harmonyInstanceId = null)
		{
			var harmony = new Harmony(harmonyInstanceId ?? $"harmony-auto-{Guid.NewGuid()}");
			harmony.PatchAll(type);
			return harmony;
		}

		/// <summary>Applies all patches specified in the assembly</summary>
		/// <param name="assembly">The assembly to scan.</param>
		/// <param name="harmonyInstanceId">The ID for the Harmony instance to create, which will be used.</param>
		///
		public static Harmony CreateAndPatchAll(Assembly assembly, string harmonyInstanceId = null)
		{
			var harmony = new Harmony(harmonyInstanceId ?? $"harmony-auto-{Guid.NewGuid()}");
			harmony.PatchAll(assembly);
			return harmony;
		}
	}
}

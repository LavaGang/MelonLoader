using System;

namespace MelonLoader.ICSharpCode.SharpZipLib.Core
{
	internal static class Empty
	{
		internal static class EmptyArray<T>
		{
			public static readonly T[] Value = new T[0];
		}
		public static T[] Array<T>() => EmptyArray<T>.Value;
	}
}

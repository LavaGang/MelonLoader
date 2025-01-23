using System;
using System.Text;
using MelonLoader.ICSharpCode.SharpZipLib.Core;

namespace MelonLoader.ICSharpCode.SharpZipLib.Zip
{
	/// <summary>
	/// This static class contains functions for encoding and decoding zip file strings
	/// </summary>
	public static class ZipStrings
	{
		static ZipStrings()
		{
			try
			{
				var platformCodepage = Encoding.GetEncoding(0).CodePage;
				SystemDefaultCodePage = (platformCodepage == 1 || platformCodepage == 2 || platformCodepage == 3 || platformCodepage == 42) ? FallbackCodePage : platformCodepage;
			}
			catch
			{
				SystemDefaultCodePage = FallbackCodePage;
			}
		}

		/// <summary>Code page backing field</summary>
		/// <remarks>
		/// The original Zip specification (https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT) states
		/// that file names should only be encoded with IBM Code Page 437 or UTF-8.
		/// In practice, most zip apps use OEM or system encoding (typically cp437 on Windows).
		/// Let's be good citizens and default to UTF-8 http://utf8everywhere.org/
		/// </remarks>
		private static int codePage = AutomaticCodePage;

		/// Automatically select codepage while opening archive
		/// see https://github.com/icsharpcode/SharpZipLib/pull/280#issuecomment-433608324
		/// 
		private const int AutomaticCodePage = -1;

		/// <summary>
		/// Encoding used for string conversion. Setting this to 65001 (UTF-8) will
		/// also set the Language encoding flag to indicate UTF-8 encoded file names.
		/// </summary>
		public static int CodePage
		{
			get
			{
				return codePage == AutomaticCodePage? Encoding.UTF8.CodePage:codePage;
			}
			set
			{
				if ((value < 0) || (value > 65535) ||
					(value == 1) || (value == 2) || (value == 3) || (value == 42))
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				codePage = value;
			}
		}

		private const int FallbackCodePage = 437;

		/// <summary>
		/// Attempt to get the operating system default codepage, or failing that, to
		/// the fallback code page IBM 437.
		/// </summary>
		public static int SystemDefaultCodePage { get; }

		/// <summary>
		/// Get whether the default codepage is set to UTF-8. Setting this property to false will
		/// set the <see cref="CodePage"/> to <see cref="SystemDefaultCodePage"/>
		/// </summary>
		/// <remarks>
		/// Get OEM codepage from NetFX, which parses the NLP file with culture info table etc etc.
		/// But sometimes it yields the special value of 1 which is nicknamed <c>CodePageNoOEM</c> in <see cref="Encoding"/> sources (might also mean <c>CP_OEMCP</c>, but Encoding puts it so).
		/// This was observed on Ukranian and Hindu systems.
		/// Given this value, <see cref="Encoding.GetEncoding(int)"/> throws an <see cref="ArgumentException"/>.
		/// So replace it with <see cref="FallbackCodePage"/>, (IBM 437 which is the default code page in a default Windows installation console.
		/// </remarks>
		public static bool UseUnicode
		{
			get
			{
				return codePage == Encoding.UTF8.CodePage;
			}
			set
			{
				if (value)
				{
					codePage = Encoding.UTF8.CodePage;
				}
				else
				{
					codePage = SystemDefaultCodePage;
				}
			}
		}

		/// <summary>
		/// Convert a portion of a byte array to a string using <see cref="CodePage"/>
		/// </summary>
		/// <param name="data">
		/// Data to convert to string
		/// </param>
		/// <param name="count">
		/// Number of bytes to convert starting from index 0
		/// </param>
		/// <returns>
		/// data[0]..data[count - 1] converted to a string
		/// </returns>
		public static string ConvertToString(byte[] data, int count)
			=> data == null
			? string.Empty
			: Encoding.GetEncoding(CodePage).GetString(data, 0, count);

		/// <summary>
		/// Convert a byte array to a string using <see cref="CodePage"/>
		/// </summary>
		/// <param name="data">
		/// Byte array to convert
		/// </param>
		/// <returns>
		/// <paramref name="data">data</paramref>converted to a string
		/// </returns>
		public static string ConvertToString(byte[] data)
			=> ConvertToString(data, data.Length);

		private static Encoding EncodingFromFlag(int flags)
			=> ((flags & (int)GeneralBitFlags.UnicodeText) != 0)
				? Encoding.UTF8
				: Encoding.GetEncoding(
					// if CodePage wasn't set manually and no utf flag present
					// then we must use SystemDefault (old behavior)
					// otherwise, CodePage should be preferred over SystemDefault
					// see https://github.com/icsharpcode/SharpZipLib/issues/274
					codePage == AutomaticCodePage? 
						SystemDefaultCodePage:
						codePage);

		/// <summary>
		/// Convert a byte array to a string  using <see cref="CodePage"/>
		/// </summary>
		/// <param name="flags">The applicable general purpose bits flags</param>
		/// <param name="data">
		/// Byte array to convert
		/// </param>
		/// <param name="count">The number of bytes to convert.</param>
		/// <returns>
		/// <paramref name="data">data</paramref>converted to a string
		/// </returns>
		public static string ConvertToStringExt(int flags, byte[] data, int count)
			=> (data == null)
				? string.Empty
				: EncodingFromFlag(flags).GetString(data, 0, count);

		/// <summary>
		/// Convert a byte array to a string using <see cref="CodePage"/>
		/// </summary>
		/// <param name="data">
		/// Byte array to convert
		/// </param>
		/// <param name="flags">The applicable general purpose bits flags</param>
		/// <returns>
		/// <paramref name="data">data</paramref>converted to a string
		/// </returns>
		public static string ConvertToStringExt(int flags, byte[] data)
			=> ConvertToStringExt(flags, data, data.Length);

		/// <summary>
		/// Convert a string to a byte array using <see cref="CodePage"/>
		/// </summary>
		/// <param name="str">
		/// String to convert to an array
		/// </param>
		/// <returns>Converted array</returns>
		public static byte[] ConvertToArray(string str)
			=> str == null
			? Empty.Array<byte>()
			: Encoding.GetEncoding(CodePage).GetBytes(str);

		/// <summary>
		/// Convert a string to a byte array using <see cref="CodePage"/>
		/// </summary>
		/// <param name="flags">The applicable <see cref="GeneralBitFlags">general purpose bits flags</see></param>
		/// <param name="str">
		/// String to convert to an array
		/// </param>
		/// <returns>Converted array</returns>
		public static byte[] ConvertToArray(int flags, string str)
			=> (string.IsNullOrEmpty(str))
				? Empty.Array<byte>()
				: EncodingFromFlag(flags).GetBytes(str);
	}
}

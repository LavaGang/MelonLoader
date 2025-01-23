using System;

namespace MelonLoader.ICSharpCode.SharpZipLib.Zip
{
    /// <summary>
    /// The method of encrypting entries when creating zip archives.
    /// </summary>
    [Obsolete("Please use an alternative library instead. This will be removed in a future version.", true)]
    public enum ZipEncryptionMethod
	{
		/// <summary>
		/// No encryption will be used.
		/// </summary>
		None,

		/// <summary>
		/// Encrypt entries with ZipCrypto.
		/// </summary>
		ZipCrypto,

		/// <summary>
		/// Encrypt entries with AES 128.
		/// </summary>
		AES128,

		/// <summary>
		/// Encrypt entries with AES 256.
		/// </summary>
		AES256
	}
}

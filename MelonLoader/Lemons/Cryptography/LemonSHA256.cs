using System;
using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonSHA256
	{
		private static HashAlgorithm algorithm;

		static LemonSHA256()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.SHA256");
			algorithm.SetHashSizeValue(256);
		}

		public static byte[] ComputeSHA256Hash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public static byte[] ComputeSHA256Hash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public static byte[] ComputeSHA256Hash(Stream inputStream) => algorithm.ComputeHash(inputStream);

        #region Obsolete Members
        [Obsolete("Use the static method instead.")]
		public byte[] ComputeHash(byte[] buffer) => ComputeSHA256Hash(buffer);
		[Obsolete("Use the static method instead.")]
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => ComputeSHA256Hash(buffer, offset, count);
        [Obsolete("Use the static method instead.")]
        public byte[] ComputeHash(Stream inputStream) => ComputeSHA256Hash(inputStream);
        #endregion
    }
}

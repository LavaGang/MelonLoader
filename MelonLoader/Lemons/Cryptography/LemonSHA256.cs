using System;
using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonSHA256
	{
		private HashAlgorithm algorithm;
        private static LemonSHA256 static_algorithm = new();

        public LemonSHA256()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.SHA256");
			algorithm.SetHashSizeValue(256);
		}

		public static byte[] ComputeSHA256Hash(byte[] buffer) => static_algorithm.ComputeHash(buffer);
        public static byte[] ComputeSHA256Hash(byte[] buffer, int offset, int count) => static_algorithm.ComputeHash(buffer, offset, count);
        public static byte[] ComputeSHA256Hash(Stream inputStream) => static_algorithm.ComputeHash(inputStream);

        public byte[] ComputeHash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public byte[] ComputeHash(Stream inputStream) => algorithm.ComputeHash(inputStream);
    }
}

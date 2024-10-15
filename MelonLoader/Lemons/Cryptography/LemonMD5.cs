using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonMD5
	{
		private HashAlgorithm algorithm;
        private static LemonMD5 static_algorithm = new();

        public LemonMD5()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.MD5");
			algorithm.SetHashSizeValue(256);
		}

        public static byte[] ComputeMD5Hash(byte[] buffer) => static_algorithm.ComputeHash(buffer);
        public static byte[] ComputeMD5Hash(byte[] buffer, int offset, int count) => static_algorithm.ComputeHash(buffer, offset, count);
        public static byte[] ComputeMD5Hash(Stream inputStream) => static_algorithm.ComputeHash(inputStream);

        public byte[] ComputeHash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public byte[] ComputeHash(Stream inputStream) => algorithm.ComputeHash(inputStream);
	}
}

using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonSHA512
	{
		private HashAlgorithm algorithm;

		public LemonSHA512()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.SHA512");
			algorithm.SetHashSizeValue(512);
		}

        public byte[] ComputeHash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public byte[] ComputeHash(Stream inputStream) => algorithm.ComputeHash(inputStream);
    }
}

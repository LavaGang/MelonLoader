using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonSHA256
	{
		private HashAlgorithm algorithm;

		public LemonSHA256()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.SHA256");
			algorithm.SetHashSizeValue(256);
		}

        public byte[] ComputeHash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public byte[] ComputeHash(Stream inputStream) => algorithm.ComputeHash(inputStream);
    }
}

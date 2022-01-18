using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.Lemons.Cryptography
{
	public class LemonMD5
	{
		private HashAlgorithm algorithm;

		public LemonMD5()
		{
			algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("System.Security.Cryptography.MD5");
			algorithm.SetHashSizeValue(256);
		}

		public byte[] ComputeHash(byte[] buffer) => algorithm.ComputeHash(buffer);
		public byte[] ComputeHash(byte[] buffer, int offset, int count) => algorithm.ComputeHash(buffer, offset, count);
		public byte[] ComputeHash(Stream inputStream) => algorithm.ComputeHash(inputStream);
	}
}

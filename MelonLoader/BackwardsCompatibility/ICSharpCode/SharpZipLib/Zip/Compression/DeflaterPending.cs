using System;

namespace MelonLoader.ICSharpCode.SharpZipLib.Zip.Compression
{
    /// <summary>
    /// This class stores the pending output of the Deflater.
    ///
    /// author of the original java version : Jochen Hoenicke
    /// </summary>
    [Obsolete("Please use an alternative library instead. This will be removed in a future version.", true)]
    public class DeflaterPending : PendingBuffer
	{
		/// <summary>
		/// Construct instance with default buffer size
		/// </summary>
		public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE)
		{
		}
	}
}

using MelonLoader.ICSharpCode.SharpZipLib.Checksum;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using MelonLoader.ICSharpCode.SharpZipLib.Encryption;
using MelonLoader.ICSharpCode.SharpZipLib.Zip.Compression;
using MelonLoader.ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace MelonLoader.ICSharpCode.SharpZipLib.Zip
{
	/// <summary>
	/// This is a DeflaterOutputStream that writes the files into a zip
	/// archive one after another.  It has a special method to start a new
	/// zip entry.  The zip entries contains information about the file name
	/// size, compressed size, CRC, etc.
	///
	/// It includes support for Stored and Deflated entries.
	/// This class is not thread safe.
	/// <br/>
	/// <br/>Author of the original java version : Jochen Hoenicke
	/// </summary>
	/// <example> This sample shows how to create a zip file
	/// <code>
	/// using System;
	/// using System.IO;
	///
	/// using MelonLoader.ICSharpCode.SharpZipLib.Core;
	/// using MelonLoader.ICSharpCode.SharpZipLib.Zip;
	///
	/// class MainClass
	/// {
	/// 	public static void Main(string[] args)
	/// 	{
	/// 		string[] filenames = Directory.GetFiles(args[0]);
	/// 		byte[] buffer = new byte[4096];
	///
	/// 		using ( ZipOutputStream s = new ZipOutputStream(File.Create(args[1])) ) {
	///
	/// 			s.SetLevel(9); // 0 - store only to 9 - means best compression
	///
	/// 			foreach (string file in filenames) {
	/// 				ZipEntry entry = new ZipEntry(file);
	/// 				s.PutNextEntry(entry);
	///
	/// 				using (FileStream fs = File.OpenRead(file)) {
	///						StreamUtils.Copy(fs, s, buffer);
	/// 				}
	/// 			}
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class ZipOutputStream : DeflaterOutputStream
	{
		#region Constructors

		/// <summary>
		/// Creates a new Zip output stream, writing a zip archive.
		/// </summary>
		/// <param name="baseOutputStream">
		/// The output stream to which the archive contents are written.
		/// </param>
		public ZipOutputStream(Stream baseOutputStream)
			: base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true))
		{
		}

		/// <summary>
		/// Creates a new Zip output stream, writing a zip archive.
		/// </summary>
		/// <param name="baseOutputStream">The output stream to which the archive contents are written.</param>
		/// <param name="bufferSize">Size of the buffer to use.</param>
		public ZipOutputStream(Stream baseOutputStream, int bufferSize)
			: base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true), bufferSize)
		{
		}

		#endregion Constructors

		/// <summary>
		/// Gets a flag value of true if the central header has been added for this archive; false if it has not been added.
		/// </summary>
		/// <remarks>No further entries can be added once this has been done.</remarks>
		public bool IsFinished
		{
			get
			{
				return entries == null;
			}
		}

		/// <summary>
		/// Set the zip file comment.
		/// </summary>
		/// <param name="comment">
		/// The comment text for the entire archive.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The converted comment is longer than 0xffff bytes.
		/// </exception>
		public void SetComment(string comment)
		{
			// TODO: Its not yet clear how to handle unicode comments here.
			byte[] commentBytes = ZipStrings.ConvertToArray(comment);
			if (commentBytes.Length > 0xffff)
			{
				throw new ArgumentOutOfRangeException(nameof(comment));
			}
			zipComment = commentBytes;
		}

		/// <summary>
		/// Sets the compression level.  The new level will be activated
		/// immediately.
		/// </summary>
		/// <param name="level">The new compression level (1 to 9).</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Level specified is not supported.
		/// </exception>
		/// <see cref="ICSharpCode.SharpZipLib.Zip.Compression.Deflater"/>
		public void SetLevel(int level)
		{
			deflater_.SetLevel(level);
			defaultCompressionLevel = level;
		}

		/// <summary>
		/// Get the current deflater compression level
		/// </summary>
		/// <returns>The current compression level</returns>
		public int GetLevel()
		{
			return deflater_.GetLevel();
		}

		/// <summary>
		/// Get / set a value indicating how Zip64 Extension usage is determined when adding entries.
		/// </summary>
		/// <remarks>Older archivers may not understand Zip64 extensions.
		/// If backwards compatability is an issue be careful when adding <see cref="ZipEntry.Size">entries</see> to an archive.
		/// Setting this property to off is workable but less desirable as in those circumstances adding a file
		/// larger then 4GB will fail.</remarks>
		public UseZip64 UseZip64
		{
			get { return useZip64_; }
			set { useZip64_ = value; }
		}

		/// <summary>
		/// Used for transforming the names of entries added by <see cref="PutNextEntry(ZipEntry)"/>.
		/// Defaults to <see cref="PathTransformer"/>, set to null to disable transforms and use names as supplied.
		/// </summary>
		public INameTransform NameTransform { get; set; } = new PathTransformer();

		/// <summary>
		/// Get/set the password used for encryption.
		/// </summary>
		/// <remarks>When set to null or if the password is empty no encryption is performed</remarks>
		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				if ((value != null) && (value.Length == 0))
				{
					password = null;
				}
				else
				{
					password = value;
				}
			}
		}

		/// <summary>
		/// Write an unsigned short in little endian byte order.
		/// </summary>
		private void WriteLeShort(int value)
		{
			unchecked
			{
				baseOutputStream_.WriteByte((byte)(value & 0xff));
				baseOutputStream_.WriteByte((byte)((value >> 8) & 0xff));
			}
		}

		/// <summary>
		/// Write an int in little endian byte order.
		/// </summary>
		private void WriteLeInt(int value)
		{
			unchecked
			{
				WriteLeShort(value);
				WriteLeShort(value >> 16);
			}
		}

		/// <summary>
		/// Write an int in little endian byte order.
		/// </summary>
		private void WriteLeLong(long value)
		{
			unchecked
			{
				WriteLeInt((int)value);
				WriteLeInt((int)(value >> 32));
			}
		}

		// Apply any configured transforms/cleaning to the name of the supplied entry.
		private void TransformEntryName(ZipEntry entry)
		{
			if (this.NameTransform != null)
			{
				if (entry.IsDirectory)
				{
					entry.Name = this.NameTransform.TransformDirectory(entry.Name);
				}
				else
				{
					entry.Name = this.NameTransform.TransformFile(entry.Name);
				}
			}
		}

		/// <summary>
		/// Starts a new Zip entry. It automatically closes the previous
		/// entry if present.
		/// All entry elements bar name are optional, but must be correct if present.
		/// If the compression method is stored and the output is not patchable
		/// the compression for that entry is automatically changed to deflate level 0
		/// </summary>
		/// <param name="entry">
		/// the entry.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// if entry passed is null.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// if an I/O error occured.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// if stream was finished
		/// </exception>
		/// <exception cref="ZipException">
		/// Too many entries in the Zip file<br/>
		/// Entry name is too long<br/>
		/// Finish has already been called<br/>
		/// </exception>
		/// <exception cref="System.NotImplementedException">
		/// The Compression method specified for the entry is unsupported.
		/// </exception>
		public void PutNextEntry(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException(nameof(entry));
			}

			if (entries == null)
			{
				throw new InvalidOperationException("ZipOutputStream was finished");
			}

			if (curEntry != null)
			{
				CloseEntry();
			}

			if (entries.Count == int.MaxValue)
			{
				throw new ZipException("Too many entries for Zip file");
			}

			CompressionMethod method = entry.CompressionMethod;

			// Check that the compression is one that we support
			if (method != CompressionMethod.Deflated && method != CompressionMethod.Stored)
			{
				throw new NotImplementedException("Compression method not supported");
			}

			// A password must have been set in order to add AES encrypted entries
			if (entry.AESKeySize > 0 && string.IsNullOrEmpty(this.Password))
			{
				throw new InvalidOperationException("The Password property must be set before AES encrypted entries can be added");
			}

			int compressionLevel = defaultCompressionLevel;

			// Clear flags that the library manages internally
			entry.Flags &= (int)GeneralBitFlags.UnicodeText;
			patchEntryHeader = false;

			bool headerInfoAvailable;

			// No need to compress - definitely no data.
			if (entry.Size == 0)
			{
				entry.CompressedSize = entry.Size;
				entry.Crc = 0;
				method = CompressionMethod.Stored;
				headerInfoAvailable = true;
			}
			else
			{
				headerInfoAvailable = (entry.Size >= 0) && entry.HasCrc && entry.CompressedSize >= 0;

				// Switch to deflation if storing isnt possible.
				if (method == CompressionMethod.Stored)
				{
					if (!headerInfoAvailable)
					{
						if (!CanPatchEntries)
						{
							// Can't patch entries so storing is not possible.
							method = CompressionMethod.Deflated;
							compressionLevel = 0;
						}
					}
					else // entry.size must be > 0
					{
						entry.CompressedSize = entry.Size;
						headerInfoAvailable = entry.HasCrc;
					}
				}
			}

			if (headerInfoAvailable == false)
			{
				if (CanPatchEntries == false)
				{
					// Only way to record size and compressed size is to append a data descriptor
					// after compressed data.

					// Stored entries of this form have already been converted to deflating.
					entry.Flags |= 8;
				}
				else
				{
					patchEntryHeader = true;
				}
			}

			if (Password != null)
			{
				entry.IsCrypted = true;
				if (entry.Crc < 0)
				{
					// Need to append a data descriptor as the crc isnt available for use
					// with encryption, the date is used instead.  Setting the flag
					// indicates this to the decompressor.
					entry.Flags |= 8;
				}
			}

			entry.Offset = offset;
			entry.CompressionMethod = (CompressionMethod)method;

			curMethod = method;
			sizePatchPos = -1;

			if ((useZip64_ == UseZip64.On) || ((entry.Size < 0) && (useZip64_ == UseZip64.Dynamic)))
			{
				entry.ForceZip64();
			}

			// Write the local file header
			WriteLeInt(ZipConstants.LocalHeaderSignature);

			WriteLeShort(entry.Version);
			WriteLeShort(entry.Flags);
			WriteLeShort((byte)entry.CompressionMethodForHeader);
			WriteLeInt((int)entry.DosTime);

			// TODO: Refactor header writing.  Its done in several places.
			if (headerInfoAvailable)
			{
				WriteLeInt((int)entry.Crc);
				if (entry.LocalHeaderRequiresZip64)
				{
					WriteLeInt(-1);
					WriteLeInt(-1);
				}
				else
				{
					WriteLeInt((int)entry.CompressedSize + entry.EncryptionOverheadSize);
					WriteLeInt((int)entry.Size);
				}
			}
			else
			{
				if (patchEntryHeader)
				{
					crcPatchPos = baseOutputStream_.Position;
				}
				WriteLeInt(0);  // Crc

				if (patchEntryHeader)
				{
					sizePatchPos = baseOutputStream_.Position;
				}

				// For local header both sizes appear in Zip64 Extended Information
				if (entry.LocalHeaderRequiresZip64 || patchEntryHeader)
				{
					WriteLeInt(-1);
					WriteLeInt(-1);
				}
				else
				{
					WriteLeInt(0);  // Compressed size
					WriteLeInt(0);  // Uncompressed size
				}
			}

			// Apply any required transforms to the entry name, and then convert to byte array format.
			TransformEntryName(entry);
			byte[] name = ZipStrings.ConvertToArray(entry.Flags, entry.Name);

			if (name.Length > 0xFFFF)
			{
				throw new ZipException("Entry name too long.");
			}

			var ed = new ZipExtraData(entry.ExtraData);

			if (entry.LocalHeaderRequiresZip64)
			{
				ed.StartNewEntry();
				if (headerInfoAvailable)
				{
					ed.AddLeLong(entry.Size);
					ed.AddLeLong(entry.CompressedSize + entry.EncryptionOverheadSize);
				}
				else
				{
					ed.AddLeLong(-1);
					ed.AddLeLong(-1);
				}
				ed.AddNewEntry(1);

				if (!ed.Find(1))
				{
					throw new ZipException("Internal error cant find extra data");
				}

				if (patchEntryHeader)
				{
					sizePatchPos = ed.CurrentReadIndex;
				}
			}
			else
			{
				ed.Delete(1);
			}

			if (entry.AESKeySize > 0)
			{
				AddExtraDataAES(entry, ed);
			}
			byte[] extra = ed.GetEntryData();

			WriteLeShort(name.Length);
			WriteLeShort(extra.Length);

			if (name.Length > 0)
			{
				baseOutputStream_.Write(name, 0, name.Length);
			}

			if (entry.LocalHeaderRequiresZip64 && patchEntryHeader)
			{
				sizePatchPos += baseOutputStream_.Position;
			}

			if (extra.Length > 0)
			{
				baseOutputStream_.Write(extra, 0, extra.Length);
			}

			offset += ZipConstants.LocalHeaderBaseSize + name.Length + extra.Length;
			// Fix offsetOfCentraldir for AES
			if (entry.AESKeySize > 0)
				offset += entry.AESOverheadSize;

			// Activate the entry.
			curEntry = entry;
			crc.Reset();
			if (method == CompressionMethod.Deflated)
			{
				deflater_.Reset();
				deflater_.SetLevel(compressionLevel);
			}
			size = 0;

			if (entry.IsCrypted)
			{
				if (entry.AESKeySize > 0)
				{
					WriteAESHeader(entry);
				}
				else
				{
					if (entry.Crc < 0)
					{            // so testing Zip will says its ok
						WriteEncryptionHeader(entry.DosTime << 16);
					}
					else
					{
						WriteEncryptionHeader(entry.Crc);
					}
				}
			}
		}

		/// <summary>
		/// Closes the current entry, updating header and footer information as required
		/// </summary>
		/// <exception cref="ZipException">
		/// Invalid entry field values.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// No entry is active.
		/// </exception>
		public void CloseEntry()
		{
			if (curEntry == null)
			{
				throw new InvalidOperationException("No open entry");
			}

			long csize = size;

			// First finish the deflater, if appropriate
			if (curMethod == CompressionMethod.Deflated)
			{
				if (size >= 0)
				{
					base.Finish();
					csize = deflater_.TotalOut;
				}
				else
				{
					deflater_.Reset();
				}
			}
			else if (curMethod == CompressionMethod.Stored)
			{
				// This is done by Finish() for Deflated entries, but we need to do it
				// ourselves for Stored ones
				base.GetAuthCodeIfAES();
			}

			// Write the AES Authentication Code (a hash of the compressed and encrypted data)
			if (curEntry.AESKeySize > 0)
			{
				baseOutputStream_.Write(AESAuthCode, 0, 10);
				// Always use 0 as CRC for AE-2 format
				curEntry.Crc = 0;
			}
			else
			{
				if (curEntry.Crc < 0)
				{
					curEntry.Crc = crc.Value;
				}
				else if (curEntry.Crc != crc.Value)
				{
					throw new ZipException($"crc was {crc.Value}, but {curEntry.Crc} was expected");
				}
			}

			if (curEntry.Size < 0)
			{
				curEntry.Size = size;
			}
			else if (curEntry.Size != size)
			{
				throw new ZipException($"size was {size}, but {curEntry.Size} was expected");
			}

			if (curEntry.CompressedSize < 0)
			{
				curEntry.CompressedSize = csize;
			}
			else if (curEntry.CompressedSize != csize)
			{
				throw new ZipException($"compressed size was {csize}, but {curEntry.CompressedSize} expected");
			}

			offset += csize;

			if (curEntry.IsCrypted)
			{
				curEntry.CompressedSize += curEntry.EncryptionOverheadSize;
			}

			// Patch the header if possible
			if (patchEntryHeader)
			{
				patchEntryHeader = false;

				long curPos = baseOutputStream_.Position;
				baseOutputStream_.Seek(crcPatchPos, SeekOrigin.Begin);
				WriteLeInt((int)curEntry.Crc);

				if (curEntry.LocalHeaderRequiresZip64)
				{
					if (sizePatchPos == -1)
					{
						throw new ZipException("Entry requires zip64 but this has been turned off");
					}

					baseOutputStream_.Seek(sizePatchPos, SeekOrigin.Begin);
					WriteLeLong(curEntry.Size);
					WriteLeLong(curEntry.CompressedSize);
				}
				else
				{
					WriteLeInt((int)curEntry.CompressedSize);
					WriteLeInt((int)curEntry.Size);
				}
				baseOutputStream_.Seek(curPos, SeekOrigin.Begin);
			}

			// Add data descriptor if flagged as required
			if ((curEntry.Flags & 8) != 0)
			{
				WriteLeInt(ZipConstants.DataDescriptorSignature);
				WriteLeInt(unchecked((int)curEntry.Crc));

				if (curEntry.LocalHeaderRequiresZip64)
				{
					WriteLeLong(curEntry.CompressedSize);
					WriteLeLong(curEntry.Size);
					offset += ZipConstants.Zip64DataDescriptorSize;
				}
				else
				{
					WriteLeInt((int)curEntry.CompressedSize);
					WriteLeInt((int)curEntry.Size);
					offset += ZipConstants.DataDescriptorSize;
				}
			}

			entries.Add(curEntry);
			curEntry = null;
		}

		/// <summary>
		/// Initializes encryption keys based on given <paramref name="password"/>.
		/// </summary>
		/// <param name="password">The password.</param>
		private void InitializePassword(string password)
		{
			var pkManaged = new PkzipClassicManaged();
			byte[] key = PkzipClassic.GenerateKeys(ZipStrings.ConvertToArray(password));
			cryptoTransform_ = pkManaged.CreateEncryptor(key, null);
		}

		/// <summary>
		/// Initializes encryption keys based on given password.
		/// </summary>
		private void InitializeAESPassword(ZipEntry entry, string rawPassword,
											out byte[] salt, out byte[] pwdVerifier)
		{
			salt = new byte[entry.AESSaltLen];

			// Salt needs to be cryptographically random, and unique per file
			_aesRnd.GetBytes(salt);

			int blockSize = entry.AESKeySize / 8;   // bits to bytes

			cryptoTransform_ = new ZipAESTransform(rawPassword, salt, blockSize, true);
			pwdVerifier = ((ZipAESTransform)cryptoTransform_).PwdVerifier;
		}

		private void WriteEncryptionHeader(long crcValue)
		{
			offset += ZipConstants.CryptoHeaderSize;

			InitializePassword(Password);

			byte[] cryptBuffer = new byte[ZipConstants.CryptoHeaderSize];
			var rng = new RNGCryptoServiceProvider();
			rng.GetBytes(cryptBuffer);

			cryptBuffer[11] = (byte)(crcValue >> 24);

			EncryptBlock(cryptBuffer, 0, cryptBuffer.Length);
			baseOutputStream_.Write(cryptBuffer, 0, cryptBuffer.Length);
		}

		private static void AddExtraDataAES(ZipEntry entry, ZipExtraData extraData)
		{
			// Vendor Version: AE-1 IS 1. AE-2 is 2. With AE-2 no CRC is required and 0 is stored.
			const int VENDOR_VERSION = 2;
			// Vendor ID is the two ASCII characters "AE".
			const int VENDOR_ID = 0x4541; //not 6965;
			extraData.StartNewEntry();
			// Pack AES extra data field see http://www.winzip.com/aes_info.htm
			//extraData.AddLeShort(7);							// Data size (currently 7)
			extraData.AddLeShort(VENDOR_VERSION);               // 2 = AE-2
			extraData.AddLeShort(VENDOR_ID);                    // "AE"
			extraData.AddData(entry.AESEncryptionStrength);     //  1 = 128, 2 = 192, 3 = 256
			extraData.AddLeShort((int)entry.CompressionMethod); // The actual compression method used to compress the file
			extraData.AddNewEntry(0x9901);
		}

		// Replaces WriteEncryptionHeader for AES
		//
		private void WriteAESHeader(ZipEntry entry)
		{
			byte[] salt;
			byte[] pwdVerifier;
			InitializeAESPassword(entry, Password, out salt, out pwdVerifier);
			// File format for AES:
			// Size (bytes)   Content
			// ------------   -------
			// Variable       Salt value
			// 2              Password verification value
			// Variable       Encrypted file data
			// 10             Authentication code
			//
			// Value in the "compressed size" fields of the local file header and the central directory entry
			// is the total size of all the items listed above. In other words, it is the total size of the
			// salt value, password verification value, encrypted data, and authentication code.
			baseOutputStream_.Write(salt, 0, salt.Length);
			baseOutputStream_.Write(pwdVerifier, 0, pwdVerifier.Length);
		}

		/// <summary>
		/// Writes the given buffer to the current entry.
		/// </summary>
		/// <param name="buffer">The buffer containing data to write.</param>
		/// <param name="offset">The offset of the first byte to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <exception cref="ZipException">Archive size is invalid</exception>
		/// <exception cref="System.InvalidOperationException">No entry is active.</exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (curEntry == null)
			{
				throw new InvalidOperationException("No open entry.");
			}

			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset), "Cannot be negative");
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), "Cannot be negative");
			}

			if ((buffer.Length - offset) < count)
			{
				throw new ArgumentException("Invalid offset/count combination");
			}

			if (curEntry.AESKeySize == 0)
			{
				// Only update CRC if AES is not enabled
				crc.Update(new ArraySegment<byte>(buffer, offset, count));
			}

			size += count;

			switch (curMethod)
			{
				case CompressionMethod.Deflated:
					base.Write(buffer, offset, count);
					break;

				case CompressionMethod.Stored:
					if (Password != null)
					{
						CopyAndEncrypt(buffer, offset, count);
					}
					else
					{
						baseOutputStream_.Write(buffer, offset, count);
					}
					break;
			}
		}

		private void CopyAndEncrypt(byte[] buffer, int offset, int count)
		{
			const int CopyBufferSize = 4096;
			byte[] localBuffer = new byte[CopyBufferSize];
			while (count > 0)
			{
				int bufferCount = (count < CopyBufferSize) ? count : CopyBufferSize;

				Array.Copy(buffer, offset, localBuffer, 0, bufferCount);
				EncryptBlock(localBuffer, 0, bufferCount);
				baseOutputStream_.Write(localBuffer, 0, bufferCount);
				count -= bufferCount;
				offset += bufferCount;
			}
		}

		/// <summary>
		/// Finishes the stream.  This will write the central directory at the
		/// end of the zip file and flush the stream.
		/// </summary>
		/// <remarks>
		/// This is automatically called when the stream is closed.
		/// </remarks>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="ZipException">
		/// Comment exceeds the maximum length<br/>
		/// Entry name exceeds the maximum length
		/// </exception>
		public override void Finish()
		{
			if (entries == null)
			{
				return;
			}

			if (curEntry != null)
			{
				CloseEntry();
			}

			long numEntries = entries.Count;
			long sizeEntries = 0;

			foreach (ZipEntry entry in entries)
			{
				WriteLeInt(ZipConstants.CentralHeaderSignature);
				WriteLeShort((entry.HostSystem << 8) | entry.VersionMadeBy);
				WriteLeShort(entry.Version);
				WriteLeShort(entry.Flags);
				WriteLeShort((short)entry.CompressionMethodForHeader);
				WriteLeInt((int)entry.DosTime);
				WriteLeInt((int)entry.Crc);

				if (entry.IsZip64Forced() ||
					(entry.CompressedSize >= uint.MaxValue))
				{
					WriteLeInt(-1);
				}
				else
				{
					WriteLeInt((int)entry.CompressedSize);
				}

				if (entry.IsZip64Forced() ||
					(entry.Size >= uint.MaxValue))
				{
					WriteLeInt(-1);
				}
				else
				{
					WriteLeInt((int)entry.Size);
				}

				byte[] name = ZipStrings.ConvertToArray(entry.Flags, entry.Name);

				if (name.Length > 0xffff)
				{
					throw new ZipException("Name too long.");
				}

				var ed = new ZipExtraData(entry.ExtraData);

				if (entry.CentralHeaderRequiresZip64)
				{
					ed.StartNewEntry();
					if (entry.IsZip64Forced() ||
						(entry.Size >= 0xffffffff))
					{
						ed.AddLeLong(entry.Size);
					}

					if (entry.IsZip64Forced() ||
						(entry.CompressedSize >= 0xffffffff))
					{
						ed.AddLeLong(entry.CompressedSize);
					}

					if (entry.Offset >= 0xffffffff)
					{
						ed.AddLeLong(entry.Offset);
					}

					ed.AddNewEntry(1);
				}
				else
				{
					ed.Delete(1);
				}

				if (entry.AESKeySize > 0)
				{
					AddExtraDataAES(entry, ed);
				}
				byte[] extra = ed.GetEntryData();

				byte[] entryComment =
					(entry.Comment != null) ?
					ZipStrings.ConvertToArray(entry.Flags, entry.Comment) :
					Empty.Array<byte>();

				if (entryComment.Length > 0xffff)
				{
					throw new ZipException("Comment too long.");
				}

				WriteLeShort(name.Length);
				WriteLeShort(extra.Length);
				WriteLeShort(entryComment.Length);
				WriteLeShort(0);    // disk number
				WriteLeShort(0);    // internal file attributes
									// external file attributes

				if (entry.ExternalFileAttributes != -1)
				{
					WriteLeInt(entry.ExternalFileAttributes);
				}
				else
				{
					if (entry.IsDirectory)
					{                         // mark entry as directory (from nikolam.AT.perfectinfo.com)
						WriteLeInt(16);
					}
					else
					{
						WriteLeInt(0);
					}
				}

				if (entry.Offset >= uint.MaxValue)
				{
					WriteLeInt(-1);
				}
				else
				{
					WriteLeInt((int)entry.Offset);
				}

				if (name.Length > 0)
				{
					baseOutputStream_.Write(name, 0, name.Length);
				}

				if (extra.Length > 0)
				{
					baseOutputStream_.Write(extra, 0, extra.Length);
				}

				if (entryComment.Length > 0)
				{
					baseOutputStream_.Write(entryComment, 0, entryComment.Length);
				}

				sizeEntries += ZipConstants.CentralHeaderBaseSize + name.Length + extra.Length + entryComment.Length;
			}

			using (ZipHelperStream zhs = new ZipHelperStream(baseOutputStream_))
			{
				zhs.WriteEndOfCentralDirectory(numEntries, sizeEntries, offset, zipComment);
			}

			entries = null;
		}

		/// <summary>
		/// Flushes the stream by calling <see cref="DeflaterOutputStream.Flush">Flush</see> on the deflater stream unless
		/// the current compression method is <see cref="CompressionMethod.Stored"/>. Then it flushes the underlying output stream.
		/// </summary>
		public override void Flush()
		{
			if(curMethod == CompressionMethod.Stored)
			{
				baseOutputStream_.Flush();
			} 
			else
			{
				base.Flush();
			}
		}

		#region Instance Fields

		/// <summary>
		/// The entries for the archive.
		/// </summary>
		private List<ZipEntry> entries = new List<ZipEntry>();

		/// <summary>
		/// Used to track the crc of data added to entries.
		/// </summary>
		private Crc32 crc = new Crc32();

		/// <summary>
		/// The current entry being added.
		/// </summary>
		private ZipEntry curEntry;

		private int defaultCompressionLevel = Deflater.DEFAULT_COMPRESSION;

		private CompressionMethod curMethod = CompressionMethod.Deflated;

		/// <summary>
		/// Used to track the size of data for an entry during writing.
		/// </summary>
		private long size;

		/// <summary>
		/// Offset to be recorded for each entry in the central header.
		/// </summary>
		private long offset;

		/// <summary>
		/// Comment for the entire archive recorded in central header.
		/// </summary>
		private byte[] zipComment = Empty.Array<byte>();

		/// <summary>
		/// Flag indicating that header patching is required for the current entry.
		/// </summary>
		private bool patchEntryHeader;

		/// <summary>
		/// Position to patch crc
		/// </summary>
		private long crcPatchPos = -1;

		/// <summary>
		/// Position to patch size.
		/// </summary>
		private long sizePatchPos = -1;

		// Default is dynamic which is not backwards compatible and can cause problems
		// with XP's built in compression which cant read Zip64 archives.
		// However it does avoid the situation were a large file is added and cannot be completed correctly.
		// NOTE: Setting the size for entries before they are added is the best solution!
		private UseZip64 useZip64_ = UseZip64.Dynamic;

		/// <summary>
		/// The password to use when encrypting archive entries.
		/// </summary>
		private string password;

		#endregion Instance Fields

		#region Static Fields

		// Static to help ensure that multiple files within a zip will get different random salt
		private static RandomNumberGenerator _aesRnd = RandomNumberGenerator.Create();

		#endregion Static Fields
	}
}

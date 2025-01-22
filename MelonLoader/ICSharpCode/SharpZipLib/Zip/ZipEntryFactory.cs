using MelonLoader.ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;

namespace MelonLoader.ICSharpCode.SharpZipLib.Zip;

/// <summary>
/// Basic implementation of <see cref="IEntryFactory"></see>
/// </summary>
public class ZipEntryFactory : IEntryFactory
{
    #region Enumerations

    /// <summary>
    /// Defines the possible values to be used for the <see cref="ZipEntry.DateTime"/>.
    /// </summary>
    public enum TimeSetting
    {
        /// <summary>
        /// Use the recorded LastWriteTime value for the file.
        /// </summary>
        LastWriteTime,

        /// <summary>
        /// Use the recorded LastWriteTimeUtc value for the file
        /// </summary>
        LastWriteTimeUtc,

        /// <summary>
        /// Use the recorded CreateTime value for the file.
        /// </summary>
        CreateTime,

        /// <summary>
        /// Use the recorded CreateTimeUtc value for the file.
        /// </summary>
        CreateTimeUtc,

        /// <summary>
        /// Use the recorded LastAccessTime value for the file.
        /// </summary>
        LastAccessTime,

        /// <summary>
        /// Use the recorded LastAccessTimeUtc value for the file.
        /// </summary>
        LastAccessTimeUtc,

        /// <summary>
        /// Use a fixed value.
        /// </summary>
        /// <remarks>The actual <see cref="DateTime"/> value used can be
        /// specified via the <see cref="ZipEntryFactory(DateTime)"/> constructor or
        /// using the <see cref="ZipEntryFactory(TimeSetting)"/> with the setting set
        /// to <see cref="TimeSetting.Fixed"/> which will use the <see cref="DateTime"/> when this class was constructed.
        /// The <see cref="FixedDateTime"/> property can also be used to set this value.</remarks>
        Fixed,
    }

    #endregion Enumerations

    #region Constructors

    /// <summary>
    /// Initialise a new instance of the <see cref="ZipEntryFactory"/> class.
    /// </summary>
    /// <remarks>A default <see cref="INameTransform"/>, and the LastWriteTime for files is used.</remarks>
    public ZipEntryFactory()
    {
        nameTransform_ = new ZipNameTransform();
        isUnicodeText_ = ZipStrings.UseUnicode;
    }

    /// <summary>
    /// Initialise a new instance of <see cref="ZipEntryFactory"/> using the specified <see cref="TimeSetting"/>
    /// </summary>
    /// <param name="timeSetting">The <see cref="TimeSetting">time setting</see> to use when creating <see cref="ZipEntry">Zip entries</see>.</param>
    public ZipEntryFactory(TimeSetting timeSetting) : this()
    {
        timeSetting_ = timeSetting;
    }

    /// <summary>
    /// Initialise a new instance of <see cref="ZipEntryFactory"/> using the specified <see cref="DateTime"/>
    /// </summary>
    /// <param name="time">The time to set all <see cref="ZipEntry.DateTime"/> values to.</param>
    public ZipEntryFactory(DateTime time) : this()
    {
        timeSetting_ = TimeSetting.Fixed;
        FixedDateTime = time;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Get / set the <see cref="INameTransform"/> to be used when creating new <see cref="ZipEntry"/> values.
    /// </summary>
    /// <remarks>
    /// Setting this property to null will cause a default <see cref="ZipNameTransform">name transform</see> to be used.
    /// </remarks>
    public INameTransform NameTransform
    {
        get { return nameTransform_; }
        set
        {
            nameTransform_ = value ?? new ZipNameTransform();
        }
    }

    /// <summary>
    /// Get / set the <see cref="TimeSetting"/> in use.
    /// </summary>
    public TimeSetting Setting
    {
        get { return timeSetting_; }
        set { timeSetting_ = value; }
    }

    /// <summary>
    /// Get / set the <see cref="DateTime"/> value to use when <see cref="Setting"/> is set to <see cref="TimeSetting.Fixed"/>
    /// </summary>
    public DateTime FixedDateTime
    {
        get { return fixedDateTime_; }
        set
        {
            if (value.Year < 1970)
            {
                throw new ArgumentException("Value is too old to be valid", nameof(value));
            }

            fixedDateTime_ = value;
        }
    }

    /// <summary>
    /// A bitmask defining the attributes to be retrieved from the actual file.
    /// </summary>
    /// <remarks>The default is to get all possible attributes from the actual file.</remarks>
    public int GetAttributes
    {
        get { return getAttributes_; }
        set { getAttributes_ = value; }
    }

    /// <summary>
    /// A bitmask defining which attributes are to be set on.
    /// </summary>
    /// <remarks>By default no attributes are set on.</remarks>
    public int SetAttributes
    {
        get { return setAttributes_; }
        set { setAttributes_ = value; }
    }

    /// <summary>
    /// Get set a value indicating whether unidoce text should be set on.
    /// </summary>
    public bool IsUnicodeText
    {
        get { return isUnicodeText_; }
        set { isUnicodeText_ = value; }
    }

    #endregion Properties

    #region IEntryFactory Members

    /// <summary>
    /// Make a new <see cref="ZipEntry"/> for a file.
    /// </summary>
    /// <param name="fileName">The name of the file to create a new entry for.</param>
    /// <returns>Returns a new <see cref="ZipEntry"/> based on the <paramref name="fileName"/>.</returns>
    public ZipEntry MakeFileEntry(string fileName)
    {
        return MakeFileEntry(fileName, null, true);
    }

    /// <summary>
    /// Make a new <see cref="ZipEntry"/> for a file.
    /// </summary>
    /// <param name="fileName">The name of the file to create a new entry for.</param>
    /// <param name="useFileSystem">If true entry detail is retrieved from the file system if the file exists.</param>
    /// <returns>Returns a new <see cref="ZipEntry"/> based on the <paramref name="fileName"/>.</returns>
    public ZipEntry MakeFileEntry(string fileName, bool useFileSystem)
    {
        return MakeFileEntry(fileName, null, useFileSystem);
    }

    /// <summary>
    /// Make a new <see cref="ZipEntry"/> from a name.
    /// </summary>
    /// <param name="fileName">The name of the file to create a new entry for.</param>
    /// <param name="entryName">An alternative name to be used for the new entry. Null if not applicable.</param>
    /// <param name="useFileSystem">If true entry detail is retrieved from the file system if the file exists.</param>
    /// <returns>Returns a new <see cref="ZipEntry"/> based on the <paramref name="fileName"/>.</returns>
    public ZipEntry MakeFileEntry(string fileName, string entryName, bool useFileSystem)
    {
        var result = new ZipEntry(nameTransform_.TransformFile(!string.IsNullOrEmpty(entryName) ? entryName : fileName))
        {
            IsUnicodeText = isUnicodeText_
        };

        var externalAttributes = 0;
        var useAttributes = setAttributes_ != 0;

        FileInfo fi = null;
        if (useFileSystem)
        {
            fi = new FileInfo(fileName);
        }

        if ((fi != null) && fi.Exists)
        {
            result.DateTime = timeSetting_ switch
            {
                TimeSetting.CreateTime => fi.CreationTime,
                TimeSetting.CreateTimeUtc => fi.CreationTimeUtc,
                TimeSetting.LastAccessTime => fi.LastAccessTime,
                TimeSetting.LastAccessTimeUtc => fi.LastAccessTimeUtc,
                TimeSetting.LastWriteTime => fi.LastWriteTime,
                TimeSetting.LastWriteTimeUtc => fi.LastWriteTimeUtc,
                TimeSetting.Fixed => fixedDateTime_,
                _ => throw new ZipException("Unhandled time setting in MakeFileEntry"),
            };
            result.Size = fi.Length;

            useAttributes = true;
            externalAttributes = (int)fi.Attributes & getAttributes_;
        }
        else
        {
            if (timeSetting_ == TimeSetting.Fixed)
            {
                result.DateTime = fixedDateTime_;
            }
        }

        if (useAttributes)
        {
            externalAttributes |= setAttributes_;
            result.ExternalFileAttributes = externalAttributes;
        }

        return result;
    }

    /// <summary>
    /// Make a new <see cref="ZipEntry"></see> for a directory.
    /// </summary>
    /// <param name="directoryName">The raw untransformed name for the new directory</param>
    /// <returns>Returns a new <see cref="ZipEntry"></see> representing a directory.</returns>
    public ZipEntry MakeDirectoryEntry(string directoryName)
    {
        return MakeDirectoryEntry(directoryName, true);
    }

    /// <summary>
    /// Make a new <see cref="ZipEntry"></see> for a directory.
    /// </summary>
    /// <param name="directoryName">The raw untransformed name for the new directory</param>
    /// <param name="useFileSystem">If true entry detail is retrieved from the file system if the file exists.</param>
    /// <returns>Returns a new <see cref="ZipEntry"></see> representing a directory.</returns>
    public ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem)
    {
        var result = new ZipEntry(nameTransform_.TransformDirectory(directoryName))
        {
            IsUnicodeText = isUnicodeText_,
            Size = 0
        };

        var externalAttributes = 0;

        DirectoryInfo di = null;

        if (useFileSystem)
        {
            di = new DirectoryInfo(directoryName);
        }

        if ((di != null) && di.Exists)
        {
            result.DateTime = timeSetting_ switch
            {
                TimeSetting.CreateTime => di.CreationTime,
                TimeSetting.CreateTimeUtc => di.CreationTimeUtc,
                TimeSetting.LastAccessTime => di.LastAccessTime,
                TimeSetting.LastAccessTimeUtc => di.LastAccessTimeUtc,
                TimeSetting.LastWriteTime => di.LastWriteTime,
                TimeSetting.LastWriteTimeUtc => di.LastWriteTimeUtc,
                TimeSetting.Fixed => fixedDateTime_,
                _ => throw new ZipException("Unhandled time setting in MakeDirectoryEntry"),
            };
            externalAttributes = (int)di.Attributes & getAttributes_;
        }
        else
        {
            if (timeSetting_ == TimeSetting.Fixed)
            {
                result.DateTime = fixedDateTime_;
            }
        }

        // Always set directory attribute on.
        externalAttributes |= setAttributes_ | 16;
        result.ExternalFileAttributes = externalAttributes;

        return result;
    }

    #endregion IEntryFactory Members

    #region Instance Fields

    private INameTransform nameTransform_;
    private DateTime fixedDateTime_ = DateTime.Now;
    private TimeSetting timeSetting_ = TimeSetting.LastWriteTime;
    private bool isUnicodeText_;

    private int getAttributes_ = -1;
    private int setAttributes_;

    #endregion Instance Fields
}

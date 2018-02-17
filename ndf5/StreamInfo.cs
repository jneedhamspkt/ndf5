using System;
using System.IO;

namespace ndf5
{
    /// <summary>
    /// Information about the streams provided to to a <see cref="T:ndf5.Hdf5File"/>
    /// </summary>
    public sealed class StreamInfo
    {
        /// <summary>
        /// True if this File can be written to;
        /// </summary>
        bool
            CanWrite;

        /// <summary>
        /// Where the strem was when we were asked to open it
        /// </summary>
        long
            Start;

        /// <summary>
        /// True if new streams may be opened
        /// </summary>
        bool
            CanOpenNewStreams;

        /// <summary>
        /// The original file.
        /// </summary>
        FileInfo
            OriginalFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aCanwrite">If set to <c>true</c> a canwrite.</param>
        /// <param name="aStart">A start.</param>
        public StreamInfo(
            bool aCanwrite,
            long aStart)
        {
            CanWrite = aCanwrite;
            Start = aStart;
            CanOpenNewStreams = false;
            OriginalFile = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aCanwrite">If set to <c>true</c> a canwrite.</param>
        /// <param name="aStart">A start.</param>
        /// <param name="aSourceFile">A source file.</param>
        public StreamInfo(
            bool aCanwrite,
            long aStart,
            FileInfo aSourceFile)
        {
            CanWrite = aCanwrite;
            Start = aStart;
            OriginalFile = aSourceFile;
            CanOpenNewStreams = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aFileInfo">A file info.</param>
        public StreamInfo(
            FileInfo aFileInfo)
        {
            CanWrite = !aFileInfo.IsReadOnly;
            Start = 0L;
            OriginalFile = aFileInfo;
            CanOpenNewStreams = true;
        }
    }
}

using System;
using System.IO;

namespace ndf5.Streams
{
    /// <summary>
    /// Information about the streams provided to to a <see cref="T:ndf5.Hdf5File"/>
    /// </summary>
    internal sealed class StreamInfo
    {
        /// <summary>
        /// True if this File can be written to;
        /// </summary>
        public readonly bool
            CanWrite;

        /// <summary>
        /// The original file.
        /// </summary>
        public readonly FileInfo
            OriginalFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aCanwrite">If set to <c>true</c> a canwrite.</param>
        public StreamInfo(
            bool aCanwrite)
        {
            CanWrite = aCanwrite;
            OriginalFile = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aCanwrite">If set to <c>true</c> a canwrite.</param>
        /// <param name="aSourceFile">A source file.</param>
        public StreamInfo(
            bool aCanwrite,
            FileInfo aSourceFile)
        {
            CanWrite = aCanwrite;
            OriginalFile = aSourceFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.StreamInfo"/> class.
        /// </summary>
        /// <param name="aFileInfo">A file info.</param>
        public StreamInfo(
            FileInfo aFileInfo)
        {
            CanWrite = !aFileInfo.IsReadOnly;
            OriginalFile = aFileInfo;
        }
    }
}

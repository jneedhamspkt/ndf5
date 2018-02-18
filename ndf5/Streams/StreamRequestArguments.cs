using System;
namespace ndf5.Streams
{
    public struct StreamRequestArguments
    {
        /// <summary>
        /// True if the stream being requested needs to be able to write
        /// </summary>
        public bool
            WriteAccessDesired;

        /// <summary>
        /// Max Time to block while getting the stream;
        /// </summary>
        public TimeSpan?
            AccessTimeout;

        public StreamRequestArguments(
            bool aWriteDesired = false,
            TimeSpan? aAccessTimeout = null)
        {
            WriteAccessDesired = aWriteDesired;
            AccessTimeout = aAccessTimeout;
        }
    }
}

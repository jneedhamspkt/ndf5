using System;
namespace ndf5.Streams
{
    /// <summary>
    /// Interface for chaning the length of  stream
    /// </summary>
    public interface IStreamResizer
    {
        void SetLength(long value);
    }
}

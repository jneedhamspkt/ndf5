using System;
namespace ndf5.ObjectHeaders
{
    /// <summary>
    /// Interface for a class that will read objcet headers from an HDF5 file.
    /// </summary>
    public interface IObjectHeaderReader
    {
        /// <summary>
        /// Reads the header found at <c>aLocation</c>
        /// </summary>
        /// <returns>The read Object Header.</returns>
        /// <param name="aLocation">A location.</param>
        IObjectHeader ReadHeaderAt(long aLocation);
    }
}

using System;
namespace ndf5.Objects
{
    /// <summary>
    /// Data space type of a Data Set object
    /// </summary>
    /// <remarks>
    /// Value annotaions and values orignated from <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#DataspaceMessage"/>
    /// </remarks>
    public enum DataSpaceType : byte
    {
        /// <summary>
        /// A scalar dataspace; in other words, a dataspace with a single, 
        /// dimensionless element.
        /// </summary>
        Scaler = 0,
        /// <summary>
        /// A simple dataspace; in other words, a dataspace with a rank greater 
        /// than 0 and an appropriate number of dimensions.
        /// </summary>
        Simple = 1,
        /// <summary>
        /// A null dataspace; in other words, a dataspace with no elements.
        /// </summary>
        Null = 2,
    }
}

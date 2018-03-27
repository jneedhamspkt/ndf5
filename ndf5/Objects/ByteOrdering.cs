using System;
namespace ndf5.Objects
{
    public enum ByteOrdering
    {
        /// <summary>
        /// Byte ordering is from least to most significant
        /// </summary>
        BigEndian,
        /// <summary>
        /// Byte ordering is from Most to Least significant
        /// </summary>
        LittleEndian,
        /// <summary>
        /// Only applies to floating point values.
        /// </summary>
        /// <seealso cref="https://nssdc.gsfc.nasa.gov/nssdc/formats/VAXFloatingPoint.htm"/>
        VAXOrder

    }
}

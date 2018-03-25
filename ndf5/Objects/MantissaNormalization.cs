using System;
namespace ndf5.Objects
{
    /// <summary>
    /// Mantissa normalization, used byt <see cref="FloatingPointDataType"/>
    /// </summary>
    public enum MantissaNormalization : byte
    {
        /// <summary>
        /// No normalization
        /// </summary>
        None = 0,

        /// <summary>
        /// The most significant bit of the mantissa is always set (except for 0.0).
        /// </summary>
        MsbSetAndStored,

        /// <summary>
        /// The most significant bit of the mantissa is not stored, but is implied to be set.
        /// </summary>
        MsbSetNotStorred
    }
}

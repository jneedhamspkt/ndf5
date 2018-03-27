using System;
namespace ndf5.Objects
{
    /// <summary>
    /// Determines the type of padding to use for the string
    /// </summary>
    public enum StringPadding : byte
    {
        /// <summary>
        /// A zero byte marks the end of the string and is guaranteed to be
        /// present after converting a long string to a short string. When 
        /// converting a short string to a long string the value is padded with 
        /// additional null characters as necessary.
        /// </summary>
        NullTerminate = 0,
        /// <summary>
        /// Null Pad: Null characters are added to the end of the value during 
        /// conversions from short values to long values but conversion in the 
        /// opposite direction simply truncates the value.
        /// </summary>
        NullPad = 1,
        /// <summary>
        /// Space characters are added to the end of the value during 
        /// conversions from short values to long values but conversion in the 
        /// opposite direction simply truncates the value. This is the Fortran 
        /// representation of the string.
        /// </summary>
        SpacePad = 2,
    }
}

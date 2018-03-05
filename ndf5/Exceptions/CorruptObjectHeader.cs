using System;
namespace ndf5.Exceptions
{
    /// <summary>
    /// Exeption fired upon encountering what apears to be a corrupt object header in 
    /// an HDF5 file
    /// </summary>
    public class CorruptObjectHeader : System.IO.IOException
    {
        public CorruptObjectHeader(
            String aMessage = null,
            Exception aInternalException = null) : base(
                aMessage,
                aInternalException)
        { }
    }
}

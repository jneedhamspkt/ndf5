using System;
namespace ndf5.Exceptions
{
    /// <summary>
    /// Exception fired when a request requires a feaure this libary does not support;
    /// </summary>
    public class Hdf5UnsupportedFeature : Exception
    {
        internal Hdf5UnsupportedFeature(
            String aMessage = null, 
            Exception aInternalException = null):base(aMessage,aInternalException)
        {
        }
    }
}

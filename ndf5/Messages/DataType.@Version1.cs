using System;

using ndf5.Streams;

namespace ndf5.Messages
{
    partial class Datatype
    {
        private class Version1 : Datatype
        {
            public static Datatype Read(
                Header aHeader,
                Hdf5Reader aReader,
                long? aLocalMessageSize,
                out long aBytes)
            {
                throw new NotImplementedException();
            }
        }
    }
}

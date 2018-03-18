using System;
using ndf5.Streams;

namespace ndf5.Messages
{
    public partial class Datatype : Message
    {
        

        public Datatype() : base(MessageType.Datatype)
        {
        }

        internal static Dataspace Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using ndf5.Streams;

namespace ndf5.Messages
{
    public abstract partial class Datatype : Message
    {
        

        public Datatype() : base(MessageType.Datatype)
        {
        }

        internal static Datatype Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            Header 
            fHeader = Header.Read(aReader);   
            switch(fHeader.Verson)
            {
                case Version.Version1:
                    return Version1.Read(
                        aReader, 
                        aLocalMessageSize, 
                        out aBytes);
            }

            throw new NotImplementedException();
        }
    }
}

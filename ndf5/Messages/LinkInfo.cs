using System;
using ndf5.Streams;

namespace ndf5.Messages
{
    public class LinkInfo : Message
    {
        const byte
            mcCurrentVersion = 0;

        [Flags]
        enum Flags
        {
            None = 0,
            CreationOrderTracked = 1,
            CreationOrderIndexed = 2
        }

        public LinkInfo() : base(MessageType.LinkInfo)
        {
        }

        internal static Message Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            int 
                fReadBytes = 4; //Always Read atleast 4
            byte 
                fVersion = (byte)aReader.ReadByte(),
                fFlags = (byte)aReader.ReadByte();
            aReader.ReadUInt16(); //Reserved

            if (fVersion != mcCurrentVersion)
                throw new Exceptions.UnknownMessageVersion<LinkInfo>(fVersion);

            throw new NotImplementedException();
        }
    }
}

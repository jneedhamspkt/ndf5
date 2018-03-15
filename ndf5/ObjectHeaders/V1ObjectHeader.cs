using System;
using System.Collections.Generic;

using ndf5.Messages;
using ndf5.Streams;
namespace ndf5.ObjectHeaders
{
    internal static class V1ObjectHeader
    {
        public static IObjectHeader Read(byte[] aHeadBytes, Hdf5Reader aReader)
        {
            List<Message>
                fMessages = new List<Message>();
            ushort
                fMessageCount = (ushort)(aHeadBytes[2] + (aHeadBytes[3] << 8)); //Little endian read of already consumed data
            uint
                fObjectReferenceCount = aReader.ReadUInt32(),
                fObjectHeaderSize = aReader.ReadUInt32();
            //long 
            //    fTerminate = aReader.Position + fObjectHeaderSize;

            //while(aReader.Position <= fTerminate)
            //{
                
            //}

            throw new NotImplementedException();
        }
    }
}

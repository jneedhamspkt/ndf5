using System;
using System.IO;

using ndf5.Exceptions;
using ndf5.Streams;

namespace ndf5.Messages
{
    partial class Datatype
    {
        protected class Header
        {
            public const int
                HeaderSize = 8;

            public readonly Version
                Verson;

            public readonly Class
                Class;

            public readonly uint
                Flags;

            public readonly uint
                Size;


            public Header(
                Version aVersion,
                Class aClass,
                uint aFlags,
                uint aSize)
            {
                Verson = aVersion;
                Class = aClass;
                Flags = aFlags;
                Size = aSize;
            }

            public static Header Read(Hdf5Reader aStream)
            {
                byte[]
                    fBuffer = new byte[HeaderSize];
                if (HeaderSize != aStream.Read(fBuffer, 0, 4))
                    throw new EndOfStreamException();

                byte 
                    fVersion = (byte)((fBuffer[0] >> 4) & 0xF);

                if (fVersion < (byte)Version.Version1 || fVersion > (byte)Version.Version3)
                    throw new UnknownMessageVersion<Datatype>(fVersion);

                byte 
                    fClass = (byte)(fBuffer[0] & 0xF);

                if (fClass > (byte)Class.Array)
                    throw new InvalidDataException($"Unkown Datatype Class {fClass}");
                uint
                    fFlags = (uint)(fBuffer[1] |
                        fBuffer[2] << 8 |
                        fBuffer[3] << 16);
                uint
                    fSize = aStream.ReadUInt32();
                
                return new Header(
                    (Version)fVersion,
                    (Class)fClass,
                    fFlags,
                    fSize);
            }
        }
    }
}

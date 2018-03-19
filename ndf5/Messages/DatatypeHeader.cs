using System;
using System.IO;

using ndf5.Exceptions;
using ndf5.Streams;

namespace ndf5.Messages
{
    class DatatypeHeader : IDatatypeHeader
    {
        public const int
            HeaderSize = 8;

        public DatatypeVersion
            Version { get; }

        public DatatypeClass
            Class { get; }

        public readonly uint
            Flags;

        public uint
            Size{ get; }


        public DatatypeHeader(
            DatatypeVersion aVersion,
            DatatypeClass aClass,
            uint aFlags,
            uint aSize)
        {
            Version = aVersion;
            Class = aClass;
            Flags = aFlags;
            Size = aSize;
        }

        public static DatatypeHeader Read(Hdf5Reader aStream)
        {
            byte[]
                fBuffer = new byte[HeaderSize];
            if (HeaderSize != aStream.Read(fBuffer, 0, 4))
                throw new EndOfStreamException();

            byte 
                fVersion = (byte)((fBuffer[0] >> 4) & 0xF);

            if (fVersion < (byte)DatatypeVersion.Version1 || fVersion > (byte)DatatypeVersion.Version3)
                throw new UnknownMessageVersion<Datatype>(fVersion);

            byte 
                fClass = (byte)(fBuffer[0] & 0xF);

            if (fClass > (byte)DatatypeClass.Array)
                throw new InvalidDataException($"Unkown Datatype Class {fClass}");
            uint
                fFlags = (uint)(fBuffer[1] |
                    fBuffer[2] << 8 |
                    fBuffer[3] << 16);
            uint
                fSize = aStream.ReadUInt32();
            
            return new DatatypeHeader(
                (DatatypeVersion)fVersion,
                (DatatypeClass)fClass,
                fFlags,
                fSize);
        }
    }
}

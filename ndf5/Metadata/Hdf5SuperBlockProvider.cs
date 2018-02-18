using System;
using System.IO;
using ndf5.Streams;
using ndf5.Exceptions;

namespace ndf5.Metadata
{
    public class Hdf5SuperBlockProvider : ISuperBlockProvider
    {
        private readonly IStreamProvider 
            mrStreamProvider;
        FormatSignatureAndVersionInfo 
            mrFormatSignatureAndVersionInfo;

        internal Hdf5SuperBlockProvider(
            IStreamProvider aStreamProvider,
            FormatSignatureAndVersionInfo aFormatSignatureAndVersionInfo)
        {
            mrStreamProvider = aStreamProvider;
            mrFormatSignatureAndVersionInfo = aFormatSignatureAndVersionInfo;
        }

        private class SuperBlockObj : ISuperBlock
        {
            public SuperBlockObj(long aLocationAddress)
            {
                LocationAddress = aLocationAddress;
            }

            public long LocationAddress { get; private set; }

            public byte SuperBlockVersion { get; set; } = 0;

            public byte FreeSpaceStorageVersion { get; set; } = 0;

            public byte RootGroupSymbolTableVersion { get; set; } = 0;

            public byte SharedHeaderMessageFormatVersion { get; set; } = 0;

            public byte SizeOfOffsets { get; set; }

            public byte SizeOfLengths { get; set; }

            public ushort GroupLeafNodeK { get; set; }

            public ushort GroupInternalNodeK { get; set; }

            public ushort IndexedStorageInternalNodeK { get; set; }

            public long BaseAddress { get; set; }

            public long FileFreespaceInfoAddress { get; set; }

            public long EndOfFileAddress { get; set; }

            public long DriverInformationBlockAddress { get; set; }
        }

        public ISuperBlock SuperBlock
        {
            get
            {
                SuperBlockObj
                    fStartingObject = new SuperBlockObj(
                        mrFormatSignatureAndVersionInfo.LocationAddress);

                switch(mrFormatSignatureAndVersionInfo.SuperBlockVersion)
                {
                    case 0:
                        return ParseV0orV1(fStartingObject, false);
                    case 1:
                        return ParseV0orV1(fStartingObject, true);
                    case 2:
                        return ParseV2(fStartingObject);
                    case 3:
                        return ParseV3(fStartingObject);
                    default:
                        throw new Hdf5UnsupportedFeature(
                            $"Unsupported superblock version: {mrFormatSignatureAndVersionInfo.SuperBlockVersion}");
                }
            }
        }



        private ISuperBlock ParseV0orV1(SuperBlockObj aContainer, bool aIsV1)
        {
            using (Stream fStream = mrStreamProvider.GetReadonlyStream())
            {
                fStream.Seek(
                    aContainer.LocationAddress + FormatSignatureAndVersionInfo.Length,
                    SeekOrigin.Begin);

                const byte
                    fcHeaderBytes = 15;
                byte[]
                    fHeadbuffer = new byte[fcHeaderBytes];

                if(fcHeaderBytes != fStream.Read(fHeadbuffer, 0,fcHeaderBytes))
                    throw new EndOfStreamException("Could not read Superblock");

                aContainer.FreeSpaceStorageVersion = fHeadbuffer[0];
                aContainer.RootGroupSymbolTableVersion = fHeadbuffer[1];
                if (fHeadbuffer[2] != 0)
                    throw new InvalidDataException("Reserved byte expected to be zero");
                aContainer.SharedHeaderMessageFormatVersion = fHeadbuffer[3];
                aContainer.SizeOfOffsets = fHeadbuffer[4];
                aContainer.SizeOfLengths = fHeadbuffer[5];
                if (fHeadbuffer[6] != 0)
                    throw new InvalidDataException("Reserved byte expected to be zero");

                aContainer.GroupLeafNodeK = BitConverter.ToUInt16(fHeadbuffer,7);
                aContainer.GroupInternalNodeK = BitConverter.ToUInt16(fHeadbuffer, 9);

                if (BitConverter.ToUInt32(fHeadbuffer, 10) != 0)
                    throw new InvalidDataException("File consistincy flags expected to be zero");


                if(aIsV1)
                {
                    const int   
                        fcV1FeildBytes = 4;
                    byte[]
                        fV1Buffer = new byte[fcV1FeildBytes];

                    if (fcV1FeildBytes != fStream.Read(fHeadbuffer, 0, fcV1FeildBytes))
                        throw new EndOfStreamException("Could not read Superblock");

                    aContainer.IndexedStorageInternalNodeK = BitConverter.ToUInt16(fV1Buffer, 0);

                    if (BitConverter.ToUInt32(fHeadbuffer, 2) != 0)
                        throw new InvalidDataException("Reserved bytes expected to be zero");
                }





                aContainer.FreeSpaceStorageVersion = (byte)fStream.ReadByte();
            }

            return aContainer;
        }

        private ISuperBlock ParseV2(SuperBlockObj aContainer)
        {
            using (Stream fStream = mrStreamProvider.GetReadonlyStream())
            {
                fStream.Seek(aContainer.LocationAddress, SeekOrigin.Begin);

            }
            return aContainer;
        }

        private ISuperBlock ParseV3(SuperBlockObj aContainer)
        {
            using (Stream fStream = mrStreamProvider.GetReadonlyStream())
            {
                fStream.Seek(aContainer.LocationAddress, SeekOrigin.Begin);

            }
            return aContainer;
        }


    }
}

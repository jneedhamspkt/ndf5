using System;
using System.IO;
using ndf5.Streams;
using ndf5.Exceptions;

namespace ndf5.Metadata
{
    public class SuperBlockProvider : ISuperBlockProvider
    {
        private readonly IStreamProvider 
            mrStreamProvider;
        FormatSignatureAndVersionInfo 
            mrFormatSignatureAndVersionInfo;

        public SuperBlockProvider(
            IStreamProvider aStreamProvider,
            FormatSignatureAndVersionInfo aFormatSignatureAndVersionInfo)
        {
            mrStreamProvider = aStreamProvider;
            mrFormatSignatureAndVersionInfo = aFormatSignatureAndVersionInfo;
        }

        private class SuperBlockObj : ISuperBlock
        {
            public SuperBlockObj(
                long aLocationAddress, 
                byte aVersion)
            {
                LocationAddress = aLocationAddress;
                BaseAddress = aLocationAddress;
                SuperBlockVersion = aVersion;
            }

            public long LocationAddress { get; private set; }

            public byte SuperBlockVersion { get; private set; }

            public byte FreeSpaceStorageVersion { get; set; } = 0;

            public byte RootGroupSymbolTableVersion { get; set; } = 0;

            public byte SharedHeaderMessageFormatVersion { get; set; } = 0;

            public byte SizeOfOffsets { get; set; }

            public byte SizeOfLengths { get; set; }

            public FileConsistencyFlags? FileConsistencyFlags { get; set; }

            public ushort GroupLeafNodeK { get; set; }

            public ushort GroupInternalNodeK { get; set; }

            public ushort IndexedStorageInternalNodeK { get; set; }

            public long BaseAddress { get; set; }

            public long? FileFreespaceInfoAddress { get; set; }

            public long EndOfFileAddress { get; set; }

            public long? DriverInformationBlockAddress { get; set; }

            public long? RootGroupAddress { get; set; }

            public object RootGroupSymbolTableEntry { get; set; }
        }

        public ISuperBlock SuperBlock
        {
            get
            {
                SuperBlockObj
                    fStartingObject = new SuperBlockObj(
                        mrFormatSignatureAndVersionInfo.LocationAddress,
                    mrFormatSignatureAndVersionInfo.SuperBlockVersion);

                switch(mrFormatSignatureAndVersionInfo.SuperBlockVersion)
                {
                    case 0:
                        return ParseV0orV1(fStartingObject, false);
                    case 1:
                        return ParseV0orV1(fStartingObject, true);
                    case 2:
                        return ParseV2orV3(fStartingObject, false);
                    case 3:
                        return ParseV2orV3(fStartingObject, true);
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

                if(aIsV1)
                {
                    const int   
                        fcV1FeildBytes = 4;
                    byte[]
                        fV1Buffer = new byte[fcV1FeildBytes];

                    if (fcV1FeildBytes != fStream.Read(fV1Buffer, 0, fcV1FeildBytes))
                        throw new EndOfStreamException("Could not read Superblock");

                    aContainer.IndexedStorageInternalNodeK = BitConverter.ToUInt16(fV1Buffer, 0);

                    if (BitConverter.ToUInt16(fV1Buffer, 2) != 0)
                        throw new InvalidDataException("Reserved bytes expected to be zero");
                }
                aContainer.BaseAddress = aContainer.LocationAddress;
                using(Hdf5Reader fReader = new Hdf5Reader(fStream, aContainer))
                {
                    long?
                        fBaseAddress = fReader.ReadOffset(),
                        fFreeSpaceAddress = fReader.ReadOffset(),
                        fEndOfFileAddress = fReader.ReadOffset(),
                        fDirverInformationBlockAddress = fReader.ReadOffset();


                    if (!fBaseAddress.HasValue)
                        throw new InvalidDataException("No base adddress Specified");
                    aContainer.BaseAddress = fBaseAddress.Value;
                    aContainer.FileFreespaceInfoAddress = fFreeSpaceAddress;
                    if (!fEndOfFileAddress.HasValue)
                        throw new InvalidDataException("No End Of file Adddress Specified");
                    aContainer.EndOfFileAddress = fEndOfFileAddress.Value;
                    aContainer.DriverInformationBlockAddress = fDirverInformationBlockAddress;

                }
            }

            return aContainer;
        }

        private ISuperBlock ParseV2orV3(SuperBlockObj aContainer, bool aIsV3)
        {
            using (Stream fStream = mrStreamProvider.GetReadonlyStream())
            {
               
                fStream.Seek(
                    aContainer.LocationAddress + FormatSignatureAndVersionInfo.Length,
                    SeekOrigin.Begin);

                const byte
                    fcHeaderBytes = 3;
                byte[]
                    fHeadbuffer = new byte[fcHeaderBytes];
                if (fcHeaderBytes != fStream.Read(fHeadbuffer, 0, fcHeaderBytes))
                    throw new EndOfStreamException("Could not read Superblock");


                aContainer.SizeOfOffsets = fHeadbuffer[0];
                aContainer.SizeOfLengths = fHeadbuffer[1];
                byte
                    fFlags = fHeadbuffer[2];

                if ((fFlags & ~((int)(FileConsistencyFlags.SwmrAccessEngaged | FileConsistencyFlags.WriteAccessOpen))) != 0)
                    throw new InvalidDataException($"Unexpected {nameof(FileConsistencyFlags)}: 0x{fFlags:X}");

                if (aIsV3)
                    aContainer.FileConsistencyFlags = (FileConsistencyFlags)fFlags;

                using (Hdf5Reader fReader = new Hdf5Reader(fStream, aContainer))
                {
                    long?
                        fBaseAddress = fReader.ReadOffset(),
                    fSuperBlockExtensionAddress = fReader.ReadOffset(),
                    fEndOfFileAddress = fReader.ReadOffset(),
                    fRootGroupAddress = fReader.ReadOffset();
                
                    if (!fBaseAddress.HasValue)
                        throw new InvalidDataException("No base adddress Specified");
                    aContainer.BaseAddress = fBaseAddress.Value;

                    //TODO: Handle SuperVlock Extensions

                    if (!fEndOfFileAddress.HasValue)
                        throw new InvalidDataException("No End Of file Adddress Specified");
                    aContainer.EndOfFileAddress = fEndOfFileAddress.Value;

                    if (!fRootGroupAddress.HasValue)
                        throw new InvalidDataException("No Root Group Specified");
                    aContainer.RootGroupAddress = fRootGroupAddress;
                }

            }
            return aContainer;
        }
    }
}

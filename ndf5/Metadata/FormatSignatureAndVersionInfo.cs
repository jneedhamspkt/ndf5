using System;
using System.IO;
using System.Linq;

namespace ndf5.Metadata
{
    /// <summary>
    /// Format signature and version.
    /// </summary>
    public class FormatSignatureAndVersionInfo
    {
        private const int
            mcBlockLength = 12,
            mcFormatSignatureLength = 8;

        private static readonly byte[]
        FormatSignature = { 137,72,68,70,13,10,26,10 };

        /// <summary>
        /// The super block version.
        /// </summary>
        public readonly byte
            SuperBlockVersion;

        /// <summary>
        /// The free space storage version.
        /// </summary>
        public readonly byte
            FreeSpaceStorageVersion;

        /// <summary>
        /// The root group symbol table version.
        /// </summary>
        public readonly byte
            RootGroupSymbolTableVersion;

        public FormatSignatureAndVersionInfo(
            byte aSuperBlockVersion,
            byte aFreeSpaceStorageVersion,
            byte aRootGroupSymbolTableVersion)
        {
            SuperBlockVersion = aSuperBlockVersion;
            FreeSpaceStorageVersion = aFreeSpaceStorageVersion;
            RootGroupSymbolTableVersion = aRootGroupSymbolTableVersion;
        }

        public byte[] AsBytes => FormatSignature
            .Concat(new byte[]{
                SuperBlockVersion,
                FreeSpaceStorageVersion,
                RootGroupSymbolTableVersion,
                0})
            .ToArray();

        public static bool TryRead(
            Stream aInputStream,
            out FormatSignatureAndVersionInfo aParsed)
        {
            byte[]
                fReadBuffer = new byte[mcBlockLength];

            if (mcBlockLength != aInputStream.Read(
                fReadBuffer,
                0,
                mcBlockLength))
            {
                aParsed = null;
                return false;
            }
            bool
                fGoodSignature = true;
            for (int fiByte = 0; fiByte < mcFormatSignatureLength; ++fiByte)
            {
                fGoodSignature &= fReadBuffer[fiByte] == FormatSignature[fiByte];
            }
            //Check Reserved byte
            fGoodSignature &= fReadBuffer[11] == 0;

            if(!fGoodSignature)
            {
                aParsed = null;
                return false;
            }

            aParsed = new FormatSignatureAndVersionInfo(
                fReadBuffer[8],
                fReadBuffer[9],
                fReadBuffer[10]);

            return true;
        }
    }
}

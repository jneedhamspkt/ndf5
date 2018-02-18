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
        /// The location of this FormatSignatureAndVersionInfo in the file
        /// </summary>
        public long
            LocationAddress;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Metadata.FormatSignatureAndVersionInfo"/> class.
        /// </summary>
        /// <param name="aSuperBlockVersion">A super block version.</param>
        /// <param name="aFreeSpaceStorageVersion">A free space storage version.</param>
        /// <param name="aRootGroupSymbolTableVersion">A root group symbol table version.</param>
        /// <param name="aLocationAddress">A location address where this was parseed</param>
        public FormatSignatureAndVersionInfo(
            byte aSuperBlockVersion,
            byte aFreeSpaceStorageVersion,
            byte aRootGroupSymbolTableVersion,
            long aLocationAddress = 0)
        {
            SuperBlockVersion = aSuperBlockVersion;
            FreeSpaceStorageVersion = aFreeSpaceStorageVersion;
            RootGroupSymbolTableVersion = aRootGroupSymbolTableVersion;
            LocationAddress = aLocationAddress;
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
            //Record where we are
            long
                fLocationAddress = aInputStream.Position;

            //Do the Read
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

            // Check the signature 
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
                fReadBuffer[10], 
                fLocationAddress);

            return true;
        }
    }
}

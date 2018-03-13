using System;
using System.IO;
namespace ndf5.Messages
{
    partial class Dataspace
    {
        private class DSHeader
        {
            /// <summary>
            /// The size in bytes of this header
            /// </summary>
            public const int
                Size = 4;

            /// <summary>
            /// The version of this DataSpaceMessage
            /// </summary>
            public readonly byte
                Version;

            /// <summary>
            /// The dimensionality of this DataSpaceMessage
            /// </summary>
            public readonly byte
                Dimensionality;

            /// <summary>
            /// The flags of this DataSpaceMessage
            /// </summary>
            public readonly byte
                Flags;

            /// <summary>
            /// The type of this DataSpaceMessage (V1 only)
            /// </summary>
            public readonly byte
                Type;

            public DSHeader(
                byte aVersion,
                byte aDimensionality,
                byte aFlags,
                byte aType)
            {
                Version = aVersion;
                Dimensionality = aDimensionality;
                Flags = aFlags;
                Type = aType;
            }

            public static DSHeader Read(Stream aStream)
            {
                byte[]
                    fBuffer = new byte[Size];
                if (Size != aStream.Read(fBuffer, 0, Size))
                    throw new EndOfStreamException();
                return new DSHeader(
                    fBuffer[0],
                    fBuffer[1],
                    fBuffer[2],
                    fBuffer[3]);
            }
        }
    }
}

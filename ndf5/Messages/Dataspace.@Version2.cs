using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;
namespace ndf5.Messages
{
    partial class Dataspace
    {
        private class Version2 : Dataspace
        {
            [Flags]
            private enum Flags : byte
            {
                HasMax = 1,
            }

            public Version2(
                DataSpaceType aDataSpaceType,
                IReadOnlyList<Dimension> aDimensions) : base(aDataSpaceType, aDimensions)
            {
                //Nothing to do
            }

            public static Version2 ReadAfterHeader(
                DSHeader aHeader,
                Hdf5Reader aReader,
                long? aLocalMessageSize,
                out long aBytes)
            {
                int
                    fDimCount = aHeader.Dimensionality;
                long
                    fReadlength = (fDimCount * aReader.mrSuperBlock.SizeOfLengths);
                if (aLocalMessageSize.HasValue && aLocalMessageSize.Value < fReadlength)
                    throw new ArgumentException("Specified Local Message Size not long enough");
                DataSpaceType
                    fType;
                switch (aHeader.Type)
                {
                    case (byte)DataSpaceType.Scaler:
                    case (byte)DataSpaceType.Simple:
                    case (byte)DataSpaceType.Null:
                        fType = (DataSpaceType)aHeader.Type;
                        break;
                    default:
                        throw new System.IO.InvalidDataException("Unknown DataSpaceType");
                }

                Flags
                    fHeaderFlags = (Flags)aHeader.Flags;
                aBytes = fReadlength;

                Dimension[] 
                    fDimensions = ReadDimensions(
                        aReader,
                        fHeaderFlags.HasFlag(Flags.HasMax),
                        fDimCount);

                return new Version2(fType, fDimensions);
            }
        }
    }
}

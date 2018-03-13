using System;
using System.Collections.Generic;
using System.Linq;

using ndf5.Streams;
using ndf5.Objects;

namespace ndf5.Messages
{
    partial class Dataspace
    {    
        private class Version1 : Dataspace
        {
            [Flags]
            private enum Flags : byte
            {
                HasMax = 1,
                HasPermutaions = 2
            }

            public Version1(
                DataSpaceType aDataSpaceType,
                IReadOnlyList<Dimension> aDimensions) : base(aDataSpaceType, aDimensions)
            {
                //Nothing to do.
            }

            public static Version1 ReadAfterHeader(
                DSHeader aHeader,
                Hdf5Reader aReader,
                long? aLocalMessageSize,
                out long aBytes)
            {
                int
                    fDimCount = aHeader.Dimensionality,
                    fBlockLength = (fDimCount * aReader.mrSuperBlock.SizeOfLengths);
                long
                    fReadlength = (fDimCount * aReader.mrSuperBlock.SizeOfLengths) + sizeof(UInt32);

                Flags
                    fHeaderFlags = (Flags)aHeader.Flags;

                if (fHeaderFlags.HasFlag(Flags.HasPermutaions))
                    throw new NotSupportedException("Permutation index not supported");
                
                bool
                    fHasMax = fHeaderFlags.HasFlag(Flags.HasMax);
                if (fHasMax)
                    fReadlength += fBlockLength;

                if (aLocalMessageSize.HasValue && aLocalMessageSize.Value < fReadlength)
                    throw new ArgumentException("Specified Local Message Size not long enough");

                aReader.ReadUInt32(); //Read reserved byte


                aBytes = fReadlength;


                Dimension[] fDimensions = ReadDimensions(
                    aReader, 
                    fHasMax, 
                    fDimCount);
                
                return new Version1(DataSpaceType.Simple, fDimensions);
            }


        }
    }
}

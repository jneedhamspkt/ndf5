using System;
using System.Collections.Generic;
using System.Linq;

using ndf5.Streams;
using ndf5.Objects;

namespace ndf5.Messages
{
    partial class DataspaceMessage
    {
        [Flags]
        private enum Flags : byte
        {
            HasMax = 1,
            HasPermutaions = 2
        }
            
        private class Version1 : DataspaceMessage
        {
            public Version1(
                DataSpaceType aDataSpaceType,
                IReadOnlyList<Dimension> aDimensions) : base(aDataSpaceType, aDimensions)
            {
                
            }

            public static Version1 ReadAfterHeader(
                DSHeader aHeader,
                Hdf5Reader aReader,
                long? aLocalMessageSize,
                out long aBytes)
            {
                long 
                    fReadlength = (aHeader.Dimensionality * aReader.mrSuperBlock.SizeOfLengths) + sizeof(UInt32);
                Flags
                    fHeaderFlags = (Flags)aHeader.Flags;
                if (fHeaderFlags.HasFlag(Flags.HasPermutaions))
                    throw new NotSupportedException("Permutation index not supported");
                aBytes = fReadlength;

                int 
                    fDimCount = aHeader.Dimensionality;

                Dimension[] 
                    fDimensions;

                long[]
                    fSizes = Enumerable
                        .Range(0, fDimCount)
                        .Select(a => 
                        {
                            long? 
                                fLength = aReader.ReadLength();
                            if (!fLength.HasValue)
                                throw new System.IO.InvalidDataException(
                                    "Dimension lengths must have real values");
                            return fLength.Value;
                        })
                        .ToArray();
                if (fHeaderFlags.HasFlag(Flags.HasMax))
                {
                    fDimensions = fSizes.Zip(
                        Enumerable.Range(0, fDimCount)
                            .Select(a => aReader.ReadLength()),
                        (aLength, aMax) => new Dimension(aLength, aMax))
                        .ToArray();
                                        
                }
                else
                {
                    fDimensions = fSizes.Select(a => new Dimension(a)).ToArray();
                }
                return new Version1(DataSpaceType.Simple, fDimensions);
            }
        }
    }
}

using System;
using System.Collections.Generic;

using ndf5.Streams;
using ndf5.Exceptions;
using ndf5.Objects;
using System.Linq;

namespace ndf5.Messages
{
    /// <summary>
    /// Message class for message objects marked as Data Space type
    /// </summary>
    public partial class DataspaceMessage : Message
    {
        /// <summary>
        /// Gets the type of the data space (Usually simple).
        /// </summary>
        /// <value>The type of the data space.</value>
        public DataSpaceType 
            DataSpaceType { get; }

        /// <summary>
        /// Gets the dimensions of the dataspace this message decribes.
        /// </summary>
        /// <value>The dimensions.</value>
        public IReadOnlyList<Dimension>
            Dimensions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.DataspaceMessage"/> class.
        /// </summary>
        /// <param name="aDataSpaceType">A data space type.</param>
        /// <param name="aDimensions">A n ordered list of this dataspace's dimensions.</param>
        public DataspaceMessage(
            DataSpaceType aDataSpaceType,
            IReadOnlyList<Dimension> aDimensions) : base(MessageType.Dataspace)
        {
            DataSpaceType = aDataSpaceType;
            Dimensions = aDimensions;
        }

        internal static DataspaceMessage Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize, 
            out long aBytes)
        {
            DSHeader
                fHeader = DSHeader.Read(aReader);
            long
                fMessageBodySize;
            DataspaceMessage
                fToRetrun;
            long?
                fBodySize = aLocalMessageSize.HasValue
                    ? (long?)aLocalMessageSize.Value - DSHeader.Size
                    : null;
            switch(fHeader.Version)
            {
                case 0:
                    fToRetrun = Version1.ReadAfterHeader(
                        fHeader,
                        aReader,
                        fBodySize,
                        out fMessageBodySize);
                    break;
                case 1:
                    fToRetrun = Version1.ReadAfterHeader(
                        fHeader,
                        aReader,
                        fBodySize,
                        out fMessageBodySize);
                    break;

                default:
                    throw new UnknownMessageVersion<DataspaceMessage>(fHeader.Version);
            }
            aBytes = DSHeader.Size + fMessageBodySize;
            return fToRetrun;
        }

        internal protected static Dimension[] ReadDimensions(
            Hdf5Reader aReader,
            bool aHasMax,
            int aDimCount)
        {
            Dimension[]
                fDimensions;

            long[]
                fSizes = Enumerable
                    .Range(0, aDimCount)
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
            if (aHasMax)
            {
                fDimensions = fSizes.Zip(
                    Enumerable.Range(0, aDimCount)
                        .Select(a => aReader.ReadLength()),
                    (aLength, aMax) => new Dimension(aLength, aMax))
                    .ToArray();

            }
            else
            {
                fDimensions = fSizes.Select(a => new Dimension(a)).ToArray();
            }

            return fDimensions;
        }
    }
}

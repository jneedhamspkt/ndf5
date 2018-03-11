using System;
using System.Collections.Generic;

using ndf5.Streams;
using ndf5.Exceptions;
using ndf5.Objects;


namespace ndf5.Messages
{
    /// <summary>
    /// Message class for message objects marked as Data Space type
    /// </summary>
    public partial class DataspaceMessage : Message
    {
        public DataSpaceType 
            DataSpaceType { get; }

        public IReadOnlyList<Dimension>
            Dimensions { get; }

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
    }
}

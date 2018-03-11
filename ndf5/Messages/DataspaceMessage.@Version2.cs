using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;
namespace ndf5.Messages
{
    partial class DataspaceMessage
    {
        private class Version2 : DataspaceMessage
        {
            public Version2(
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
                throw new NotImplementedException();
            }
        }
    }
}

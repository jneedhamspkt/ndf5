using System;
using System.IO;
using ndf5.Streams;

namespace ndf5.Metadata
{
    internal class Hdf5SuperBlockProvider : ISuperBlockProvider
    {
        private readonly IStreamProvider 
            mrStreamProvider;
        FormatSignatureAndVersionInfo 
            mrFormatSignatureAndVersionInfo;

        internal Hdf5SuperBlockProvider(
            IStreamProvider aStreamProvider,
            FormatSignatureAndVersionInfo aFormatSignatureAndVersionInfo)
        {
            mrStreamProvider = aStreamProvider;
            mrFormatSignatureAndVersionInfo = aFormatSignatureAndVersionInfo;

        }

        public ISuperBlock SuperBlock
        {
            get
            {
                throw new NotImplementedException();

                
            }
        }
    }
}

using System;
using System.IO;
using TinyIoC;

using ndf5.Metadata;

namespace ndf5
{
    public class Hdf5File
    {
        /// <summary>
        /// Pesristantly avlaible data in this file
        /// </summary>
        private readonly TinyIoCContainer
            FileData = new TinyIoCContainer();

        public StreamInfo
            StreamInfo;

        public FormatSignatureAndVersionInfo
            FormatSignatureAndVersionInfo;

        public Hdf5File(Stream aFileStream)
        {
            if(!aFileStream.CanRead)
            {
                throw new Exception("Cannot Read Stream");
            }

            System.IO.FileStream
                  fFileStream = aFileStream as FileStream;
            
            if(!ReferenceEquals(null, fFileStream))
            {
                StreamInfo = new StreamInfo(
                    fFileStream.CanRead,
                    fFileStream.Position,
                    new FileInfo(fFileStream.Name));
            }
            else
            {
                StreamInfo = new StreamInfo(
                    aFileStream.CanRead,
                    aFileStream.Position);
            }

            
            FileData.Register(
                typeof(StreamInfo),
                this.StreamInfo);

            if(!FormatSignatureAndVersionInfo.TryRead(
                aFileStream,
                out FormatSignatureAndVersionInfo))
                throw new Exception("This does not appear to be an HDF5 file / stream");
                

        }

    }
}

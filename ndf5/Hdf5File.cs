using System;
using System.IO;
using TinyIoC;

using ndf5.Metadata;
using ndf5.Streams;

namespace ndf5
{
    public class Hdf5File
    {
        /// <summary>
        /// Pesristantly avlaible data in this file
        /// </summary>
        private readonly TinyIoCContainer
            mrFileData = new TinyIoCContainer();

        /// <summary>
        /// Gets the stream info.
        /// </summary>
        /// <value>The stream info.</value>
        internal StreamInfo
            StreamInfo => mrFileData.Resolve(
                typeof(StreamInfo),
                ResolveOptions.Default) as StreamInfo;
        
        /// <summary>
        /// Gets the format signature and version info.
        /// </summary>
        /// <value>The format signature and version info.</value>
        public FormatSignatureAndVersionInfo
            FormatSignatureAndVersionInfo => mrFileData.Resolve(
                typeof(FormatSignatureAndVersionInfo), 
                ResolveOptions.Default) as FormatSignatureAndVersionInfo;

        private Hdf5File()
        {
            
        }

        public static Hdf5File Open(string aPath)
        {
            return Open(new FileInfo(aPath));
        }

        public static Hdf5File Open(FileInfo aFileInfo)
        {
            Hdf5File
                fToReturn = new Hdf5File();
            StreamInfo 
                fStreamInfo = new StreamInfo(aFileInfo);



            return fToReturn;

        }

        public Hdf5File Open(Stream aFileStream)
        {
            Hdf5File
                fToReturn = new Hdf5File();
            if(!aFileStream.CanRead)
            {
                throw new Exception("Cannot Read Stream");
            }

            System.IO.FileStream
                  fFileStream = aFileStream as FileStream;

            StreamInfo
                fStreamInfo;

            if(!ReferenceEquals(null, fFileStream))
            {
                fStreamInfo = new StreamInfo(
                    fFileStream.CanRead,
                    fFileStream.Position,
                    new FileInfo(fFileStream.Name));
            }
            else
            {
                fStreamInfo = new StreamInfo(
                    aFileStream.CanRead,
                    aFileStream.Position);
            }

            
            fToReturn.mrFileData.Register(
                fStreamInfo);
            FormatSignatureAndVersionInfo
                fFormatSignatureAndVersionInfo;
            if(!FormatSignatureAndVersionInfo.TryRead(
                aFileStream,
                out fFormatSignatureAndVersionInfo))
                throw new Exception("This does not appear to be an HDF5 file / stream");

            fToReturn.mrFileData.Register(fFormatSignatureAndVersionInfo);

            return fToReturn;
        }

    }
}

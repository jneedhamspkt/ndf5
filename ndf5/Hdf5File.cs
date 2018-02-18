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
        StreamInfo => mrFileData.Resolve<StreamInfo>(
                ResolveOptions.Default);
        
        /// <summary>
        /// Gets the format signature and version info.
        /// </summary>
        /// <value>The format signature and version info.</value>
        public FormatSignatureAndVersionInfo
        FormatSignatureAndVersionInfo => mrFileData.Resolve<FormatSignatureAndVersionInfo>(
                ResolveOptions.Default);

        private IStreamProvider
            StreamProvider => mrFileData.Resolve<IStreamProvider>(
                ResolveOptions.Default);

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
            FileStream
                fFileStream = new FileStream(
                    aFileInfo.FullName,
                    FileMode.Open,
                    fStreamInfo.CanWrite
                        ? FileAccess.ReadWrite
                        : FileAccess.Read,
                    FileShare.Read);

            fToReturn.mrFileData.Register(fStreamInfo).AsSingleton();
            fToReturn.mrFileData.Register<IStreamProvider>(
                new SingleStreamProvider(fFileStream));

            fToReturn.Load();

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
                fStreamInfo).AsSingleton();

            fToReturn.mrFileData.Register<IStreamProvider>(
                new SingleStreamProvider(aFileStream)).AsSingleton();
            
            fToReturn.Load();

            return fToReturn;
        }

        private void Load()
        {
            IStreamProvider 
                fStreamProvider = this.StreamProvider;
            using (Stream fLoadingStream = fStreamProvider.GetReadonlyStream()) 
            {
                FormatSignatureAndVersionInfo
                    fFormatSignatureAndVersionInfo;

                if (!FormatSignatureAndVersionInfo.TryRead(
                    fLoadingStream,
                    out fFormatSignatureAndVersionInfo))
                    throw new Exception("This does not appear to be an HDF5 file / stream");

                this.mrFileData.Register(
                    fFormatSignatureAndVersionInfo).AsSingleton();
            }
        }


    }
}

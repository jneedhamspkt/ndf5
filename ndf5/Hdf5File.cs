using System;
using System.IO;
using TinyIoC;

using ndf5.Metadata;
using ndf5.Streams;

namespace ndf5
{
    public class Hdf5File : IDisposable
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

        public ISuperBlock
            SuperBlock => mrFileData.Resolve<ISuperBlock>();

        private IStreamProvider
            StreamProvider => mrFileData.Resolve<IStreamProvider>(
                ResolveOptions.Default);

        Hdf5File()
        {
            mrFileData.Register<ISuperBlockProvider, Hdf5SuperBlockProvider>().AsSingleton();
            mrFileData.Register<ISuperBlock>((arg1, arg2) =>
                 arg1.Resolve<ISuperBlockProvider>().SuperBlock);
        }

        ~Hdf5File()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool aDisposing)
        {
            if (!aDisposing)
                return;

            IDisposable
                fStreamProvider = mrFileData.Resolve<IStreamProvider>() as IDisposable;
            if (!ReferenceEquals(null, fStreamProvider))
                fStreamProvider.Dispose();

            mrFileData.Dispose();
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

            fToReturn.mrFileData.Register(fStreamInfo);
            fToReturn.mrFileData.Register<IStreamProvider, SimpleSingleStreamProvider>(
                new SimpleSingleStreamProvider(
                    fFileStream));

            fToReturn.Load();

            return fToReturn;

        }

        public static Hdf5File Open(Stream aFileStream)
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
                    new FileInfo(fFileStream.Name));
            }
            else
            {
                fStreamInfo = new StreamInfo(
                    aFileStream.CanRead);
            }

            fToReturn.mrFileData.Register(
                fStreamInfo);

            fToReturn.mrFileData.Register<IStreamProvider, SimpleSingleStreamProvider>(
                new SimpleSingleStreamProvider(
                    aFileStream));
            
            fToReturn.Load();

            return fToReturn;
        }

        private void Load()
        {
            IStreamProvider 
                fStreamProvider = this.StreamProvider;
            using (Stream fLoadingStream = fStreamProvider.GetReadonlyStream()) 
            {
                //We Must start at the beginning
                fLoadingStream.Seek(0L, SeekOrigin.Begin);
                long
                    fStreamLength = fLoadingStream.Length;
                long
                    fiStart = 0; 

                FormatSignatureAndVersionInfo
                fFormatSignatureAndVersionInfo = null;

                while (fiStart < fStreamLength)
                {
                    if (FormatSignatureAndVersionInfo.TryRead(
                        fLoadingStream,
                        out fFormatSignatureAndVersionInfo))
                    {
                        //TODO: Verify Checksum for Version 2 + 3 Here 
                        //      before we decide to break or not
                        break;
                    }
                        

                    // Look for correct HDF5 start a 512, 1024, 2048, etc 
                    // See https://support.hdfgroup.org/HDF5/doc/H5.format.html#Superblock
                    fiStart = fiStart == 0
                        ? 512
                        : fiStart * 2;
                }
                if(ReferenceEquals(null, fFormatSignatureAndVersionInfo))
                    throw new Exception("This does not appear to be an HDF5 file / stream");

                this.mrFileData.Register(
                    fFormatSignatureAndVersionInfo);



            }
        }


    }
}

using System.IO;
using ndf5.Metadata;
namespace ndf5.Streams
{
    public interface IHdfStreamProvider : IStreamProvider
    {
        /// <summary>
        /// Gets the super controlling the reader / writer parameters
        /// </summary>
        /// <value>The super block.</value>
        ISuperBlock SuperBlock { get; }

        /// <summary>
        /// Gets an HdfReader instance
        /// </summary>
        /// <returns>The reader</returns>
        Hdf5Reader GetReader();

        /// <summary>
        /// Gets an HdfWriter instance
        /// </summary>
        /// <returns>The writer</returns>
        Hdf5Reader GetWriter();
    }
}

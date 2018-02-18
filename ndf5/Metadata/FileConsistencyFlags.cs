using System;
namespace ndf5.Metadata
{
    /// <summary>
    /// File consistency flags for version 3 superblocks
    /// </summary>
    [Flags]
    public enum FileConsistencyFlags : byte
    {
        /// <summary>
        /// The file is open for write access
        /// </summary>
        WriteAccessOpen = 0x1,


        /// <summary>
        /// The File is open with Single Writer Multiple Readers
        /// </summary>
        SwmrAccessEngaged = 0x4
    }
}

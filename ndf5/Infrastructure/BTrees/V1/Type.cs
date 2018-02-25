using System;
namespace ndf5.Infrastructure.BTrees.V1
{
    /// <summary>
    /// Type of data containd by a V1 Btree
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// This tree points to group nodes
        /// </summary>
        Group = 0,

        /// <summary>
        /// This tree points to raw data chunk nodes.
        /// </summary>
        Data = 1
    }
}

using System;
namespace ndf5.Messages
{
    /// <summary>
    /// Flags that describe what types of objects require a message type
    /// </summary>
    [Flags]
    public enum RequiredFlags : byte
    {
        /// <summary>
        /// Not Required by any Ojbect type
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Required by old Style group objects
        /// </summary>
        OldStyleGroups = 0x01,

        /// <summary>
        /// Required by new Style group objects
        /// </summary>
        NewStyleGroups = 0x2,

        /// <summary>
        /// Required by any group object
        /// </summary>
        AllGroups = 0x03,

        /// <summary>
        /// Required by any data object
        /// </summary>
        DataSets = 0x04,

        /// <summary>
        /// Required by any Committed Data Type
        /// </summary>
        CommittedDataType = 0x08,

        /// <summary>
        /// Required by all objects
        /// </summary>
        All = 0x0F
    }
}

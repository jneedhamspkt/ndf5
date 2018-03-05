using System;
namespace ndf5.Messages
{
    [Flags]
    public enum RequiredFlags : byte
    {
        None = 0x00,

        OldStyleGroups = 0x01,

        NewStyleGroups = 0x2,

        AllGroups = 0x03,

        DataSets = 0x04,

        CommittedDataType = 0x08,

        All = 0x0F
    }
}

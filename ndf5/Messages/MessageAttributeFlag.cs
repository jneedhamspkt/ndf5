using System;
namespace ndf5.Messages
{
    /// <summary>
    /// Flags that decribe how to interprit a messag in an object header
    /// </summary>
    /// <remarks>
    /// Annotaions orignate from <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#V1ObjectHeaderPrefix"/>
    /// </remarks>
    [Flags]
    public enum MessageAttributeFlag : byte
    {
        /// <summary>
        /// MessageAttributeFlag with no bits set
        /// </summary>
        None = 0x0,

        /// <summary>
        /// If set, the message data is constant. This is used for messages like
        /// the datatype message of a dataset.
        /// </summary>
        Constatnt = 0x01,

        /// <summary>
        /// If set, the message is shared and stored in another location than 
        /// the object header. The Header Message Data field contains a Shared 
        /// Message (described in the Data Object Header Messages section below)
        /// and the Size of Header Message Data field contains the size of that 
        /// Shared Message.
        /// </summary>
        Shared = 0x02,

        /// <summary>
        /// If set, the message should not be shared.
        /// </summary>
        NotShareable = 0x04,

        /// <summary>
        /// If set, the HDF5 decoder should fail to open this object if it does 
        /// not understand the message’s type and the file is open with 
        /// permissions allowing write access to the file. (Normally, unknown 
        /// messages can just be ignored by HDF5 decoders)
        /// </summary>
        MustUnderstandToRead = 0x08,

        /// <summary>
        /// If set, the HDF5 decoder should set bit 5 of this message’s flags 
        /// (in other words, this bit field) if it does not understand the 
        /// message’s type and the object is modified in any way. (Normally, 
        /// unknown messages can just be ignored by HDF5 decoders)
        /// </summary>
        MarkNonUnderstoodModify = 0x10,

        /// <summary>
        /// If set, this object was modified by software that did not understand 
        /// this message. (Normally, unknown messages should just be ignored by
        ///  HDF5 decoders) (Can be used to invalidate an index or a similar feature)
        /// </summary>
        NonUnderstoodModified = 0x20,

        /// <summary>
        /// If set, this message is shareable.
        /// </summary>
        Sharable = 0x40,

        /// <summary>
        /// If set, the HDF5 decoder should always fail to open this object if 
        /// it does not understand the message’s type (whether it is open for 
        /// read-only or read-write access). (Normally, unknown messages can 
        /// just be ignored by HDF5 decoders)
        /// </summary>
        MustUnderstandToWrite= 0x80
    }
}

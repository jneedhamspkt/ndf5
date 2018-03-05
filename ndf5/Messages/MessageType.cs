using System;
using System.Collections.Generic;

namespace ndf5.Messages
{
    /// <summary>
    /// Type of a message in an HDF5 object header
    /// Notes / comments originate from <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#ObjectHeaderMessages"/>
    /// </summary>
    public enum MessageType : byte
    {
        /// <summary>
        /// The NIL message is used to indicate a message which is to be ignored 
        /// when reading the header messages for a data object. [Possibly one
        /// which has been deleted for some reason.]
        /// </summary>
        /// <remarks>Optional; may be repeated.</remarks>
        NIL = 0x00,

        /// <summary>
        /// The dataspace message describes the number of dimensions (in other
        /// words, “rank”) and size of each dimension that the data object has.
        /// This message is only used for datasets which have a simple, 
        /// rectilinear, array-like layout; datasets requiring a more complex
        /// layout are not yet supported.
        /// </summary>
        /// <remarks>Required for dataset objects; may not be repeated.</remarks>
        Dataspace = 0x01,

        /// <summary>
        /// The link info message tracks variable information about the current 
        /// state of the links for a “new style” group’s behavior. Variable
        /// information will be stored in this message and constant information 
        /// will be stored in the Group Info message.
        /// </summary>
        /// <remarks>ptional; may not be repeated.</remarks>
        LinkInfo = 0x02,

        /// <summary>
        /// The datatype message defines the datatype for each element of a
        /// dataset or a common datatype for sharing between multiple datasets.
        /// A datatype can describe an atomic type like a fixed- or floating-point 
        /// type or more complex types like a C struct (compound datatype), array 
        /// (array datatype), or C++ vector (variable-length datatype).
        ///
        /// Datatype messages that are part of a dataset object do not describe
        /// how elements are related to one another; the dataspace message is 
        /// used for that purpose. Datatype messages that are part of a 
        /// committed datatype (formerly named datatype) message describe a 
        /// common datatype that can be shared by multiple datasets in the file.
        /// </summary>
        /// <remarks>
        /// Required for dataset or committed datatype (formerly named datatype)
        /// objects; may not be repeated.
        /// </remarks>
        Datatype = 0x03,

        /// <summary>
        /// The fill value message stores a single data value which is returned
        /// to the application when an uninitialized data element is read from
        /// a dataset. The fill value is interpreted with the same datatype as
        /// the dataset. If no fill value message is present then a fill value
        /// of all zero bytes is assumed.
        ///
        /// This fill value message is deprecated in favor of the “new” fill 
        /// value message(Message Type 0x0005) and is only written to the file 
        /// for forward compatibility with versions of the HDF5 Library before 
        /// the 1.6.0 version.Additionally, it only appears for datasets with a 
        /// user-defined fill value(as opposed to the library default fill value
        ///  or an explicitly set “undefined” fill value).
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        OldFillValue = 0x04,

        /// <summary>
        /// The fill value message stores a single data value which is returned 
        /// to the application when an uninitialized data element is read from 
        /// a dataset. The fill value is interpreted with the same datatype as 
        /// the dataset.
        /// </summary>
        /// <remarks>Required for dataset objects; may not be repeated.</remarks>
        FillValue = 0x05,

        /// <summary>
        /// This message encodes the information for a link in a group’s object 
        /// header, when the group is storing its links “compactly”, or in the
        /// group’s fractal heap, when the group is storing its links “densely”.
        ///
        /// A group is storing its links compactly when the fractal heap address
        /// in the Link Info Message is set to the “undefined address” value.
        /// </summary>
        /// <remarks>Optional; may be repeated.</remarks>
        Link = 0x06,


        /// <summary>
        /// The external data storage message indicates that the data for an 
        /// object is stored outside the HDF5 file. The filename of the object 
        /// is stored as a Universal Resource Location (URL) of the actual 
        /// filename containing the data. An external file list record also 
        /// contains the byte offset of the start of the data within the file 
        /// and the amount of space reserved in the file for that data.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        ExternalDataFiles = 0x07,

        /// <summary>
        /// The Data Layout message describes how the elements of a multi-
        /// dimensional array are stored in the HDF5 file. Four types of data 
        /// layout are supported:
        /// 
        /// Contiguous: The array is stored in one contiguous area of the file. 
        ///     This layout requires that the size of the array be constant: 
        ///     data manipulations such as chunking, compression, checksums,
        ///     or encryption are not permitted. The message stores the total 
        ///     storage size of the array. The offset of an element from the 
        ///     beginning of the storage area is computed as in a C array.
        /// 
        /// Chunked: The array domain is regularly decomposed into chunks, and 
        ///     each chunk is allocated and stored separately. This layout 
        ///     supports arbitrary element traversals, compression, encryption,
        ///     and checksums (these features are described in other messages).
        ///     The message stores the size of a chunk instead of the size of 
        ///     the entire array; the storage size of the entire array can be 
        ///     calculated by traversing the chunk index that stores the chunk 
        ///     addresses.
        /// 
        /// Compact: The array is stored in one contiguous block as part of this 
        ///     object header message.
        /// 
        /// Virtual: This is only supported for version 4 of the Data Layout
        ///     message. The message stores information that is used to locate 
        ///     the global heap collection containing the Virtual Dataset (VDS)
        ///     mapping information. The mapping associates the VDS to the
        ///     source dataset elements that are stored across a collection of
        ///     HDF5 files.
        /// </summary>
        /// <remarks>Required for datasets; may not be repeated.</remarks>
        DataLayout = 0x08,

        // <summary>
        // For testing only; should never be stored in a valid file.
        // </summary>
        //Bogus = 0x09

        /// <summary>
        /// This message stores information for the constants defining a 
        /// “new style” group’s behavior. Constant information will be stored 
        /// in this message and variable information will be stored in the Link 
        /// Info message.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        GroupInfo = 0x0A,

        /// <summary>
        /// This message describes the filter pipeline which should be applied
        /// to the data stream by providing filter identification numbers, 
        /// flags, a name, and client data.
        ///
        /// This message may be present in the object headers of both dataset 
        /// and group objects. For datasets, it specifies the filters to apply
        /// to raw data. For groups, it specifies the filters to apply to the 
        /// group’s fractal heap. Currently, only datasets using chunked data 
        /// storage use the filter pipeline on their raw data.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        DataStorageFilterPipeline = 0x0B,

        /// <summary>
        /// The Attribute message is used to store objects in the HDF5 file 
        /// which are used as attributes, or “metadata” about the current object.
        /// An attribute is a small dataset; it has a name, a datatype, a 
        /// dataspace, and raw data. Since attributes are stored in the object 
        /// header, they should be relatively small (in other words, less than
        /// 64KB). They can be associated with any type of object which has an 
        /// object header (groups, datasets, or committed (named) datatypes).
        /// 
        /// In 1.8.x versions of the library, attributes can be larger than 
        /// 64KB. See the “Special Issues” section of the Attributes chapter in 
        /// the HDF5 User’s Guide for more information.
        /// 
        /// Note: Attributes on an object must have unique names: the HDF5 
        /// Library currently enforces this by causing the creation of an 
        /// attribute with a duplicate name to fail. Attributes on different 
        /// objects may have the same name, however.
        /// </summary>
        /// <remarks>Optional; may be repeated.</remarks>
        Attribute = 0x0C,

        /// <summary>
        /// The object comment is designed to be a short description of an
        /// object. An object comment is a sequence of non-zero (\0) ASCII
        /// characters with no other formatting included by the library.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        ObjectComment = 0x0D,

        /// <summary>
        /// The object modification date and time is a timestamp which indicates
        /// (using ISO-8601 date and time format) the last modification of an
        /// object. The time is updated when any object header message changes 
        /// according to the system clock where the change was posted. All 
        /// fields of this message should be interpreted as coordinated 
        /// universal time (UTC).
        /// 
        /// This modification time message is deprecated in favor of the “new” 
        /// Object Modification Time message and is no longer written to the 
        /// file in versions of the HDF5 Library after the 1.6.0 version.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        OldObjectModificationTime = 0x0E,

        /// <summary>
        /// This message is used to locate the table of shared object header
        /// message (SOHM) indexes. Each index consists of information to find
        /// the shared messages from either the heap or object header. This
        /// message is only found in the superblock extension.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        SharedMessageTable = 0x0f,

        /// <summary>
        /// The object header continuation is the location in the file of a 
        /// block containing more header messages for the current data object. 
        /// This can be used when header blocks become too large or are likely
        /// to change over time.
        /// </summary>
        /// <remarks>Optional; may be repeated.</remarks>
        ObjectHeaderContinuation = 0x10,

        /// <summary>
        /// Each “old style” group has a v1 B-tree and a local heap for storing 
        /// symbol table entries, which are located with this message.
        /// </summary>
        /// <remarks>Required for “old style” groups; may not be repeated.</remarks>
        SymbolTableMessage = 0x11,

        /// <summary>
        /// The object modification time is a timestamp which indicates the
        /// time of the last modification of an object. The time is updated when
        /// any object header message changes according to the system clock 
        /// where the change was posted.
        /// </summary>
        /// <remarks> Optional; may not be repeated.</remarks>
        ObjectModificationTime = 0x12,

        /// <summary>
        /// This message retrieves non-default ‘K’ values for internal and leaf
        /// nodes of a group or indexed storage v1 B-trees. This message is only
        /// found in the superblock extension.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        BtreeKValues = 0x13,

        /// <summary>
        /// This message contains information needed by the file driver to 
        /// reopen a file. This message is only found in the superblock 
        /// extension: see the “Disk Format: Level 0C - Superblock Extension” 
        /// section for more information. For more information on the fields in 
        /// the driver info message, see the “Disk Format: Level 0B - File 
        /// Driver Info” section; those who use the multi and family file 
        /// drivers will find this section particularly helpful.
        /// </summary>
        DriverInfo = 0x14,

        /// <summary>
        /// This message stores information about the attributes on an object, 
        /// such as the maximum creation index for the attributes created and 
        /// the location of the attribute storage when the attributes are 
        /// stored “densely”.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        AttributeInfo = 0x15,

        /// <summary>
        /// This message stores the number of hard links (in groups or objects) 
        /// pointing to an object: in other words, its reference count.
        /// </summary>
        /// <remarks>Optional; may not be repeated.</remarks>
        ObjectReferenceCount = 0x16,
    }

    public static class MessageTypeExtensions
    {
        private static readonly Dictionary<MessageType, MessageTypeInfo>
        msrTypeInfo = new Dictionary<MessageType, MessageTypeInfo>
        {
            {
                MessageType.NIL,
                new MessageTypeInfo(
                    aName: "NIL",
                    aRepeatable: true,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.Dataspace,
                new MessageTypeInfo(
                    aName: "Dataspace",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.DataSets)
            },
            {
                MessageType.LinkInfo,
                new MessageTypeInfo(
                    aName: "Link Info",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None )
            },
            {
                MessageType.Datatype,
                new MessageTypeInfo(
                    aName: "Datatype",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.DataSets | RequiredFlags.CommittedDataType )
            },
            {
                MessageType.OldFillValue,
                new MessageTypeInfo(
                    aName: "Fill Value (old)",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.FillValue,
                new MessageTypeInfo(
                    aName: "Fill Value",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.DataSets)
            },
            {
                MessageType.Link,
                new MessageTypeInfo(
                    aName: "Link",
                    aRepeatable: true,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.ExternalDataFiles,
                new MessageTypeInfo(
                    aName: "External Data Files",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.DataLayout,
                new MessageTypeInfo(
                    aName: "Data Layout",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.DataSets)
            },
            {
                MessageType.GroupInfo,
                new MessageTypeInfo(
                    aName: "Group Info",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.DataStorageFilterPipeline,
                new MessageTypeInfo(
                    aName: "Data Storage - Filter Pipeline",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.Attribute,
                new MessageTypeInfo(
                    aName: "Attribute",
                    aRepeatable: true,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.ObjectComment,
                new MessageTypeInfo(
                    aName: "Object Comment",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.OldObjectModificationTime,
                new MessageTypeInfo(
                    aName: "Object Modification Time (Old)",
                    aLength: 16,
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.SharedMessageTable,
                new MessageTypeInfo(
                    aName: "Shared Message Table",
                    aLength: null, // Documentaion says "fixed", but it's dependant 
                                   // on the Super Block data
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.ObjectHeaderContinuation,
                new MessageTypeInfo(
                    aName: "Object Header Continuation",
                    aLength: null, // Documentaion says "fixed", but it's dependant 
                                   // on the Super Block data
                    aRepeatable: true,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.SharedMessageTable,
                new MessageTypeInfo(
                    aName: "Symbol Table Message",
                    aLength: null, // Documentaion says "fixed", but it's dependant 
                                   // on the Super Block data
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.OldStyleGroups)
            },
            {
                MessageType.ObjectModificationTime,
                new MessageTypeInfo(
                    aName: "Object Modification Time",
                    aLength: 8,
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.BtreeKValues,
                new MessageTypeInfo(
                    aName: "B-tree ‘K’ Values",
                    aLength: 8,
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.DriverInfo,
                new MessageTypeInfo(
                    aName: "Driver Info",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.AttributeInfo,
                new MessageTypeInfo(
                    aName: "Attribute Info",
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            },
            {
                MessageType.ObjectReferenceCount,
                new MessageTypeInfo(
                    aName: "Object Reference Count",
                    aLength: 8,
                    aRepeatable: false,
                    aRequiredFor: RequiredFlags.None)
            }
        };


        /// <summary>
        /// Name of the specified aMessageType.
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="aMessageType">A message type to check.</param>
        public static string Name(this MessageType aMessageType) =>
            msrTypeInfo[aMessageType].Name;

        /// <summary>
        /// Length the specified aMessageType.
        /// </summary>
        /// <returns>The length.</returns>
        /// <param name="aMessageType">A message type to check.</param>
        public static int? Length(this MessageType aMessageType) =>
            msrTypeInfo[aMessageType].Length;

        /// <summary>
        /// Checks to see if this message type is repeatable
        /// </summary>
        /// <returns><c>true</c>, if repeatable was ised, <c>false</c> otherwise.</returns>
        /// <param name="aMessageType">A message type to check.</param>
        public static bool IsRepeatable(this MessageType aMessageType) =>
            msrTypeInfo[aMessageType].IsRepeatable;

        /// <summary>
        /// Checks to see if this messag typ eis required for a type of object
        /// </summary>
        /// <returns><c>true</c>, if for was requireded, <c>false</c> otherwise.</returns>
        /// <param name="aMessageType">A message type.</param>
        /// <param name="aFlags">Type of object to check for.</param>
        public static bool RequiredFor(this MessageType aMessageType, RequiredFlags aFlags) =>
            (msrTypeInfo[aMessageType].RequiredFor | aFlags) != 0;
    }
}

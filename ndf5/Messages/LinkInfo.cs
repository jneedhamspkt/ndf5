using System;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Link info.
    /// </summary>
    /// <remarks>Property annotaions coem from <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#LinkMessage"/></remarks>
    public class LinkInfo : Message
    {
        const byte
            mcCurrentVersion = 0;

        [Flags]
        enum Flags
        {
            None = 0,
            CreationOrderTracked = 1,
            CreationOrderIndexed = 2
        }

        /// <summary>
        /// This 64-bit value is the maximum creation order index value stored 
        /// for a link in this group. (Null if untracked)
        /// </summary>
        /// <value>The maximum index of the creation.</value>
        public ulong? MaximumCreationIndex { get; }

        /// <summary>
        /// This is the address of the fractal heap to store dense links. Each
        /// link stored in the fractal heap is stored as a Link Message.
        ///
        /// If there are no links in the group, or the group’s links are stored
        /// “compactly” (as object header messages), this value will be null
        /// </summary>
        /// <value>The fractal heap address.</value>
        public Offset FractalHeapAddress { get; }

        /// <summary>
        /// This is the address of the version 2 B-tree to index names of links.
        /// 
        /// If there are no links in the group, or the group’s links are stored
        /// “compactly” (as object header messages), this value will be null
        /// </summary>
        /// <value>The name index BT ree address.</value>
        public Offset NameIndexBTreeAddress { get; }

        /// <summary>
        /// This is the address of the version 2 B-tree to index creation order
        /// of links.
        /// 
        /// If there are no links in the group, or the group’s links are stored
        /// “compactly” (as object header messages), this value will be null
        /// </summary>
        /// <value>The name index BT ree address.</value>
        public Offset CreationOrderIndexBTreeAddress { get; }

        /// <summary>
        /// True if Creation order is tracked
        /// </summary>
        /// <value><c>true</c> if is creation order tracked; otherwise, <c>false</c>.</value>
        public bool IsCreationOrderTracked { get; }

        /// <summary>
        /// True if Creation order is indexed
        /// </summary>
        /// <value><c>true</c> if is creation order indexed; otherwise, <c>false</c>.</value>
        public bool IsCreationOrderIndexed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.LinkInfo"/> class.
        /// </summary>
        /// <param name="aMaximumCreationIndex">A maximum creation index. (Null if not tracked) </param>
        /// <param name="aFractalHeapAddress">The fractal heap address.</param>
        /// <param name="aNameIndexBTreeAddress">The name index B-Tree address.</param>
        /// <param name="aIsCreationOrderIndexed">If set to <c>true</c> a is creation order indexed.</param>
        /// <param name="aCreationOrderIndexBTreeAddress">The creation order index B Tree address.</param>
        public LinkInfo(
            ulong? aMaximumCreationIndex,
            Offset aFractalHeapAddress,
            Offset aNameIndexBTreeAddress,
            bool aIsCreationOrderIndexed,
            Offset aCreationOrderIndexBTreeAddress) : base(MessageType.LinkInfo)
        {
            MaximumCreationIndex = aMaximumCreationIndex;
            IsCreationOrderTracked = aMaximumCreationIndex.HasValue;
            FractalHeapAddress = aFractalHeapAddress;
            NameIndexBTreeAddress = aNameIndexBTreeAddress;
            IsCreationOrderIndexed = aIsCreationOrderIndexed;
            CreationOrderIndexBTreeAddress = aCreationOrderIndexBTreeAddress;
        }

        internal static Message Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            //Read Message Headder data
            int 
                fReadBytes = 4; //Always Read at least 4
            byte 
                fVersion = (byte)aReader.ReadByte(),
                fFlags = (byte)aReader.ReadByte();
            aReader.ReadUInt16(); //Reserved

            if (fVersion != mcCurrentVersion)
                throw new Exceptions.UnknownMessageVersion<LinkInfo>(fVersion);

            bool
                fCreationOrderTracked = ((Flags)fFlags).HasFlag(Flags.CreationOrderTracked),
                fCreationOrderIndexed = ((Flags)fFlags).HasFlag(Flags.CreationOrderIndexed);

            //Calculate Length
            if (fCreationOrderTracked)
                fReadBytes += 8;
            fReadBytes += 2 * (int)aReader.SizeOfOffset; //Fractal Heap address, B-Tree adress
            if (fCreationOrderIndexed)
                fReadBytes += (int)aReader.SizeOfOffset;
            aBytes = fReadBytes;

            if(aLocalMessageSize.HasValue &&  fReadBytes > aLocalMessageSize.Value)
                throw new ArgumentException("Specified Local Message Size not long enough");
            
            //Read Feilds and build the object
            LinkInfo
                fToReturn = new LinkInfo(
                    fCreationOrderTracked ? aReader.ReadUInt64() : (ulong?)null,
                    aReader.ReadOffset(),
                    aReader.ReadOffset(),
                    fCreationOrderIndexed,
                    fCreationOrderIndexed ? aReader.ReadOffset() : (Offset)null);

            return fToReturn;
        }
    }
}

using System;
using System.IO;
using ndf5.Streams;

namespace ndf5.Infrastructure.SymbolTable
{
    public class SymbolTableEntry
    {
        /// <summary>
        /// This is the byte offset into the group’s local heap for the name 
        /// of the link. The name is null terminated.
        /// </summary>
        /// <value>The link name offset.</value>
        public Offset LinkNameOffset { get; }

        /// <summary>
        /// Every object has an object header which serves as a permanent
        /// location for the object’s metadata. In addition to appearing in 
        /// the object header, some of the object’s metadata can be cached 
        /// in the scratch-pad space.
        /// </summary>
        /// <value>The object header address.</value>
        public Offset ObjectHeaderAddress { get; }

        /// <summary>
        /// Gets the type of the cache.
        /// </summary>
        /// <value>The type of the cache.</value>
        public CacheType CacheType { get; }

        /// <summary>
        /// This is the file address for the root of the group’s B-tree.
        /// </summary>
        /// <value>The BT ree address.</value>
        public Offset BTreeAddress { get; }

        /// <summary>
        /// Gets the local name heap address.
        /// </summary>
        /// <value>The name heap address.</value>
        public Offset NameHeapAddress{ get; }

        /// <summary>
        /// The value of a symbolic link (that is, the name of the thing to 
        /// which it points) is stored in the local heap. This field is the
        /// 4-byte offset into the local heap for the start of the link value, 
        /// which is null terminated.
        /// </summary>
        /// <value>The symbolic link offset.</value>
        public uint? SymbolicLinkOffset { get; }

        /// <summary>
        /// Hdf5s the reader.
        /// </summary>
        /// <param name="aLinkNameOffset">A link name offset.</param>
        /// <param name="aObjectHeaderAddress">A object header address.</param>
        /// <param name="aCacheType">A cache type.</param>
        /// <param name="aBTreeAddress">BT ree address.</param>
        /// <param name="aNameHeapAddress">Name heap address.</param>
        /// <param name="aSymbolicLinkOffset">Symbolic link offset.</param>
        private SymbolTableEntry(
            Offset aLinkNameOffset,
            Offset aObjectHeaderAddress,
            CacheType aCacheType,
            Offset aBTreeAddress = null,
            Offset aNameHeapAddress = null,
            uint? aSymbolicLinkOffset = null)
        {
            LinkNameOffset = aLinkNameOffset;
            ObjectHeaderAddress = aObjectHeaderAddress;
            CacheType = aCacheType;
            BTreeAddress = aBTreeAddress;
            NameHeapAddress = aNameHeapAddress;
            SymbolicLinkOffset = aSymbolicLinkOffset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Infrastructure.SymbolTable.SymbolTableEntry"/> class
        /// with type <see cref="T:ndf5.Infrastructure.SymbolTable.CacheType.NoCache"/>
        /// </summary>
        /// <param name="aLinkNameOffset">A link name offset.</param>
        /// <param name="aObjectHeaderAddress">A object header address.</param>
        public SymbolTableEntry (
            Offset aLinkNameOffset,
            Offset aObjectHeaderAddress) : this(
                aLinkNameOffset,
                aObjectHeaderAddress,
                CacheType.NoCache) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Infrastructure.SymbolTable.SymbolTableEntry"/> class
        /// with type <see cref="T:ndf5.Infrastructure.SymbolTable.CacheType.Cache"/>
        /// <param name="aLinkNameOffset">A link name offset.</param>
        /// <param name="aObjectHeaderAddress">A object header address.</param>
        /// <param name="aBTreeAddress">A BT ree address.</param>
        /// <param name="aNameHeapAddress">A name heap address.</param>
        public SymbolTableEntry (
            Offset aLinkNameOffset,
            Offset aObjectHeaderAddress,
            Offset aBTreeAddress = null,
            Offset aNameHeapAddress = null) : this(
                aLinkNameOffset,
                aObjectHeaderAddress,
                CacheType.Cache,
                aBTreeAddress,
                aNameHeapAddress) {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Infrastructure.SymbolTable.SymbolTableEntry"/> class
        /// with type <see cref="T:ndf5.Infrastructure.SymbolTable.CacheType.SymbolicLink"/>
        /// <param name="aLinkNameOffset">A link name offset.</param>
        /// <param name="aObjectHeaderAddress">A object header address.</param>
        /// <param name="aSymbolicLinkOffset">A symbolic link offset.</param>
        public SymbolTableEntry(
            Offset aLinkNameOffset,
            Offset aObjectHeaderAddress,
            uint? aSymbolicLinkOffset = null) : this(
                aLinkNameOffset,
                aObjectHeaderAddress,
                CacheType.SymbolicLink,
                aSymbolicLinkOffset: aSymbolicLinkOffset)
        { }

        /// <summary>
        /// Parse the specified SymbolTableEntry frome aStreamProvider and aLocation.
        /// </summary>
        /// <returns>The parsed SymbolTableEntry</returns>
        /// <param name="aStreamProvider">A stream provider.</param>
        /// <param name="aLocation">The location in the stream of the SymbolTableEntry</param>
        public static SymbolTableEntry Parse(
            IHdfStreamProvider aStreamProvider,
            long aLocation)
        {
            using(Hdf5Reader aReader = aStreamProvider.GetReader())
            {
                return Read(aReader); 
            }
        }

        /// <summary>
        /// Read the specified aStream and aLocation.
        /// </summary>
        /// <returns>The read.</returns>
        /// <param name="aStream">A stream.</param>
        /// <param name="aLocation">A location.</param>
        public static SymbolTableEntry Read(
            Hdf5Reader aReader)
        {
            Offset fLinkNameOffset = aReader.ReadOffset();
            Offset fObjectHeaderAddress = aReader.ReadOffset();
            CacheType fCacheType = (CacheType)aReader.ReadUInt32();
            aReader.ReadUInt32(); // Reserved Word
            switch (fCacheType)
            {
                case CacheType.NoCache:
                    return new SymbolTableEntry(
                        fLinkNameOffset,
                        fObjectHeaderAddress);
                case CacheType.Cache:
                    return new SymbolTableEntry(
                        fLinkNameOffset,
                        fObjectHeaderAddress,
                        aReader.ReadOffset(),
                        aReader.ReadOffset());
                case CacheType.SymbolicLink:
                    return new SymbolTableEntry(
                        fLinkNameOffset,
                        fObjectHeaderAddress,
                        aReader.ReadUInt32());

                default:
                    throw new InvalidDataException($"Unknown {nameof(CacheType)}");
            }
        }

    }
}

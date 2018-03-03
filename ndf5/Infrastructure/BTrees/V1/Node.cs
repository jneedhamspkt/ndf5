using System;
using System.Text;
using System.Collections.Generic;
using ndf5.Streams;
using ndf5.Metadata;
using System.IO;

namespace ndf5.Infrastructure.BTrees.V1
{
    internal class Node : BTreeNode
    {
        /// <summary>
        /// The byte signature of V1 B-Tree nodes
        /// </summary>
        public static readonly IReadOnlyList<byte>
            Signature = System.Text.Encoding.ASCII.GetBytes("TREE");

        /// <summary>
        /// The type of the tree this node is a mamber of.
        /// </summary>
        public readonly NodeType
            NodeType;

        /// <summary>
        /// Distance from the Leaves (Node Level 0)
        /// </summary>
        /// <remarks>
        /// From The HDF group (<see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#V1Btrees"/>):
        ///    
        /// The node level indicates the level at which this node appears in the
        /// tree(leaf nodes are at level zero). Not only does the level indicate 
        /// whether child pointers point to sub-trees or to data, but it can also
        /// be used to help file consistency checking utilities reconstruct damaged 
        /// trees.
        /// </remarks>
        public readonly byte
            NodeLevel;

        /// <summary>
        /// Number of entries presently in this node
        /// </summary>
        /// <remarks>
        /// From The HDF group (<see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#V1Btrees"/>):
        ///    
        /// This determines the number of children to which this node points.
        /// All nodes of a particular type of tree have the same maximum degree,
        /// but most nodes will point to less than that number of children. The
        /// valid child pointers and keys appear at the beginning of the node and
        /// the unused pointers and keys appear at the end of the node. The unused 
        /// pointers and keys have undefined values.
        /// </remarks>
        public readonly ushort
            EntriesUsed;

        /// <summary>
        /// The address of the left sibling of the node (Null if none)
        /// </summary>
        public readonly long?
            LeftSibling;

        /// <summary>
        /// The address of the right sibling of the node (Null if none)
        /// </summary>
        public readonly long?
            RightSibling;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Infrastructure.BTrees.V1.Node"/> class.
        /// </summary>
        /// <param name="aStreamProvider">A stream provider.</param>
        /// <param name="aLocation">A location in the stream of this Node</param>
        public static Node Parse(
            IHdfStreamProvider aStreamProvider,
            long aLocation)
        {
            using(Hdf5Reader fReader = aStreamProvider.GetReader())
            {
                return new Node(fReader, aLocation);
            }
        }
            
        public Node(Hdf5Reader aReader, long? aLocation) : base(
            BTreeVerson.One,
            aLocation ?? aReader.Position )  
        {
            aReader.VerifySignature(Signature);
            byte
                fType = (byte)aReader.ReadByte();
            if (fType > (byte)NodeType.Data)
                throw new InvalidDataException($"Unexpected Node Type: {fType}");
            this.NodeType = (NodeType) fType;
            this.NodeLevel = (byte)aReader.ReadByte();
            this.EntriesUsed = aReader.ReadUInt16();
            this.LeftSibling = aReader.ReadOffset();
            this.RightSibling = aReader.ReadOffset();
        }

        public Node(
            NodeType aNodeType,
            byte aNodeLevel,
            ushort aEntriesUsed,
            long? aLeftSibling,
            long? aRightSibling) : base(
                BTreeVerson.One, null)
        {
            this.NodeType = aNodeType;
            this.NodeLevel = aNodeLevel;
            this.EntriesUsed = aEntriesUsed;
            this.LeftSibling = aLeftSibling;
            this.RightSibling = aRightSibling;
        }
            
    }
}

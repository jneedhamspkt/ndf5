using System;
namespace ndf5.Infrastructure.BTrees.V2{
    /// <summary>
    /// Type of data containd by a V2 Btree
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// This B-tree is used for testing only. This value should not be used 
        /// for storing records in actual HDF5 files.
        /// </summary>
        Test = 0,

        /// <summary>
        /// This B-tree is used for indexing indirectly accessed, 
        /// non-filtered ‘huge’ fractal heap objects.
        /// </summary>
        IndirectNonFilteredHeapObject = 1,

        /// <summary>
        /// This B-tree is used for indexing indirectly accessed, filtered
        /// ‘huge’ fractal heap objects.
        /// </summary>
        IndirectFilteredHeapObject = 2,

        /// <summary>
        /// This B-tree is used for indexing directly accessed, non-filtered 
        /// ‘huge’ fractal heap objects
        /// </summary>
        DirectNonFilteredHeapObject = 3,

        /// <summary>
        /// This B-tree is used for indexing directly accessed, filtered 
        /// ‘huge’ fractal heap objects.
        /// </summary>
        DirectFilteredHeapObject = 4,

        /// <summary>
        /// This B-tree is used for indexing the ‘name’ field for links in 
        /// indexed groups.
        /// </summary>
        GroupName = 5,

        /// <summary>
        /// This B-tree is used for indexing the ‘creation order’ field for 
        /// links in indexed groups.
        /// </summary>
        GroupCreationOrder = 6,

        /// <summary>
        /// This B-tree is used for indexing shared object header messages.
        /// </summary>
        SharedObjectMessageHeader = 7,

        /// <summary>
        /// This B-tree is used for indexing the ‘name’ field for indexed
        /// attributes.
        /// </summary>
        AttributeName = 8,

        /// <summary>
        /// This B-tree is used for indexing the ‘creation order’ field for
        /// indexed attributes.
        /// </summary>
        AttributeCreationOrder = 9,


        /// <summary>
        /// This B-tree is used for indexing chunks of datasets with no
        /// filters and with more than one dimension of unlimited extent.
        /// </summary>
        UnfilteredUnlimitedData = 10,

        /// <summary>
        /// This B-tree is used for indexing chunks of datasets with filters
        /// and more than one dimension of unlimited extent.
        /// </summary>
        FilteredUnlimitedData = 11,
    }
}

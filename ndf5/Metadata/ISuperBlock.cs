﻿using System;
using ndf5.Infrastructure.SymbolTable;

namespace ndf5.Metadata
{
    public interface ISuperBlock
    {
        /// <summary>
        /// Location of the first byte of the superblock in the file 
        /// (found, not parsed)
        /// </summary>
        /// <value>The location address.</value>
        Offset LocationAddress { get; }

        /// <summary>
        /// Gets the super block version.
        /// </summary>
        /// <value>The super block version.</value>
        byte SuperBlockVersion { get; }

        /// <summary>
        /// Gets the free space storage version.
        /// </summary>
        /// <value>The free space storage version.</value>
        byte FreeSpaceStorageVersion { get; }

        /// <summary>
        /// Gets the root group symbol table version.
        /// </summary>
        /// <value>The root group symbol table version.</value>
        byte RootGroupSymbolTableVersion { get; }

        /// <summary>
        /// Gets the shared header message format version.
        /// </summary>
        /// <value>The shared header message format version.</value>
        byte SharedHeaderMessageFormatVersion { get; }

        /// <summary>
        /// Gets the size of offsets.
        /// </summary>
        /// <value>The size of offsets.</value>
        byte SizeOfOffsets { get; }

        /// <summary>
        /// Gets the size of lengths.
        /// </summary>
        /// <value>The size of lengths.</value>
        byte SizeOfLengths { get; }

        /// <summary>
        /// Gets the group leaf node K.
        /// </summary>
        /// <value>The group leaf node k.</value>
        ushort GroupLeafNodeK { get; }

        /// <summary>
        /// Gets the group leaf node K.
        /// </summary>
        /// <remarks>
        /// <value>The group leaf node k.</value>
        ushort GroupInternalNodeK { get; }

        /// <summary>
        /// Gets the group leaf node K.
        /// </summary>
        /// <value>The group leaf node k.</value>
        ushort IndexedStorageInternalNodeK { get; }

        /// <summary>
        /// Gets the file consistency flags for the file
        /// </summary>
        /// <value>The file consistency flags.</value>
        FileConsistencyFlags? FileConsistencyFlags { get; }

        /// <summary>
        /// Gets the base address.
        /// </summary>
        /// <remarks>
        /// This is the absolute file address of the first byte of the HDF5 data 
        /// within the file. The library currently constrains this value to be 
        /// the absolute file address of the superblock itself when creating new
        /// files; future versions of the library may provide greater 
        /// flexibility When opening an existing file and this address does not
        /// match the offset of the superblock, the library assumes that the 
        /// entire contents of the HDF5 file have been adjusted in the file 
        /// and adjusts the base address and end of file address to reflect 
        /// their new positions in the file. Unless otherwise noted, all other
        /// file addresses are relative to this base address
        /// 
        /// Source: <see cref="https://support.hdfgroup.org/HDF5/doc/H5.format.html#Superblock"/>
        /// </remarks>
        /// <value>The base address.</value>
        Offset BaseAddress { get; }

        /// <summary>
        /// Gets the file freespace info address.
        /// </summary>
        /// <value>The file freespace info address.</value>
        Offset FileFreespaceInfoAddress { get; }

        /// <summary>
        /// Gets the end of file address.
        /// </summary>
        /// <value>The end of file address.</value>
        Offset EndOfFileAddress { get; }

        /// <summary>
        /// Gets the driver information block address.
        /// </summary>
        /// <value>The driver information block address.</value>
        Offset DriverInformationBlockAddress { get; }


        //TODO: Add Superblock Extension

        /// <summary>
        /// Gets the root group address, if Null, use the RootGroupSymbolTableEntry
        /// </summary>
        /// <value>The root group address.</value>
        Offset RootGroupAddress { get; }

        /// <summary>
        /// Gets the root group symbol table entry. (May be null)
        /// </summary>
        /// <value>The root group symbol table entry.</value>
        SymbolTableEntry RootGroupSymbolTableEntry { get; }

    }
}

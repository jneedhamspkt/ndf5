using System;
using System.Collections.Generic;
using System.Linq;
using ndf5.Streams;
using System.IO;

namespace ndf5.Messages
{
    /// <summary>
    /// Recursive data type which notates an array of elements of a base type
    /// </summary>
    public class ArrayType : Datatype
    {
        private readonly uint[]
            mrDimensionSizes;

        private readonly Datatype
            mrBaseType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.ArrayType"/> class.
        /// </summary>
        /// <param name="aHeader">IDatatypeHeader with the basic data about this type</param>
        /// <param name="aDimensionSizes">Size of each dimension in this array</param>
        /// <param name="aBaseType">Type of the data elements of this array</param>
        public ArrayType(
            IDatatypeHeader aHeader,
            uint[] aDimensionSizes,
            Datatype aBaseType) : this(aHeader.Size, aDimensionSizes, aBaseType)
        {
            if (aHeader.Class != DatatypeClass.Array)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.Array)}",
                    nameof(aHeader));
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.ArrayType"/> class.
        /// </summary>
        /// <param name="aSize">The size in bytes of this array</param>
        /// <param name="aDimensionSizes">Size of each dimension in this array</param>
        /// <param name="aBaseType">Type of the data elements of this array</param>
        public ArrayType(
            uint aSize,
            uint[] aDimensionSizes,
            Datatype aBaseType) : base(DatatypeClass.Array, aSize)
        {
            mrDimensionSizes = aDimensionSizes;
            mrBaseType = aBaseType;
        }

        protected override IEnumerable<object>
            EqualityMembers => mrDimensionSizes
                .Select(a => (object)a)
                .Concat(new[] { BaseType });
        
        /// <summary>
        /// This value is the size of each dimension of the array as stored in 
        /// the file. The first dimension stored in the list of dimensions is 
        /// the slowest changing dimension and the last dimension stored is the 
        /// fastest changing dimension.
        /// </summary>
        /// <value>The dimension sizes.</value>
        public uint[]
            DimensionSizes => mrDimensionSizes;

        /// <summary>
        /// Each array type is based on some parent type. The information
        /// for that parent type is described recursively by this field.
        /// </summary>
        /// <value>The type of the base.</value>
        public Datatype 
            BaseType => BaseType;

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.Array)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.Array)}");
            bool
                fReadPermutaions;
            switch(aHeader.Version)
            {
                case DatatypeVersion.Version1:
                    throw new InvalidDataException("Arrays are not supported in verion 1");
                    break;
                case DatatypeVersion.Version2:
                    fReadPermutaions = true;
                    break;
                default:
                    fReadPermutaions = false;
                    break;
            }

            byte
                fDimensionality = aReader.ReadByte();
            aReader.Seek(3, System.IO.SeekOrigin.Current); // Reserved bytes

            uint[]
                fDimeensions = Enumerable
                    .Range(0, fDimensionality)
                    .Select(a=>aReader.ReadUInt32())
                    .ToArray();

            if(fReadPermutaions)
            {
                if (!fDimeensions.All(a => a == aReader.ReadUInt32()))
                    throw new Exceptions.Hdf5UnsupportedFeature("Custom permutations not supported");
            }

            Datatype
            fBaseType = Datatype.Read ()


            throw new NotImplementedException();

        }
    }
}

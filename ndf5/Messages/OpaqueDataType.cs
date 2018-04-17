using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ndf5.Streams;
using ndf5.Objects;

namespace ndf5.Messages
{
    /// <summary>
    /// Opaque data type.
    /// </summary>
    public class OpaqueDataType : Datatype
    {
        private readonly string
            mrAsciiTag; 

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.OpaqueDataType"/> class.
        /// </summary>
        /// <param name="aHeader">IDatatypeHeader with the basic data about this type</param>
        /// <param name="aAsciiTag">A ASCII tag.</param>
        public OpaqueDataType(
            IDatatypeHeader aHeader,
            string aAsciiTag) : this(aHeader.Size, aAsciiTag)
        {
            if (aHeader.Class != DatatypeClass.Opaque)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.Opaque)}",
                    nameof(aHeader));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ndf5.Messages.OpaqueDataType"/> class.
        /// </summary>
        /// <param name="aSize">A size.</param>
        /// <param name="aAsciiTag">A ASCII tag.</param>
        public OpaqueDataType(
            uint aSize,
            string aAsciiTag) : base(DatatypeClass.Opaque, aSize)
        {
            byte[] 
                fTagByes = Encoding.ASCII.GetBytes(aAsciiTag);
            if (fTagByes.Length > byte.MaxValue)
                throw new ArgumentException("Tag is too long");
            mrAsciiTag = aAsciiTag;
        }

        /// <summary>
        /// A string that provides a description for the opaque type (Must use ASCII character set)
        /// </summary> 
        /// <value>The ASCII tag.</value>
        public string AsciiTag => mrAsciiTag;

        protected override IEnumerable<object>
            EqualityMembers => new object[] { AsciiTag };

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.Opaque)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.Opaque)}");

            int 
                fTagSize = (int)aHeader.Flags & byte.MaxValue;

            if (aLocalMessageSize.HasValue && aLocalMessageSize < fTagSize)
                throw new ArgumentException("Specified Local Message Size not long enough");
            
            byte[] 
                fTagBytes = new byte[fTagSize];

            int 
                fReadByteCount = aReader.Read(fTagBytes, 0, fTagSize);
            if (fReadByteCount != fTagSize)
                throw new EndOfStreamException("Could not read entire Opaque data type");

            String 
                fAsciiTag = Encoding.ASCII.GetString(fTagBytes).TrimEnd((char)0);

            aBytes = fTagSize;

            return new OpaqueDataType(
                aHeader,
                fAsciiTag);
        }
    }
}

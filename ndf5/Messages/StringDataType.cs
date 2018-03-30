using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Massage that decribes string data in an HDF5 file.
    /// </summary>
    public class StringDataType : Datatype
    {
        private StringDataType(
            IDatatypeHeader aHeader,
            StringPadding aStringPadding,
            StringEncoding aStringEncoding) : this(
                aHeader.Size, 
                aStringPadding,
                aStringEncoding)
        {
            if (aHeader.Class != DatatypeClass.String)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.String)}",
                    nameof(aHeader));
        }

        public StringDataType(
            uint aSize,
            StringPadding aStringPadding,
            StringEncoding aStringEncoding) : base(DatatypeClass.String, aSize)
        {
            StringPadding = aStringPadding;
            StringEncoding = aStringEncoding;
        }

        protected override IEnumerable<object> EqualityMembers => new object[]
        {
            StringPadding,
            StringEncoding
        };

        public StringPadding
            StringPadding { get; }


        public StringEncoding 
            StringEncoding { get; }

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.String)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.String)}");

            if(aLocalMessageSize.HasValue && aLocalMessageSize < 0)
                throw new ArgumentException("Specified Local Message Size not long enough");

            byte
                fStringPadding = (byte)(aHeader.Flags & 0x0F),
                fStringEncoding = (byte)((aHeader.Flags & 0xF0) >> 4);

            if (fStringPadding > (byte)StringPadding.SpacePad)
                throw new ArgumentException($"Invalid {nameof(StringPadding)}: {fStringPadding}");
            if (fStringEncoding > (byte)StringEncoding.UTF8)
                throw new ArgumentException($"Invalid {nameof(StringEncoding)}: {fStringEncoding}");

            aBytes = 0;// No addional Read
            return new StringDataType(
                aHeader,
                (StringPadding) fStringPadding,
                (StringEncoding) fStringEncoding);
        }
    }
}

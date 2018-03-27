using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Massage that decribes the time data in an HDF5 file.
    /// Times are number of seconds since the unix epoch.
    /// </summary>
    /// <seealso cref="https://support.hdfgroup.org/pubs/rfcs/RFC_Native_Time_Types.pdf"/>
    public class TimeDataType : Datatype
    {
        [Flags]
        private enum Flags : uint
        {
            None = 0x0,
            BigEndian = 0x1
        }

        private const int 
            mcAddionalSize = sizeof(uint);

        private TimeDataType(
            IDatatypeHeader aHeader,
            ByteOrdering aByteOrdering,
            uint aBitPrecision) : this(
                aHeader.Size,
                aByteOrdering,
                aBitPrecision)
        {
            if (aHeader.Class != DatatypeClass.Time)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.Time)}",
                    nameof(aHeader));
        }

        public TimeDataType(
            uint aSize,
            ByteOrdering aByteOrdering,
            uint aBitPrecision) : base(DatatypeClass.Time, aSize)
        {
            ByteOrdering = aByteOrdering;
            BitPrecision = aBitPrecision;
        }

        protected override IEnumerable<object> EqualityMembers => new object[]
        {
            ByteOrdering,
            BitPrecision
        };

		/// <summary>
		/// Gets the byte ordering for the described time value
		/// </summary>
		/// <value>The byte ordering.</value>
		public ByteOrdering
            ByteOrdering { get; }


        /// <summary>
        /// The number of bits of precision of the time value.
        /// </summary>
        /// <value>The bit offset.</value>
        public uint 
            BitPrecision { get; }

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.FixedPoint)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.Time)}");

            if(aLocalMessageSize.HasValue && aLocalMessageSize < mcAddionalSize)
                throw new ArgumentException("Specified Local Message Size not long enough");

            Flags fFlags = (Flags)aHeader.Flags;

            ByteOrdering
                fByteOrdering = fFlags.HasFlag(Flags.BigEndian)
                    ? ByteOrdering.BigEndian
                    : ByteOrdering.LittleEndian;
            uint
                fBitPrecision = aReader.ReadUInt32();
            aBytes = mcAddionalSize;
            return new TimeDataType(
                aHeader,
                fByteOrdering,
                fBitPrecision);
        }
    }
}

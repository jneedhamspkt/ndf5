using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Data type message which describes a bitfield
    /// </summary>
    public class BitFieldDataType : Datatype
    {
        [Flags]
        private enum Flags : uint
        {
            None = 0x0,
            BigEndian = 0x1,
            LowPad = 0x2,
            HighPad = 0x4
        }

        private const int
            mcAddionalSize = 2 * sizeof(ushort);

        private BitFieldDataType(
            IDatatypeHeader aHeader,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            ushort aBitOffset,
            ushort aBitPrecision) : this(
                aHeader.Size,
                aByteOrdering,
                aHighPaddingBit,
                aLowPaddingBit,
                aBitOffset,
                aBitPrecision)
        {
            if (aHeader.Class != DatatypeClass.BitField)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.BitField)}",
                    nameof(aHeader));
        }

        public BitFieldDataType(
            uint aSize,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            ushort aBitOffset,
            ushort aBitPrecision) : base(DatatypeClass.BitField, aSize)
        {
            if (!(aHighPaddingBit == 0 || aHighPaddingBit == 1))
                throw new ArgumentException(
                    "Padding bits must be 0 or 1",
                    nameof(aHighPaddingBit));
            if (!(aLowPaddingBit == 0 || aLowPaddingBit == 1))
                throw new ArgumentException(
                    "Padding bits must be 0 or 1",
                    nameof(aLowPaddingBit));

            ByteOrdering = aByteOrdering;
            HighPaddingBit = aHighPaddingBit;
            LowPaddingBit = aLowPaddingBit;
            BitOffset = aBitOffset;
            BitPrecision = aBitPrecision;
        }

        /// <summary>
        /// Gets the byte ordering for the described bit feild
        /// </summary>
        /// <value>The byte ordering.</value>
        public ByteOrdering
            ByteOrdering
        { get; }

        /// <summary>
        /// (Either zero or one) If a datum has unused bits at the low end (MSb), 
        /// then <c>HighPaddingBit</c> bit is copied to those locations.
        /// </summary>
        /// <value>The high padding bit.</value>
        public byte
            HighPaddingBit
        { get; }

        /// <summary>
        /// (Either zero or one) If a datum has unused bits at the low end (LSb), 
        /// then <c>LowPaddingBit</c> bit is copied to those locations.
        /// </summary>
        /// <value>The high padding bit.</value>
        public byte
            LowPaddingBit
        { get; }


        /// <summary>
        /// The bit offset of the first significant bit of the fixed-point value
        /// within the datatype. The bit offset specifies the number of bits 
        /// “to the right of” the value (which are set to the lo_pad bit value).
        /// </summary>
        /// <value>The bit offset.</value>
        public ushort
            BitOffset
        { get; }

        /// <summary>
        /// The number of bits of precision of the fixed-point value within the 
        /// datatype. This value, combined with the datatype element’s size and 
        /// the Bit Offset field specifies the number of bits “to the left of” 
        /// the value (which are set to the hi_pad bit value).
        /// </summary>
        /// <value>The bit offset.</value>
        public ushort
            BitPrecision
        { get; }

        protected override IEnumerable<object> EqualityMembers => new object[]
        {
            ByteOrdering,
            HighPaddingBit,
            LowPaddingBit,
            BitOffset,
            BitPrecision
        };

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.BitField)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.BitField)}");

            if (aLocalMessageSize.HasValue && aLocalMessageSize < mcAddionalSize)
                throw new ArgumentException("Specified Local Message Size not long enough");

            Flags fFlags = (Flags)aHeader.Flags;

            ByteOrdering
                fByteOrdering = fFlags.HasFlag(Flags.BigEndian)
                    ? ByteOrdering.BigEndian
                    : ByteOrdering.LittleEndian;
            byte
                fLowPad = (byte)(fFlags.HasFlag(Flags.LowPad) ? 1 : 0),
                fHighPad = (byte)(fFlags.HasFlag(Flags.HighPad) ? 1 : 0);

            ushort
                fBitOffset = aReader.ReadUInt16(),
                fBitPrecision = aReader.ReadUInt16();
            aBytes = mcAddionalSize;
            return new BitFieldDataType(
                aHeader,
                fByteOrdering,
                fHighPad,
                fLowPad,
                fBitOffset,
                fBitPrecision);
        }
    }
}

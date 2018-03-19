using System;

using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Massage that decribes the fixed point data in an HDF5 file
    /// </summary>
    public class FixedPointDataType : Datatype
    {
        [Flags]
        private enum Flags : uint
        {
            None = 0x0,
            BigEndian = 0x1,
            LowPad = 0x2,
            HighPad = 0x4,
            Signed = 0x8,
        }

        private const int 
            mcAddionalSize = 2 * sizeof(ushort);

        private FixedPointDataType(
            IDatatypeHeader aHeader,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            bool aIsSigned,
            ushort aBitOffset,
            ushort aBitPrecision) : this(
                aHeader.Size,
                aByteOrdering,
                aHighPaddingBit,
                aLowPaddingBit,
                aIsSigned,
                aBitOffset,
                aBitPrecision)
        {
            if (aHeader.Class != DatatypeClass.FixedPoint)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.FixedPoint)}",
                    nameof(aHeader));
        }

        public FixedPointDataType(
            uint aSize,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            bool aIsSigned,
            ushort aBitOffset,
            ushort aBitPrecision) : base(DatatypeClass.FixedPoint, aSize)
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
            IsSigned = aIsSigned;
            BitOffset = aBitOffset;
            BitPrecision = aBitPrecision;
        }


        /// <summary>
        /// Gets the byte ordering for the described fixed point numbers
        /// </summary>
        /// <value>The byte ordering.</value>
        public ByteOrdering
            ByteOrdering { get; }

        /// <summary>
        /// (Either zero or one) If a datum has unused bits at the low end (MSb), 
        /// then <c>HighPaddingBit</c> bit is copied to those locations.
        /// </summary>
        /// <value>The high padding bit.</value>
        public byte
            HighPaddingBit { get; }

        /// <summary>
        /// (Either zero or one) If a datum has unused bits at the low end (LSb), 
        /// then <c>LowPaddingBit</c> bit is copied to those locations.
        /// </summary>
        /// <value>The high padding bit.</value>
        public byte
            LowPaddingBit { get; }

        /// <summary>
        /// If true set then the fixed-point number is in 2’s complement form.
        /// </summary>
        /// <value><c>true</c> if is signed; otherwise, <c>false</c>.</value>
        public bool
            IsSigned { get; }

        /// <summary>
        /// The bit offset of the first significant bit of the fixed-point value
        /// within the datatype. The bit offset specifies the number of bits 
        /// “to the right of” the value (which are set to the lo_pad bit value).
        /// </summary>
        /// <value>The bit offset.</value>
        public ushort 
            BitOffset { get; }

        /// <summary>
        /// The number of bits of precision of the fixed-point value within the 
        /// datatype. This value, combined with the datatype element’s size and 
        /// the Bit Offset field specifies the number of bits “to the left of” 
        /// the value (which are set to the hi_pad bit value).
        /// </summary>
        /// <value>The bit offset.</value>
        public ushort 
            BitPrecision { get; }

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.FixedPoint)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.FixedPoint)}");

            if(aLocalMessageSize.HasValue && aLocalMessageSize < mcAddionalSize)
                throw new ArgumentException("Specified Local Message Size not long enough");

            Flags fFlags = (Flags)aHeader.Flags;

            ByteOrdering
                fByteOrdering = fFlags.HasFlag(Flags.BigEndian)
                    ? ByteOrdering.BigEndian
                    : ByteOrdering.LittleEndian;
            byte
                fLowPad = (byte)(fFlags.HasFlag(Flags.LowPad) ? 1 : 0),
                fHighPad = (byte)(fFlags.HasFlag(Flags.HighPad) ? 1 : 0);
            bool
                fIsSigned = fFlags.HasFlag(Flags.Signed);

            ushort
                fBitOffset = aReader.ReadUInt16(),
                fBitPrecision = aReader.ReadUInt16();
            aBytes = mcAddionalSize;
            return new FixedPointDataType(
                aHeader,
                fByteOrdering,
                fHighPad,
                fLowPad,
                fIsSigned,
                fBitOffset,
                fBitPrecision);
        }
    }
}

using System;
using System.Collections.Generic;
using ndf5.Objects;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Massage that decribes the fixed point data in an HDF5 file
    /// </summary>
    public class FloatingPointDataType : Datatype
    {
        [Flags]
        private enum Flags : uint
        {
            None = 0x0,
            ByteOrderLow = 0x01,
            ByteOrderHigh = 0x20,
            ByteOrderMask = ByteOrderLow | ByteOrderHigh,
            LowPad = 0x02,
            HighPad = 0x04,
            InternalPad = 0x08,
            MantissaNormalizationLow = 0x10,
            MantissaNormalizationHigh = 0x20,
            MantissaNormalizationMask = MantissaNormalizationLow | MantissaNormalizationHigh,
            SignLocationMask = 0xFF00
        }

        private const int 
            mcAddionalSize = 12;

        private FloatingPointDataType(
            IDatatypeHeader aHeader,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            byte aInternalPaddingBit,
            ushort aSignLocation,
            ushort aBitOffset,
            ushort aBitPrecision,
            byte aExponentLocation,
            byte aExponentSize,
            byte aMantissaLocation,
            byte aMantissaSize,
            uint aExponentBias) : this(
                aHeader.Size,
                aByteOrdering,
                aHighPaddingBit,
                aLowPaddingBit,
                aInternalPaddingBit,
                aSignLocation,
                aBitOffset,
                aBitPrecision,
                aExponentLocation,
                aExponentSize,
                aMantissaLocation,
                aMantissaSize,
                aExponentBias)
        {
            if (aHeader.Class != DatatypeClass.FloatingPoint)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.FixedPoint)}",
                    nameof(aHeader));
        }

        public FloatingPointDataType(
            uint aSize,
            ByteOrdering aByteOrdering,
            byte aHighPaddingBit,
            byte aLowPaddingBit,
            byte aInternalPaddingBit,
            ushort aSignLocation,
            ushort aBitOffset,
            ushort aBitPrecision,
            byte aExponentLocation,
            byte aExponentSize,
            byte aMantissaLocation,
            byte aMantissaSize,
            uint aExponentBias) : base(DatatypeClass.FloatingPoint, aSize)
        {
            if (!(aHighPaddingBit == 0 || aHighPaddingBit == 1))
                throw new ArgumentException(
                    "Padding bits must be 0 or 1", 
                    nameof(aHighPaddingBit));
            if (!(aLowPaddingBit == 0 || aLowPaddingBit == 1))
                throw new ArgumentException(
                    "Padding bits must be 0 or 1",
                    nameof(aLowPaddingBit));
            if (!(aInternalPaddingBit == 0 || aInternalPaddingBit == 1))
                throw new ArgumentException(
                    "Padding bits must be 0 or 1",
                    nameof(aLowPaddingBit));

            ByteOrdering = aByteOrdering;
            HighPaddingBit = aHighPaddingBit;
            LowPaddingBit = aLowPaddingBit;
            InternalPaddingBit = aInternalPaddingBit;
            SignLocation = aSignLocation;
            BitOffset = aBitOffset;
            BitPrecision = aBitPrecision;
            ExponentLocation = aExponentLocation;
            ExponentSize = aExponentSize;
            MantissaLocation = aMantissaLocation;
            MantissaSize = aMantissaSize;
            ExponentBias = aExponentBias;
        }

        protected override IEnumerable<object> EqualityMembers => new object[]
        {
            ByteOrdering,
            HighPaddingBit,
            LowPaddingBit,
            SignLocation,
            BitOffset,
            BitPrecision,
            ExponentLocation,
            ExponentSize,
            MantissaLocation,
            MantissaSize,
            ExponentBias
        };

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
        /// (Either zero or one) If a datum has unused bits inside the feild, 
        /// then <c>LowPaddingBit</c> bit is copied to those locations.
        /// </summary>
        /// <value>The high padding bit.</value>
        public byte
            InternalPaddingBit { get; }

        /// <summary>
        /// This is the bit position of the sign bit. Bits are numbered with the
        /// least significant bit zero.
        /// </summary>
        /// <value>The sign location.</value>
        public ushort
            SignLocation { get; }

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

        /// <summary>
        /// The bit position of the exponent field. Bits are numbered with the
        /// least significant bit number zero.
        /// </summary>
        /// <value>The exponent location.</value>
        public byte
            ExponentLocation{get;}

        /// <summary>
        /// The size of the exponent field in bits.
        /// </summary>
        /// <value>The size of the exponent.</value>
        public byte
            ExponentSize{get;}

        /// <summary>
        /// The bit position of the mantissa field. Bits are numbered with the 
        /// least significant bit number zero.
        /// </summary>
        /// <value>The mantissa location.</value>
        public byte
            MantissaLocation{get;}

        /// <summary>
        /// The size of the Mantissa field in bits.
        /// </summary>
        /// <value>The size of the exponent.</value>
        public byte
            MantissaSize{get;}

        /// <summary>
        /// The bias of the exponent field.
        /// </summary>
        /// <value>The exponent bias.</value>
        public uint
            ExponentBias {get;}

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.FloatingPoint)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.FloatingPoint)}");

            if(aLocalMessageSize.HasValue && aLocalMessageSize < mcAddionalSize)
                throw new ArgumentException("Specified Local Message Size not long enough");

            Flags fFlags = (Flags)aHeader.Flags;

            ByteOrdering
                fByteOrdering;
            Flags
                fByteOrderFlags = Flags.ByteOrderMask & fFlags;

            switch(fByteOrderFlags)
            {
                case Flags.None:
                    fByteOrdering = ByteOrdering.LittleEndian;
                    break;
                case Flags.ByteOrderLow:
                    fByteOrdering = ByteOrdering.BigEndian;
                    break;
                case Flags.ByteOrderMask:
                    fByteOrdering = ByteOrdering.VAXOrder;
                    break;
                default :
                    throw new System.IO.InvalidDataException("Unknown Byte Ordering");
            }

            byte
                fLowPad = (byte)(fFlags.HasFlag(Flags.LowPad) ? 1 : 0),
                fHighPad = (byte)(fFlags.HasFlag(Flags.HighPad) ? 1 : 0),
                fInternalPad = (byte)(fFlags.HasFlag(Flags.InternalPad) ? 1 : 0);


            MantissaNormalization
                fMantissaNormalization;
            byte
                fMantissaNormFlags = (byte)((byte)(Flags.MantissaNormalizationMask & fFlags) >> 4);
            if (fMantissaNormFlags > (byte)MantissaNormalization.MsbSetNotStorred)
                throw new System.IO.InvalidDataException("Unknown Mantissa Normalization");
            fMantissaNormalization = (MantissaNormalization)fMantissaNormFlags;

            ushort
                fSignLocation = (ushort) ((0x00FF00 & aHeader.Flags) >> 8);

            ushort
                fBitOffset = aReader.ReadUInt16(),
                fBitPrecision = aReader.ReadUInt16();
            byte
                fExponentLocation = aReader.ReadByte(),
                fExponentSize = aReader.ReadByte(),
                fMantissaLocation = aReader.ReadByte(),
                fMantissaSize = aReader.ReadByte();
            uint
                fExponentBias = aReader.ReadUInt32();

            aBytes = mcAddionalSize;
            return new FloatingPointDataType(
                aHeader,
                fByteOrdering,
                fHighPad,
                fLowPad,
                fInternalPad,
                fSignLocation,
                fBitOffset,
                fBitPrecision,
                fExponentLocation,
                fExponentSize,
                fMantissaLocation,
                fMantissaSize,
                fExponentBias);
        }
    }
}

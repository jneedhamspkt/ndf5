using System;
using System.Collections.Generic;
using System.Linq;
using ndf5.Streams;

namespace ndf5.Messages
{
    public abstract partial class Datatype : 
        Message,
        IEquatable<Datatype>
    {
        internal protected Datatype(
            DatatypeClass aClass,
            uint aSize) :
                base(MessageType.Datatype)
        {
            Class = aClass;
            Size = aSize;
        }

        protected Datatype(
            IDatatypeHeader aHeader) : 
            base(MessageType.Datatype)
        {
            Class = aHeader.Class;
            Size = aHeader.Size;
        }

        /// <summary>
        /// The type of data decribed by this Datatype message
        /// </summary>
        /// <value>The class.</value>
        public DatatypeClass Class { get; }

        /// <summary>
        /// The size of a datatype element in bytes.
        /// </summary>
        /// <value>The size.</value>
        public uint Size { get; }

        internal static Datatype Read(
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aLocalMessageSize.HasValue && aLocalMessageSize.Value < DatatypeHeader.HeaderSize)
                throw new ArgumentException("Specified Local Message Size not long enough");
            DatatypeHeader 
                fHeader = DatatypeHeader.Read(aReader);
            Datatype
                fMessage;
            long
                fAdditionalBytes;

            long?
                fBodySize = aLocalMessageSize.HasValue
                    ? (long?)aLocalMessageSize.Value - DatatypeHeader.HeaderSize
                    : null;

            switch(fHeader.Class)
            {
                case DatatypeClass.FixedPoint:
                    fMessage = FixedPointDataType.ReadMessage(
                        fHeader,
                        aReader,
                        fBodySize,
                        out fAdditionalBytes);
                    break;

                case DatatypeClass.FloatingPoint:
                    fMessage = FloatingPointDataType.ReadMessage(
                        fHeader,
                        aReader,
                        fBodySize,
                        out fAdditionalBytes);
                    break;


                default:
                    // We shoudl never git her, as header parsing should check 
                    // for known versions of the header
                    throw new Exception("Unexpected Version Type");
            }
            aBytes = DatatypeHeader.HeaderSize + fAdditionalBytes;
            return fMessage;
        }

        protected abstract IEnumerable<object> EqualityMembers { get; }

		public override int GetHashCode()
		{
            return new object[]{Class, Size}.Concat(EqualityMembers)
                       .Select(a => a.GetHashCode())
                       .Aggregate((aAcc, aNext) => (aAcc * 397) ^ aNext);
		}

		public override bool Equals(object obj)
		{
            Datatype fOther = obj as Datatype;
            return this.Equals(fOther);
		}

		public bool Equals(Datatype other)
        {
            if(other.IsNull())
                return false;
            if (this.Class != other.Class)
                return false;
            if (this.Size != other.Size)
                return false;
            object[]
                fMyMembers = EqualityMembers.ToArray(),
                fOtherMembers = other.EqualityMembers.ToArray();
            if (fMyMembers.Length != fOtherMembers.Length)
                return false;
            return fMyMembers
                .Zip(fOtherMembers, (a, b) => a.Equals(b))
                .All(a => a);
        }
    }
}

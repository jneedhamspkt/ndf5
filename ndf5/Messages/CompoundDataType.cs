using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ndf5.Streams;

namespace ndf5.Messages
{
    /// <summary>
    /// Compound data type.
    /// </summary>
    public class CompoundDataType : Datatype
    {
        /// <summary>
        /// Description infomration for a member of a CompoundDataType 
        /// instance.
        /// </summary>
        public class MemberEntry
        {
            public readonly string
                Name;

            public readonly Datatype
                Datatype;

            public readonly ulong
                ByteOffset;

            public MemberEntry(
                string aName,
                Datatype aDataType,
                ulong aByteOffset)
            {
                if (aName.IsNull())
                    throw new ArgumentNullException(nameof(aName));
                if (aDataType.IsNull())
                    throw new ArgumentNullException(nameof(aDataType));
                Name = aName;
                Datatype = aDataType;
                ByteOffset = aByteOffset;
            }

			public override bool Equals(object obj)
			{
                MemberEntry 
                    fOther = obj as MemberEntry;
                if (fOther.IsNull())
                    return false;
                return this.Name.Equals(fOther.Name) &&
                       this.Datatype.Equals(fOther.Datatype) &&
                       this.ByteOffset.Equals(fOther.ByteOffset);
			}

			public override int GetHashCode()
			{
                int 
                    fResult = Name.GetHashCode();
                fResult = (fResult * 397) ^ Datatype.GetHashCode(); 
                fResult = (fResult * 397) ^ ByteOffset.GetHashCode();
                return fResult;
			}
		}

        public CompoundDataType(
            IDatatypeHeader aHeader,
            IEnumerable<MemberEntry> aMembers) : this(aHeader.Size, aMembers)
        {
            if (aHeader.Class != DatatypeClass.Compound)
                throw new ArgumentException(
                    $"Header Class must be {nameof(DatatypeClass.Compound)}",
                    nameof(aHeader));
        }

        public CompoundDataType(
            uint aSize,
            IEnumerable<MemberEntry> aMembers) : base(DatatypeClass.Compound, aSize)
        {
            Members = aMembers.ToArray();
        }

        public IReadOnlyList<MemberEntry>
            Members { get; }

        protected override IEnumerable<object> 
            EqualityMembers => Members;

        internal static Datatype ReadMessage(
            DatatypeHeader aHeader,
            Hdf5Reader aReader,
            long? aLocalMessageSize,
            out long aBytes)
        {
            if (aHeader.Class != DatatypeClass.Opaque)
                throw new ArgumentException(
                    $"aHeader must be for type {nameof(DatatypeClass.Compound)}");

            int
                fTagSize = (int)aHeader.Flags & byte.MaxValue;

            if (aLocalMessageSize.HasValue && aLocalMessageSize < fTagSize)
                throw new ArgumentException("Specified Local Message Size not long enough");

            aBytes = fTagSize;

            MemberEntry
                fMembers = null;



            return new CompoundDataType(
                aHeader,
                fMembers);
        }
    }
}

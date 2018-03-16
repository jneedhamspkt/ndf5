using System;
namespace ndf5
{
    /// <summary>
    /// Class that encaplates a value decribed by a file superblock
    /// </summary>
    public abstract class UnsignedNumber : 
        IComparable,
        IComparable<UnsignedNumber>,
        IEquatable<UnsignedNumber>
    {
        protected internal readonly ulong
            Value;

        protected UnsignedNumber(ulong aValue)
        {
            this.Value = aValue;
        }

		public int CompareTo(object obj)
        {
            UnsignedNumber
                fOther = obj as UnsignedNumber;
            if (ReferenceEquals(null, fOther))
                return ((IComparable)Value).CompareTo(obj);;
            return Value.CompareTo(fOther.Value);
        }

        public int CompareTo(UnsignedNumber other) => 
            this.Value.CompareTo(Value);

        public override bool Equals(object obj)
		{
            UnsignedNumber
                fOther = obj as UnsignedNumber;
            if (ReferenceEquals(null, fOther))
                return Value.Equals(obj);
            return Value.Equals(fOther.Value);
		}

        public bool Equals(UnsignedNumber other) =>
            Value.Equals(other.Value);

        public override int GetHashCode() => 
            Value.GetHashCode();

		public override string ToString() =>
            $"{base.GetType()}(Value = {this.Value})";

        public static bool operator ==(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return true;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value == aRhs.Value;
        }

        public static bool operator !=(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return false;
            if (ReferenceEquals(aLhs, null))
                return true;
            if (ReferenceEquals(aRhs, null))
                return true;
            return aLhs.Value != aRhs.Value;
        }

        public static bool operator >(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return false;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value > aRhs.Value;
        }

        public static bool operator <(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return false;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value < aRhs.Value;
        }

        public static bool operator >=(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return true;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value >= aRhs.Value;
        }

        public static bool operator <=(
            UnsignedNumber aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return true;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value <= aRhs.Value;
        }

        public static implicit operator ulong?(UnsignedNumber obj) => 
            obj?.Value;

        public static explicit operator ulong (UnsignedNumber obj) =>
            obj.Value;

	}
}

using System;
namespace ndf5
{
    /// <summary>
    /// Class that encaplates a value decribed by a file superblock
    /// </summary>
    public abstract class SuperblockNumber : 
        IComparable,
        IComparable<SuperblockNumber>,
        IEquatable<SuperblockNumber>
    {
        protected internal readonly ulong
            Value;

        protected SuperblockNumber(ulong aValue)
        {
            this.Value = aValue;
        }

		public int CompareTo(object obj)
        {
            SuperblockNumber
                fOther = obj as SuperblockNumber;
            if (ReferenceEquals(null, fOther))
                return ((IComparable)Value).CompareTo(obj);;
            return Value.CompareTo(fOther.Value);
        }

        public int CompareTo(SuperblockNumber other) => 
            this.Value.CompareTo(Value);

        public override bool Equals(object obj)
		{
            SuperblockNumber
                fOther = obj as SuperblockNumber;
            if (ReferenceEquals(null, fOther))
                return false;
            return Value.Equals(fOther.Value);
		}

        public bool Equals(SuperblockNumber other) =>
            Value.Equals(other.Value);

        public override int GetHashCode() => 
            Value.GetHashCode();

		public override string ToString() =>
            $"{base.GetType()}(Value = {this.Value})";

        public static bool operator ==(
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
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
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
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
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
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
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
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
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
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
            SuperblockNumber aLhs,
            SuperblockNumber aRhs)
        {
            if (ReferenceEquals(aLhs, aRhs))
                return true;
            if (ReferenceEquals(aLhs, null))
                return false;
            if (ReferenceEquals(aRhs, null))
                return false;
            return aLhs.Value <= aRhs.Value;
        }

        public static explicit operator ushort?(SuperblockNumber obj) =>
            (ushort) obj?.Value;

        public static explicit operator uint?(SuperblockNumber obj) =>
            (uint) obj?.Value;

        public static explicit operator long?(SuperblockNumber obj) => 
            (long) obj?.Value;

        public static implicit operator ulong?(SuperblockNumber obj) => 
            obj?.Value;

	}
}

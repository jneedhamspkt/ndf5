using System;
namespace ndf5
{
    /// <summary>
    /// Decribes an offset into an hdf5 file. 
    /// </summary>
    public class Offset : UnsignedNumber
    {
        public Offset(ulong aValue) : base(aValue) { }

        public static Offset operator +(
            Offset aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value + aRhs.Value);
        }

        public static Offset operator -(
            Offset aLhs,
            UnsignedNumber aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value - aRhs.Value);
        }

        public static Offset operator +(
            Offset aLhs,
            ulong aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value + aRhs);
        }

        public static Offset operator -(
            Offset aLhs,
            ulong aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value - aRhs);
        }

        public static Offset operator +(
            Offset aLhs,
            uint aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value + aRhs);
        }

        public static Offset operator -(
            Offset aLhs,
            uint aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Offset(aLhs.Value - aRhs);
        }

        public static explicit operator Offset(ulong aOffset) =>
            new Offset(aOffset);
    }
}

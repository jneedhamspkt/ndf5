using System;
namespace ndf5
{
    /// <summary>
    /// Decribes a length (usually in bytes) inside an hdf5 file. 
    /// </summary>
    public class Length : SuperblockNumber
    {
        public Length(ulong aValue) : base(aValue) { }

        public static Length operator +(
            Length aLhs,
            Length aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Length(aLhs.Value + aRhs.Value);
        }

        public static Length operator -(
            Length aLhs,
            Length aRhs)
        {
            if (ReferenceEquals(aLhs, null) || ReferenceEquals(aRhs, null))
                throw new NullReferenceException();
            return new Length(aLhs.Value - aRhs.Value);
        }

        public static implicit operator Length(ulong aLength) =>
            new Length(aLength);
    }
}

using System;
namespace ndf5.Objects
{
    /// <summary>
    /// Class that decribes one dimension of a Data Space object
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// This value is the current size of the dimension of the data as
        /// stored in the file. The first dimension stored in the list of 
        /// dimensions is the slowest changing dimension and the last dimension
        /// stored is the fastest changing dimension.
        /// </summary>
        public readonly long
            Size;

        /// <summary>
        /// This value is the maximum size of the dimension of the data as 
        /// stored in the file. This value may be the special “unlimited” size (null) 
        /// which indicates that the data may expand along this dimension 
        /// indefinitely. If these values are not stored, the maximum size of 
        /// each dimension is assumed to be the dimension’s current size.
        /// </summary>
        public readonly long?
            MaxSize;

        public Dimension(
            long aSize,
            long? aMaxSize = null)
        {
            Size = aSize;
            MaxSize = aMaxSize;
        }

		public override bool Equals(object obj)
		{
            Dimension
                fOther = obj as Dimension;
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return (this.Size == fOther.Size) &&
                (this.MaxSize == fOther.MaxSize);
		}

		public override int GetHashCode()
		{
            return (MaxSize.GetHashCode() * 397) ^ Size.GetHashCode();
		}

		public override string ToString()
		{
            return $"{nameof(Dimension)}: Size = {this.Size}, MaxSize = {this.MaxSize}";
		}
	}
}

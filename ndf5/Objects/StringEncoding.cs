using System.Text;
namespace ndf5.Objects
{
    /// <summary>
    /// Type of encidding used in an HDF5 string data type
    /// </summary>
    public enum StringEncoding : byte
    {
        ASCII = 0,
        UTF8 = 1
    }

    public static class StringEncodingExtensions
    {
        /// <summary>
        /// Gets the encoding implmntation for e specidified <see cref="StringEncoding"/>
        /// </summary>
        /// <returns>The encoding.</returns>
        /// <param name="aValue">The StringEncoding to convert.</param>
        public static Encoding GetEncoding(this StringEncoding aValue)
        {
            switch(aValue)
            {
                case StringEncoding.ASCII: return Encoding.ASCII;
                case StringEncoding.UTF8: return Encoding.UTF8;
                default:
                    throw new System.ArgumentException($"Unknown Encoding: {aValue}");
            }
        }
    }

}

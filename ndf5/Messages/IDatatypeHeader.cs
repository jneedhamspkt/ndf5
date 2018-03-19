namespace ndf5.Messages
{
    /// <summary>
    /// Basic Data aobut a datat Type
    /// </summary>
    public interface IDatatypeHeader
    {
        /// <summary>
        /// Vesion of the data type message
        /// </summary>
        /// <value>The version.</value>
        DatatypeVersion 
            Version { get; }

        /// <summary>
        /// Class of the Data type message
        /// </summary>
        /// <value>The class.</value>
        DatatypeClass 
            Class { get; }

        /// <summary>
        /// The size of a datatype element in bytes.
        /// </summary>
        /// <value>The size.</value>
        uint
            Size { get; }
    }
}
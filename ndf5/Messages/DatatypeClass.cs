using System;
namespace ndf5.Messages
{
   
    /// <summary>
    /// The class of the datatype determines the format for the class bit 
    /// field and properties portion of the datatype message.
    /// </summary>
    public enum DatatypeClass : byte
    {
        FixedPoint = 0,
        FloatingPoint = 1,
        Time = 2,
        String = 3,
        BitField = 4,
        Opaque = 5,
        Compound = 6,
        Reference = 7,
        Enumerated = 8,
        VariableLength = 9,
        Array = 10
    }
}

using System;
namespace ndf5.Messages
{
    /// <summary>
    /// Message class for message objects marked nil 
    /// </summary>
    public class Nil : Message
    {
        public Nil() : base(MessageType.NIL) { }
    }
}

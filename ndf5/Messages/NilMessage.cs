using System;
namespace ndf5.Messages
{
    /// <summary>
    /// Message class for message objects marked nil 
    /// </summary>
    public class NilMessage : Message
    {
        public NilMessage() : base(MessageType.NIL)
        {
        }
    }
}

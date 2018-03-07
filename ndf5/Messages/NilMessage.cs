using System;
namespace ndf5.Messages
{
    public class NilMessage : Message
    {
        public NilMessage() : base(MessageType.NIL)
        {
        }
    }
}

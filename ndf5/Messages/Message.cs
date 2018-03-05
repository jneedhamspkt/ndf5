using System;
namespace ndf5.Messages
{
    public abstract class Message
    {
        public readonly MessageType
            MessageType;

        protected Message(MessageType aMessageType)
        {
        }

    }
}

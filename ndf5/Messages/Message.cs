using System;
using ndf5.Streams;

namespace ndf5.Messages
{
    public abstract class Message
    {
        public readonly MessageType
            MessageType;

        protected Message(MessageType aMessageType)
        {
            
        }

        /// <summary>
        /// Reads the message at the current location of an Hdf5Reader
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="aReader">A reader.</param>
        /// <param name="aReadFlags">Flags that describe how to handle this message</param>
        /// <param name="MessageType">Type of message to read</param>
        /// <param name="aLocalMessageSize">Number of bytes to read</param>
        /// <param name="aBytes">Number of bytes that were actually read</param>
        public static Message ReadMessage(
            Hdf5Reader aReader,
            MessageType aMessageType,
            MessageAttributeFlag aReadFlags)
        {
            long 
                aDummy;
            return ReadMessage(
                aReader,
                aMessageType,
                aReadFlags,
                null,
                out aDummy);
        }

        /// <summary>
        /// Reads the message at the current location of an Hdf5Reader
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="aReader">A reader.</param>
        /// <param name="aReadFlags">Flags that describe how to handle this message</param>
        /// <param name="aMessageType">Type of message to read</param>
        /// <param name="aLocalMessageSize">Number of bytes to read</param>
        /// <param name="aBytes">Number of bytes that were actually read</param>
        public static Message ReadMessage(
            Hdf5Reader aReader,
            MessageType aMessageType,
            MessageAttributeFlag aReadFlags,
            long? aLocalMessageSize,
            out long aBytes)
        {
            switch (aMessageType)
            {   
                case MessageType.NIL:
                    aBytes = 0;
                    return new Nil();

                case MessageType.Dataspace:
                    return Dataspace.Read(
                        aReader,
                        aLocalMessageSize,
                        out aBytes);

                default:
                    if(aReadFlags.HasFlag(MessageAttributeFlag.MustUnderstandToRead))
                    {
                        throw new Exceptions.RequiredMessageNotUnderstood(
                            $"Message type {aMessageType} must be understood for this object to be valid");
                    }
                    break;
            }

            throw new NotImplementedException();
        }

    }
}

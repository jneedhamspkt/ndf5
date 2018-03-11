using System;
namespace ndf5.Exceptions
{
    /// <summary>
    /// Exception thrown when Messages of an unsupported verion number are encountered
    /// </summary>
    public abstract class UnknownMessageVersion : Exception
    {
        public int
            UnknonwVersionNumber { get; }

        public Type
            UnknonMessageType { get; }

        protected UnknownMessageVersion(
            Type aMessageType,
            int aUnknownVersion,
            String aMessage = null,
            Exception aInternalException = null) : base(
                aMessage ?? $"Unknonw {aMessageType} Meassge version: {aUnknownVersion}",
                aInternalException)
        {
            UnknonMessageType = aMessageType;
            UnknonwVersionNumber = aUnknownVersion;
        }
            
    }
    /// <summary>
    /// Exception thrown when Messages of an unsupported verion number are encountered
    /// </summary>
    /// <typeparam name="tMessage">
    /// Type of the message with an unknown version number
    /// <typeparam name="tMessage"/>
    public class UnknownMessageVersion<tMessage> : UnknownMessageVersion 
        where tMessage : Messages.Message
    {
        public UnknownMessageVersion(
            int aUnknownVersion,
            String aMessage = null,
            Exception aInternalException = null) : base(
                typeof(tMessage),
                aUnknownVersion,
                aMessage,
                aInternalException)
        {
            //Nothing to do here
        }
    }
}

using System;
namespace ndf5.Messages
{
    internal class MessageTypeInfo
    {
        public readonly string
            Name; 

        public readonly int?
            Length;

        public readonly bool
            IsRepeatable;

        public readonly RequiredFlags
            RequiredFor;

        public MessageTypeInfo(
            string aName, 
            int? aLength = null,
            bool aRepeatable = false,
            RequiredFlags aRequiredFor = RequiredFlags.None)
        {
            Name = aName;
            Length = aLength;
            IsRepeatable = aRepeatable;
            RequiredFor = aRequiredFor;
        }
    }
}

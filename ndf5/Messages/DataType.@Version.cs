using System;
namespace ndf5.Messages
{
    partial class Datatype
    {
        protected enum Version : byte
        {
            /// <summary>
            /// Used by early versions of the library to encode compound 
            /// datatypes with explicit array fields.
            /// </summary>
            Version1,

            /// <summary>
            /// Used when an array datatype needs to be encoded.
            /// </summary>
            Version2,

            /// <summary>
            /// Used when a VAX byte-ordered type needs to be encoded. Packs
            /// various other datatype classes more efficiently also.
            /// </summary>
            Version3,
        }
    }
}

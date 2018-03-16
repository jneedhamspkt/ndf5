using System;
namespace ndf5
{
    internal static class ObjectExtensions
    {
        public static bool IsNull(this object obj) => 
            ReferenceEquals(null, obj);

        public static bool IsNotNull(this object obj) =>
            !ReferenceEquals(null, obj);
    }
}

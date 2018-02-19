using System;
namespace ndf5.Metadata
{
    internal interface ISuperBlockProvider
    {
        ISuperBlock SuperBlock { get; }
    }
}

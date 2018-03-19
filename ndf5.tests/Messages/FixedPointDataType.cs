using System;
using System.IO;

using ndf5.Streams;

using NUnit.Framework;

using uTest = ndf5.Messages.FixedPointDataType;
using uMessages = ndf5.Messages;


namespace ndf5.tests.Messages
{
    [TestFixture]
    public class FixedPointDataType
    {
        [Test, Pairwise]
        public void Test_Basic_Parsing(
            [Values] uMessages.DatatypeVersion 
                aVersion,
            [Values(uMessages.ByteOrdering.BigEndian, uMessages.ByteOrdering.LittleEndian)] 
                uMessages.ByteOrdering aByteOrdering,
            [Values(0, 1)]byte 
                aLowPaddingBit,
            [Values(0, 1)] byte 
                aHighPaddingBit,
            [Values] bool 
                aIsSigned,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint 
                aSize,
            [Values((ushort)0, (ushort)0xffff), Random(1)] ushort 
                aBitOffset,
            [Values((ushort)0, (ushort)0xffff), Random(1)] ushort 
                aBitPrecision)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)((byte)aVersion << 4));
                fWriter.Write((byte)(
                    (aByteOrdering == uMessages.ByteOrdering.BigEndian ? 1 : 0) |
                    (aLowPaddingBit << 1) |
                    (aHighPaddingBit << 2) |
                    (aIsSigned ? 0x8 : 0)));
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write(aSize);
                fWriter.Write(aBitOffset);
                fWriter.Write(aBitPrecision);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Moq.Mock<ndf5.Metadata.ISuperBlock>
                   fSuperblock = new Moq.Mock<ndf5.Metadata.ISuperBlock>(
                       Moq.MockBehavior.Loose);
                using (Hdf5Reader fReader = new Hdf5Reader(
                    fTestSource,
                    fSuperblock.Object))
                {
                    long 
                        fReadBytes;
                    uTest fResult = ndf5.Messages.Message.Read(
                        fReader,
                        uMessages.MessageType.Datatype,
                        uMessages.MessageAttributeFlag.None,
                        null,
                        out fReadBytes) as uTest;
                    Assert.That(
                        fResult,
                        Is.Not.Null,
                        "Incorrect Message Type returned");
                    Assert.That(
                        fReadBytes,
                        Is.EqualTo(12),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(12),
                        "Wrong number of bytes read");
                    Assert.That(
                        fResult.Class,
                        Is.EqualTo(uMessages.DatatypeClass.FixedPoint), 
                        "Incorrect Data class");
                    Assert.That(
                        fResult.ByteOrdering,
                        Is.EqualTo(aByteOrdering),
                        "Incorrect byte ordering");
                    Assert.That(
                        fResult.LowPaddingBit,
                        Is.EqualTo(aLowPaddingBit),
                        "Incorrect low bit padding");
                    Assert.That(
                        fResult.HighPaddingBit,
                        Is.EqualTo(aHighPaddingBit),
                        "Incorrect high bit padding");
                    Assert.That(
                        fResult.IsSigned,
                        Is.EqualTo(aIsSigned),
                        "Incorrect value for IsSigned");
                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(aSize),
                        "Incorrect Data Element Size");
                    Assert.That(
                        fResult.BitOffset,
                        Is.EqualTo(aBitOffset),
                        "Incorrect bit offset");
                    Assert.That(
                        fResult.BitPrecision,
                        Is.EqualTo(aBitPrecision),
                        "Incorrect bit offset");
                }
            }
        }
    }
}

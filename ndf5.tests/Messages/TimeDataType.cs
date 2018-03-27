using System;
using System.IO;

using ndf5.Streams;

using NUnit.Framework;

using uTest = ndf5.Messages.TimeDataType;
using uMessages = ndf5.Messages;
using ndf5.Objects;

namespace ndf5.tests.Messages
{
    [TestFixture(
        Description = "Verify usage of time data type messages",
        TestOf = typeof(uTest))]
    public class TimeDataType
    {
        [Test, Pairwise, TestOf(typeof(uTest))]
        public void Test_Basic_Parsing(
            [Values] uMessages.DatatypeVersion 
                aVersion,
            [Values(ByteOrdering.BigEndian, ByteOrdering.LittleEndian)] 
                ByteOrdering aByteOrdering,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint
                aSize,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint 
                aBitPrecision)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)(((byte)aVersion << 4) | 0x02));
                fWriter.Write((byte)(
                    (aByteOrdering == ByteOrdering.BigEndian ? 1 : 0)));
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);

                fWriter.Write(aSize);
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
                    uTest 
                        fExpected = new uTest(
                            aSize,
                            aByteOrdering,
                            aBitPrecision),
                        fResult = ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            null,
                            out fReadBytes) as uTest;

                    Assert.That(
                        fResult,
                        Is.EqualTo(fExpected),
                        "Equality check failed");

                    Assert.That(
                        fResult,
                        Is.Not.EqualTo(new uTest(
                            aSize,
                            aByteOrdering,
                            (ushort)(aBitPrecision + 1))),
                        "Inequality Check Failed");

                    Assert.That(
                        fResult,
                        Is.Not.EqualTo(null),
                        "Null Inequality Check Failed");

                    Assert.That(
                        fResult.GetHashCode(),
                        Is.EqualTo(fExpected.GetHashCode()),
                        "Hash Code Equality check failed");
                    
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
                        Is.EqualTo(uMessages.DatatypeClass.Time), 
                        "Incorrect Data class");
                    Assert.That(
                        fResult.ByteOrdering,
                        Is.EqualTo(aByteOrdering),
                        "Incorrect byte ordering");
                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(aSize),
                        "Incorrect Data Element Size");
                    Assert.That(
                        fResult.BitPrecision,
                        Is.EqualTo(aBitPrecision),
                        "Incorrect bit precision");
                }
            }
        }


        /// <summary>
        /// Tests a simple exaple of reading 16 bit fixed points
        /// </summary>
        [Test, TestOf(typeof(uTest))]
        public void Test_SixteenBit_Example()
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x12, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00,
                    0x20, 0x00, 0x00, 0x00,
                });

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
                        Is.EqualTo(uMessages.DatatypeClass.Time),
                        "Incorrect Data class");
                    Assert.That(
                        fResult.ByteOrdering,
                        Is.EqualTo(ByteOrdering.LittleEndian),
                        "Incorrect byte ordering");
                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(4),
                        "Incorrect Data Element Size");
                    Assert.That(
                        fResult.BitPrecision,
                        Is.EqualTo(32),
                        "Incorrect bit precision");
                }
            }
        }

        [Test, TestOf(typeof(uTest))]
        public void Short_Read_Checking_Test(
            [Range(0, 11)] int aNumBytesGiven)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x12, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                });
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
                    Assert.That(() =>
                    {
                        ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            aNumBytesGiven,
                            out fReadBytes);
                    }, Throws.ArgumentException,
                    "Length not checked");
                }
            }
        }

        [Test, TestOf(typeof(uTest))]
        public void Short_Stream_Check()
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x12, 0x00, 0x00,
                });
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
                    Assert.That(() =>
                    {
                        ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            12,
                            out fReadBytes);
                    }, Throws.Exception.TypeOf(typeof(System.IO.EndOfStreamException)),
                    "Length not checked");
                

                    //Add three more bytes to get past the header length
                    fWriter.Write(new byte[]{
                        0x12, 0x00, 0x00,
                    });

                    fTestSource.Seek(0, SeekOrigin.Begin);

                    Assert.That(() =>
                    {
                        ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            12,
                            out fReadBytes);
                    }, Throws.Exception.TypeOf(typeof(System.IO.EndOfStreamException)),
                    "Length not checked");
                }
            }
        }

    }
}

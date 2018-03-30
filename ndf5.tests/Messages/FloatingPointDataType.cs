using System;
using System.IO;

using ndf5.Streams;

using NUnit.Framework;

using uTest = ndf5.Messages.FloatingPointDataType;
using uMessages = ndf5.Messages;
using ndf5.Objects;

namespace ndf5.tests.Messages
{
    [TestFixture(
        Description = "Verify usage of floating point data type messages",
        TestOf = typeof(uTest))]
    public class FloatingPointDataType
    {
        [Test, Pairwise, TestOf(typeof(uTest))]
        public void Test_Basic_FloatingPointDataType_Parsing(
            [Values] uMessages.DatatypeVersion 
                aVersion,
            [Values] 
                ByteOrdering aByteOrdering,
            [Values(0, 1)]byte 
                aLowPaddingBit,
            [Values(0, 1)] byte 
                aHighPaddingBit,
            [Values(0, 1)] byte
                aInternalPaddingBit,
            [Values((byte)0, (byte)0xff), Random(1)] byte 
                aSignBitLocation,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint 
                aSize,
            [Values((ushort)0, (ushort)0xffff), Random(1)] ushort 
                aBitOffset,
            [Values((ushort)0, (ushort)0xffff), Random(1)] ushort 
                aBitPrecision,
            [Values((byte)0, (byte)0xff), Random(1)] byte 
                aExponentLocation,
            [Values((byte)0, (byte)0xff), Random(1)] byte 
                aExponentSize,
            [Values((byte)0, (byte)0xff), Random(1)] byte 
                aMantissaLocation,
            [Values((byte)0, (byte)0xff), Random(1)] byte 
                aMantissaSize,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint 
                aExponentBias)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)(((byte)aVersion << 4) | 0x01));
                fWriter.Write((byte)(
                    (aByteOrdering == ByteOrdering.BigEndian ? 0x01 : 0x00) |
                    (aByteOrdering == ByteOrdering.VAXOrder ? 0x21 : 0x00) |
                    (aLowPaddingBit << 1) |
                    (aHighPaddingBit << 2) |
                    (aInternalPaddingBit << 3)));
                fWriter.Write(aSignBitLocation);
                fWriter.Write((byte)0);
                fWriter.Write(aSize);
                fWriter.Write(aBitOffset);
                fWriter.Write(aBitPrecision);
                fWriter.Write(aExponentLocation);
                fWriter.Write(aExponentSize);
                fWriter.Write(aMantissaLocation);
                fWriter.Write(aMantissaSize);
                fWriter.Write(aExponentBias);

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
                            aHighPaddingBit,
                            aLowPaddingBit,
                            aInternalPaddingBit,
                            aSignBitLocation,
                            aBitOffset,
                            aBitPrecision,
                            aExponentLocation,
                            aExponentSize,
                            aMantissaLocation,
                            aMantissaSize,
                            aExponentBias),
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
                            aHighPaddingBit,
                            aLowPaddingBit,
                            aInternalPaddingBit,
                            aSignBitLocation,
                            aBitOffset,
                            aBitPrecision,
                            aExponentLocation,
                            aExponentSize,
                            aMantissaLocation,
                            aMantissaSize,
                            aExponentBias + 1)),
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
                        Is.EqualTo(20),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(20),
                        "Wrong number of bytes read");
                    Assert.That(
                        fResult.Class,
                        Is.EqualTo(uMessages.DatatypeClass.FloatingPoint), 
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
                        fResult.InternalPaddingBit,
                        Is.EqualTo(aInternalPaddingBit),
                        "Incorrect internal bit padding");
                    Assert.That(
                        fResult.SignLocation,
                        Is.EqualTo(aSignBitLocation),
                        "Incorrect Sign bit location");
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
                        "Incorrect bit precision");

                    Assert.That(
                        fResult.ExponentSize,
                        Is.EqualTo(aExponentSize),
                        "Incorrect Exponent Size");
                    Assert.That(
                        fResult.ExponentLocation,
                        Is.EqualTo(aExponentLocation),
                        "Incorrect Exponent Location");
                    Assert.That(
                        fResult.ExponentBias,
                        Is.EqualTo(aExponentBias),
                        "Incorrect Exponent Bias");

                    Assert.That(
                        fResult.MantissaSize,
                        Is.EqualTo(aMantissaSize),
                        "Incorrect Mantissa Size");
                    Assert.That(
                        fResult.MantissaLocation,
                        Is.EqualTo(aMantissaLocation),
                        "Incorrect Mantissa Location");
                }
            }
        }


        /// <summary>
        /// Tests a simple exaple of reading 16 bit fiexed points
        /// </summary>
        [Test, TestOf(typeof(uTest))]
        public void Test_Float_Example()
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x11, 0x10, 0x1F, 0x00,
                    0x04, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x20, 0x00,
                    0x17, 0x08, 0x00, 0x17,
                    0x7e, 0x00, 0x00, 0x00,
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
                        Is.EqualTo(20),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(20),
                        "Wrong number of bytes read");
                    Assert.That(
                        fResult.Class,
                        Is.EqualTo(uMessages.DatatypeClass.FloatingPoint),
                        "Incorrect Data class");
                    Assert.That(
                        fResult.ByteOrdering,
                        Is.EqualTo(ByteOrdering.LittleEndian),
                        "Incorrect byte ordering");
                    Assert.That(
                        fResult.LowPaddingBit,
                        Is.EqualTo(0),
                        "Incorrect low bit padding");
                    Assert.That(
                        fResult.HighPaddingBit,
                        Is.EqualTo(0),
                        "Incorrect high bit padding");
                    Assert.That(
                        fResult.InternalPaddingBit,
                        Is.EqualTo(0),
                        "Incorrect internal bit padding");
                    Assert.That(
                        fResult.SignLocation,
                        Is.EqualTo(31),
                        "Incorrect Sign bit location");
                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(4),
                        "Incorrect Data Element Size");
                    Assert.That(
                        fResult.BitOffset,
                        Is.EqualTo(0),
                        "Incorrect bit offset");
                    Assert.That(
                        fResult.BitPrecision,
                        Is.EqualTo(32),
                        "Incorrect bit precision");
                }
            }
        }

        [Test, TestOf(typeof(uTest))]
        public void Short_Read_Checking_Test(
            [Range(0, 19)] int aNumBytesGiven)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x11, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
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
    }
}

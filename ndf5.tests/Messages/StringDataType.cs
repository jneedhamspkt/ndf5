using System;
using System.IO;

using ndf5.Streams;

using NUnit.Framework;

using uTest = ndf5.Messages.StringDataType;
using uMessages = ndf5.Messages;
using ndf5.Objects;

namespace ndf5.tests.Messages
{
    [TestFixture(TestOf = typeof(uTest))]
    public class StringDataType
    {
        [Test, Combinatorial]
        public void Test_Basic_Parse(
            [Values] uMessages.DatatypeVersion
                aVersion,
            [Values] StringPadding
                aPadding,
            [Values] StringEncoding 
                aEncoding,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint
                aSize)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)(((byte)aVersion << 4) | 0x03));
                fWriter.Write((byte)((byte)aPadding | (((byte)aEncoding << 4))));
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write(aSize);

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
                            aSize, aPadding, aEncoding),
                        fResult = ndf5.Messages.Message.Read(
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
                        fResult,
                        Is.EqualTo(fExpected),
                        "Equality check failed");

                    Assert.That(
                        fResult,
                        Is.Not.EqualTo(new uTest(
                            aSize,
                            aPadding,
                            aEncoding == StringEncoding.ASCII
                                ? StringEncoding.UTF8
                                : StringEncoding.ASCII)),
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
                        fReadBytes,
                        Is.EqualTo(8),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(8),
                        "Wrong number of bytes read");

                    Assert.That(
                        fResult.Class,
                        Is.EqualTo(uMessages.DatatypeClass.String),
                        "Incorrect Data class");

                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(aSize),
                        "Incorrect Size");

                    Assert.That(
                        fResult.StringPadding,
                        Is.EqualTo(aPadding),
                        "Incorrect StringPadding");

                    Assert.That(
                        fResult.StringEncoding,
                        Is.EqualTo(aEncoding),
                        "Incorrect StringEncoding");
                }
            }
        }

        [Test]
        public void Test_Fixed_Read()
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write(new byte[]{
                    0x23, 0x10, 0x00, 0x00,
                    0x0D, 0x0C, 0x0B, 0x0A
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
                    uTest
                        fExpected = new uTest(
                        0x0A0B0C0D, StringPadding.NullTerminate, StringEncoding.UTF8),
                        fResult = ndf5.Messages.Message.Read(
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
                        fResult,
                        Is.EqualTo(fExpected),
                        "Equality check failed");

                    Assert.That(
                        fResult,
                        Is.Not.EqualTo(new uTest(
                            0x0ABBCCDD, StringPadding.NullTerminate, StringEncoding.UTF8)),
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
                        fReadBytes,
                        Is.EqualTo(8),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(8),
                        "Wrong number of bytes read");

                    Assert.That(
                        fResult.Class,
                        Is.EqualTo(uMessages.DatatypeClass.String),
                        "Incorrect Data class");

                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(0x0A0B0C0D),
                        "Incorrect Size");

                    Assert.That(
                        fResult.StringPadding,
                        Is.EqualTo(StringPadding.NullTerminate),
                        "Incorrect StringPadding");

                    Assert.That(
                        fResult.StringEncoding,
                        Is.EqualTo(StringEncoding.UTF8),
                        "Incorrect StringEncoding");
                }
            }
        }
    }
}

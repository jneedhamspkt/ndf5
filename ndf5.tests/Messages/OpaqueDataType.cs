using System;
using System.Text;
using System.IO;
using System.Linq;

using ndf5.Streams;

using NUnit.Framework;

using uTest = ndf5.Messages.OpaqueDataType;
using uMessages = ndf5.Messages;
using ndf5.Objects;

namespace ndf5.tests.Messages
{
    [TestFixture(Description = "Test of the 'Opaque' data type)", 
                 TestOf = typeof(uTest))]
    public class OpaqueDataType
    {
        [Test, Pairwise, TestOf(typeof(uTest))]
        public void Test_Basic_Parsing(
            [Values] uMessages.DatatypeVersion
                aVersion,
            [Values((uint)0, (uint)0xffffffff), Random(1)] uint
                aSize,
            [Values("", "a", "A", ".", "abc123", "!@#$1234%^&*()67890")] string 
                aName)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                byte[] 
                    fTagBytes = Encoding.ASCII.GetBytes(aName);

                int 
                    fPadding = 8 - (fTagBytes.Length & 0x7),
                    fLength = fTagBytes.Length + fPadding;

                //Verifying this test
                Assert.That(fLength % 8, Is.EqualTo(0));

                fWriter.Write((byte)(((byte)aVersion << 4) | (byte)0x05));
                fWriter.Write((byte)fLength);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write(aSize);
                fWriter.Write(fTagBytes);
                fWriter.Write(Enumerable.Repeat((byte)0, fPadding).ToArray());

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
                            aName),
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
                            aName + "Fail")), 
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
                        Is.EqualTo(fLength + 8),
                        "Wrong number of bytes read");
                    Assert.That(
                        fTestSource.Position,
                        Is.EqualTo(fLength + 8),
                        "Wrong number of bytes read");

                    Assert.That(
                        fResult.Size,
                        Is.EqualTo(aSize),
                        "Incorrect Data Element Size");
                    Assert.That(
                        fResult.AsciiTag,
                        Is.EqualTo(aName),
                        "Incorrect Ascii Tag");
                }
            }
        }
    }
}

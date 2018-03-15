using System;
using System.IO;
using NUnit.Framework;
using Moq;

using SuperBlock = ndf5.Metadata.ISuperBlock;
using TestStream = ndf5.Streams.Hdf5Reader;
namespace ndf5.tests.Streams
{
    [TestFixture]
    [TestOf(typeof(TestStream))]
    public static class Hdf5Reader
    {
        [Test]
        public static void TestBasicReader ()
        {
            Mock<SuperBlock>
                fSuperblock = new Mock<SuperBlock>(MockBehavior.Loose);
            fSuperblock.Setup((a) => a.SizeOfLengths).Returns(8);
            fSuperblock.Setup((a) => a.SizeOfOffsets).Returns(8);

            using(MemoryStream fStream = new MemoryStream())
            using (TestStream fTestStream = new TestStream(fStream, fSuperblock.Object))
            {
                Assert.That(fTestStream.CanRead, Is.True, "Should be able to read");
                Assert.That(fTestStream.CanWrite, Is.False, "Should not be able to write");
            }
        }

        [Test]
        public static void Test_Short_Reads()
        {
            Mock<SuperBlock>
                fSuperblock = new Mock<SuperBlock>(MockBehavior.Loose);
            fSuperblock.Setup((a) => a.SizeOfLengths).Returns(2);
            fSuperblock.Setup((a) => a.SizeOfOffsets).Returns(2);

            using (MemoryStream fStream = new MemoryStream(new byte[]{0xA, 0xB, 0xC, 0xD}))
            using (TestStream fTestStream = new TestStream(fStream, fSuperblock.Object))
            {
                Assert.That(fTestStream.ReadLength(), Is.EqualTo(new Length(0x0b0a)), "Should read little endian short");
                Assert.That(fTestStream.ReadOffset(), Is.EqualTo(new Offset(0x0d0c)), "Should read little endian short");

                fStream.Seek(0, SeekOrigin.Begin);
                Assert.That(fTestStream.ReadUInt16(), Is.EqualTo(0x0b0a), "Should read little endian short");
                Assert.That(fTestStream.ReadUInt16(), Is.EqualTo(0x0d0c), "Should read little endian short");
            }
        }

        [Test]
        public static void Test_Int_Reads()
        {
            Mock<SuperBlock>
                fSuperblock = new Mock<SuperBlock>(MockBehavior.Loose);
            fSuperblock.Setup((a) => a.SizeOfLengths).Returns(4);
            fSuperblock.Setup((a) => a.SizeOfOffsets).Returns(4);

            using (MemoryStream fStream = new MemoryStream(new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 }))
            using (TestStream fTestStream = new TestStream(fStream, fSuperblock.Object))
            {
                Assert.That(fTestStream.ReadLength(), Is.EqualTo(new Length(0x03020100)), "Should read little endian uint");
                Assert.That(fTestStream.ReadOffset(), Is.EqualTo(new Offset(0x07060504)), "Should read little endian uint");

                fStream.Seek(0, SeekOrigin.Begin);
                Assert.That(fTestStream.ReadUInt32(), Is.EqualTo(0x03020100), "Should read little endian uint");
                Assert.That(fTestStream.ReadUInt32(), Is.EqualTo(0x07060504), "Should read little endian uint");
            }
        }

        [Test]
        public static void Test_Long_Reads()
        {
            Mock<SuperBlock>
                fSuperblock = new Mock<SuperBlock>(MockBehavior.Loose);
            fSuperblock.Setup((a) => a.SizeOfLengths).Returns(8);
            fSuperblock.Setup((a) => a.SizeOfOffsets).Returns(8);

            using (MemoryStream fStream = new MemoryStream(new byte[] {
                0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf}))
            using (TestStream fTestStream = new TestStream(fStream, fSuperblock.Object))
            {
                Assert.That(fTestStream.ReadLength(), Is.EqualTo(new Length(0x0706050403020100)), "Should read little endian long");
                Assert.That(fTestStream.ReadOffset(), Is.EqualTo(new Offset(0x0f0e0d0c0b0a0908)), "Should read little endian long");
            
                fStream.Seek(0, SeekOrigin.Begin);
                Assert.That(fTestStream.ReadUInt64(), Is.EqualTo(0x0706050403020100), "Should read little endian long");
                Assert.That(fTestStream.ReadUInt64(), Is.EqualTo(0x0f0e0d0c0b0a0908), "Should read little endian long");
            }
        }

        [Test]
        public static void Test_Mixed_Lengths()
        {
            Mock<SuperBlock>
                fSuperblock = new Mock<SuperBlock>(MockBehavior.Loose);
            fSuperblock.Setup((a) => a.SizeOfLengths).Returns(8);
            fSuperblock.Setup((a) => a.SizeOfOffsets).Returns(2);

            using (MemoryStream fStream = new MemoryStream(new byte[] {
                0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9}))
            using (TestStream fTestStream = new TestStream(fStream, fSuperblock.Object))
            {
                Assert.That(fTestStream.ReadLength(), Is.EqualTo(new Length(0x0706050403020100)), "Should read little endian long");
                Assert.That(fTestStream.ReadOffset(), Is.EqualTo(new Offset(0x0908)), "Should read little endian short");
            }
        }

    }
}

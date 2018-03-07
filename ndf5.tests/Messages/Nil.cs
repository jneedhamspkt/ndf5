using System;
using ndf5.Streams;
using NUnit.Framework;
using Moq;

using uMessages = ndf5.Messages;

namespace ndf5.tests.Messages
{
    [TestFixture]
    public class Nil
    {
        private Mock<IHdfStreamProvider>
            NilStreamProvider { get; set; }

        [SetUp]
        public void Setup()
        {
            NilStreamProvider = new Mock<IHdfStreamProvider>(MockBehavior.Loose);
        }

        [TearDown]
        public void TearDown()
        {
            NilStreamProvider = null;
        }

        [Test]
        public void TestBasicNilRead(
            [Values] uMessages.MessageAttributeFlag aFlags,
            [Values(2, 4, 8)] int aOffsetBytes,
            [Values(2, 4, 8)] int aLengthBytes)
        {
            Mock<ndf5.Metadata.ISuperBlock>
                fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);    
            fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)aOffsetBytes);
            fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);
            NilStreamProvider
                .Setup(a => a.GetStream(It.IsAny<StreamRequestArguments>()))
                .Returns(new Hdf5Reader(new System.IO.MemoryStream(new byte[0]),fSuperBlock.Object));

            uMessages.Message fShortTest = ndf5.Messages.Message.ReadMessage(
                NilStreamProvider.Object.GetReader(),
                uMessages.MessageType.NIL,
                aFlags);

            Assert.That(fShortTest, Is.InstanceOf(typeof(uMessages.NilMessage)));
        }

        [Test]
        public void TestBasicLengthedNilRead(
            [Values] uMessages.MessageAttributeFlag aFlags,
            [Values(2, 4, 8)] int aOffsetBytes,
            [Values(2, 4, 8)] int aLengthBytes,
            [Values(0, 2, 4, 8, 255, 2047)] int aReadBytes)
        {
            Mock<ndf5.Metadata.ISuperBlock>
                fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
            fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)aOffsetBytes);
            fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);
            NilStreamProvider
                .Setup(a => a.GetStream(It.IsAny<StreamRequestArguments>()))
                .Returns(new Hdf5Reader(new System.IO.MemoryStream(new byte[0]), fSuperBlock.Object));

            long fRead;
            uMessages.Message fShortTest = ndf5.Messages.Message.ReadMessage(
                NilStreamProvider.Object.GetReader(),
                uMessages.MessageType.NIL,
                aFlags,
                (long)aReadBytes,
                out fRead);

            Assert.That(fShortTest, Is.InstanceOf(typeof(uMessages.NilMessage)), 
                "Incorrect Message type parsed");
            Assert.That(fRead, Is.EqualTo(0), 
                "Zero bytes should be read");
        }
    }
}

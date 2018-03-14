using System;
using System.IO;
using System.Linq;
using ndf5.Streams;
using NUnit.Framework;
using Moq;

using uMessages = ndf5.Messages;
using uObjects = ndf5.Objects;

namespace ndf5.tests.Messages
{
    [TestFixture]
    public class LinkInfo
    {
        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void No_Creation_Tracking_Compact(
            [Values(2, 4, 8)] int aOffsetBytes)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<ulong?> fWriteOffset = null;
                switch (aOffsetBytes)
                {
                    case 2:
                        fWriteOffset = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteOffset = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteOffset = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)0);     //Version 0
                fWriter.Write((byte)0);     //Flags
                fWriter.Write((byte)0x0);   //Reserved
                fWriter.Write((byte)0x0);   //Reserved
                fWriteOffset(null);         //Heap 
                fWriteOffset(null);         //Name

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)aOffsetBytes);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)8);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Test read bytes checking
                long fRead;
                Assert.That(() =>
                    {
                        ndf5.Messages.Message.ReadMessage(
                            fReader,
                            uMessages.MessageType.LinkInfo,
                            uMessages.MessageAttributeFlag.None,
                            5,
                            out fRead);
                    }, 
                    Throws.ArgumentException,
                    "Read bytes not checked properly");

                fTestSource.Seek(0, SeekOrigin.Begin);

                //Act
                uMessages.Message fTest = ndf5.Messages.Message.ReadMessage(
                    fReader,
                    uMessages.MessageType.LinkInfo,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.LinkInfo
                    fResult = fTest as uMessages.LinkInfo;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(
                    fResult.IsCreationOrderTracked,
                    Is.False,
                    "Incorrect flag for IsCreationOrderTracked parsed");
                Assert.That(
                    fResult.IsCreationOrderIndexed,
                    Is.False, 
                    "Incorrect flag for IsCreationOrderIndexed parsed");
                Assert.That(
                    fResult.MaximumCreationIndex,
                    Is.Null,
                    "Incorrect value for MaximumCreationIndex parsed");
                Assert.That(
                    fResult.FractalHeapAddress,
                    Is.Null,
                    "Incorrect value for FractalHeapAddress parsed");
                Assert.That(
                    fResult.NameIndexBTreeAddress,
                    Is.Null,
                    "Incorrect value for NameIndexBTreeAddress parsed");
                Assert.That(
                    fResult.CreationOrderIndexBTreeAddress,
                    Is.Null,
                    "Incorrect value for CreationOrderIndexBTreeAddress parsed");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void No_Creation_Tracking_NonCompact(
            [Values(2, 4, 8)] int aOffsetBytes,
            [Random(2)] byte aHeapAddr,
            [Random(2)] byte aTreeAddr)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<ulong?> fWriteOffset = null;
                switch (aOffsetBytes)
                {
                    case 2:
                        fWriteOffset = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteOffset = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteOffset = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)0);     //Version 0
                fWriter.Write((byte)0);     //Flags
                fWriter.Write((byte)0x0);   //Reserved
                fWriter.Write((byte)0x0);   //Reserved
                fWriteOffset(aHeapAddr);         //Heap 
                fWriteOffset(aHeapAddr);         //Name

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)aOffsetBytes);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)8);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Test read bytes checking
                long fRead;
                Assert.That(() =>
                {
                    ndf5.Messages.Message.ReadMessage(
                        fReader,
                        uMessages.MessageType.LinkInfo,
                        uMessages.MessageAttributeFlag.None,
                        5,
                        out fRead);
                },
                    Throws.ArgumentException,
                    "Read bytes not checked properly");

                fTestSource.Seek(0, SeekOrigin.Begin);

                //Act
                uMessages.Message fTest = ndf5.Messages.Message.ReadMessage(
                    fReader,
                    uMessages.MessageType.LinkInfo,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.LinkInfo
                    fResult = fTest as uMessages.LinkInfo;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(
                    fResult.IsCreationOrderTracked,
                    Is.False,
                    "Incorrect flag for IsCreationOrderTracked parsed");
                Assert.That(
                    fResult.IsCreationOrderIndexed,
                    Is.False,
                    "Incorrect flag for IsCreationOrderIndexed parsed");
                Assert.That(
                    fResult.MaximumCreationIndex,
                    Is.Null,
                    "Incorrect value for MaximumCreationIndex parsed");
                Assert.That(
                    fResult.FractalHeapAddress,
                    Is.EqualTo(aHeapAddr),
                    "Incorrect value for FractalHeapAddress parsed");
                Assert.That(
                    fResult.NameIndexBTreeAddress,
                    Is.EqualTo(aTreeAddr),
                    "Incorrect value for NameIndexBTreeAddress parsed");
                Assert.That(
                    fResult.CreationOrderIndexBTreeAddress,
                    Is.Null,
                    "Incorrect value for CreationOrderIndexBTreeAddress parsed");
            }
        }


    }
}

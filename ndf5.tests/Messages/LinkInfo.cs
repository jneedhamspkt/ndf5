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
        [Test, TestOf(typeof(uMessages.LinkInfo))]
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
                        ndf5.Messages.Message.Read(
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
                uMessages.Message fTest = ndf5.Messages.Message.Read(
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
                    fRead, Is.EqualTo(
                    fTestSource.Position),
                    "Incorrect Read bytes");
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

        [Test, TestOf(typeof(uMessages.LinkInfo))]
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
                fWriteOffset(aTreeAddr);         //Name

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
                    ndf5.Messages.Message.Read(
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
                uMessages.Message fTest = ndf5.Messages.Message.Read(
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
                    fRead, Is.EqualTo(
                    fTestSource.Position),
                    "Incorrect Read bytes");
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
                    (ulong)fResult.FractalHeapAddress,
                    Is.EqualTo(aHeapAddr),
                    "Incorrect value for FractalHeapAddress parsed");
                Assert.That(
                    (ulong)fResult.NameIndexBTreeAddress,
                    Is.EqualTo(aTreeAddr),
                    "Incorrect value for NameIndexBTreeAddress parsed");
                Assert.That(
                    fResult.CreationOrderIndexBTreeAddress,
                    Is.Null,
                    "Incorrect value for CreationOrderIndexBTreeAddress parsed");
            }
        }

        [Test, TestOf(typeof(uMessages.LinkInfo))]
        public void Full_Creation_Tracking_Compact(
            [Values(2, 4, 8)] int aOffsetBytes,
            [Random (2)] ulong aMaxCreation)
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
                fWriter.Write((byte)0x0);       //Version 0
                fWriter.Write((byte)0x3);       //Flags
                fWriter.Write((byte)0x0);       //Reserved
                fWriter.Write((byte)0x0);       //Reserved
                fWriter.Write(aMaxCreation);     //MaxCreationOrder
                fWriteOffset(null);             //Heap 
                fWriteOffset(null);             //Name
                fWriteOffset(null);             //Creation order

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
                    ndf5.Messages.Message.Read(
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
                uMessages.Message fTest = ndf5.Messages.Message.Read(
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
                    fRead, Is.EqualTo(
                    fTestSource.Position),
                    "Incorrect Read bytes");
                Assert.That(
                    fResult.IsCreationOrderTracked,
                    Is.True,
                    "Incorrect flag for IsCreationOrderTracked parsed");
                Assert.That(
                    fResult.IsCreationOrderIndexed,
                    Is.True,
                    "Incorrect flag for IsCreationOrderIndexed parsed");
                Assert.That(
                    fResult.MaximumCreationIndex,
                    Is.EqualTo(aMaxCreation),
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

        [Test, TestOf(typeof(uMessages.LinkInfo))]
        public void Full_Creation_Tracking_NonCompact(
            [Values(2, 4, 8)] int aOffsetBytes,
            [Random(2)] ushort aHeapAddr,
            [Random(2)] ushort aTreeAddr,
            [Random(2)] ushort aCreateTreeAddr,
            [Random(2)] ulong aMaxCreation)
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
                fWriter.Write((byte)0);         // Version 0
                fWriter.Write((byte)0x3);       // Flags
                fWriter.Write((byte)0x0);       // Reserved
                fWriter.Write((byte)0x0);       // Reserved
                fWriter.Write(aMaxCreation);    // Max  Creation
                fWriteOffset(aHeapAddr);        // Heap 
                fWriteOffset(aTreeAddr);        // Name
                fWriteOffset(aCreateTreeAddr);   // Creation Index

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
                    ndf5.Messages.Message.Read(
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
                uMessages.Message fTest = ndf5.Messages.Message.Read(
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
                    fRead, Is.EqualTo(
                    fTestSource.Position),
                    "Incorrect Read bytes");
                Assert.That(
                    fResult.IsCreationOrderTracked,
                    Is.True,
                    "Incorrect flag for IsCreationOrderTracked parsed");
                Assert.That(
                    fResult.IsCreationOrderIndexed,
                    Is.True,
                    "Incorrect flag for IsCreationOrderIndexed parsed");
                Assert.That(
                    fResult.MaximumCreationIndex,
                    Is.EqualTo(aMaxCreation),
                    "Incorrect value for MaximumCreationIndex parsed");
                Assert.That(
                    (ulong)fResult.FractalHeapAddress,
                    Is.EqualTo(aHeapAddr),
                    "Incorrect value for FractalHeapAddress parsed");
                Assert.That(
                    (ulong)fResult.NameIndexBTreeAddress,
                    Is.EqualTo(aTreeAddr),
                    "Incorrect value for NameIndexBTreeAddress parsed");
                Assert.That(
                    fResult.CreationOrderIndexBTreeAddress,
                    Is.EqualTo((Offset)aCreateTreeAddr),
                    "Incorrect value for CreationOrderIndexBTreeAddress parsed");
            }
        }
    }
}

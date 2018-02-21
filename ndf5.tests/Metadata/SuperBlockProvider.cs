using System;
using System.IO;
using NUnit.Framework;
using Moq;

namespace ndf5.tests.Metadata
{
    [TestFixture]
    public class SuperBlockProvider
    {
        [Test, Combinatorial]
        public void Test_Basic_V0_and_V1_Parse(
            [Values(0,1)] int aVerson,
            [Random (0, 255, 1)] int aFsVersion,
            [Random (0, 255, 1)] int aRgVersion,
            [Random(0, 255, 1)] int aShvVersion,
            [Values(2,4,8)] int aOffset,
            [Values(2,4,8)] int aLength,
            [Random(0, 65535, 1)] int aLeafK,
            [Random(0, 65535, 1)] int aInternalK,
            [Random(0, 65535, 1)] int aStorageK)
        {
            using(MemoryStream fBuffer = new MemoryStream())
            {
                //Arrange
                Mock<ndf5.Streams.IStreamProvider>
                         fStreamProvider = new Mock<Streams.IStreamProvider>(MockBehavior.Loose);
                fStreamProvider.Setup(a=>a.GetStream(It.IsAny<ndf5.Streams.StreamRequestArguments>())).Returns(fBuffer);
                    
                    
                using(BinaryWriter fwriter = new BinaryWriter(fBuffer))
                {
                    ndf5.Metadata.FormatSignatureAndVersionInfo
                        fSig = new ndf5.Metadata.FormatSignatureAndVersionInfo((byte)aVerson, 0);
                    byte[]
                        fFormatBlock = fSig.AsBytes;
                    fwriter.Write(fFormatBlock);
                    fwriter.Write((byte) aFsVersion);
                    fwriter.Write((byte) aRgVersion);
                    fwriter.Write((byte) 0);

                    fwriter.Write((byte) aShvVersion);
                    fwriter.Write((byte) aOffset);
                    fwriter.Write((byte) aLength);
                    fwriter.Write((byte) 0);

                    fwriter.Write((ushort)aLeafK);
                    fwriter.Write((ushort)aInternalK);
                
                    fwriter.Write((uint) 0x0);

                    if (aVerson == 1)
                    {
                        fwriter.Write((ushort)aStorageK);
                        fwriter.Write((ushort)0);
                    }

                    switch(aOffset)
                    {
                        case 2:
                            fwriter.Write((ushort)0x1234);
                            fwriter.Write((ushort)0x5678);
                            fwriter.Write((ushort)0x9ABC);
                            fwriter.Write((ushort)0xDEF0);
                            break;

                        case 4:
                            fwriter.Write((uint)0x10203040);
                            fwriter.Write((uint)0x05060708);
                            fwriter.Write((uint)0x90A0B0C0);
                            fwriter.Write((uint)0x0D0E0F00);
                            break;

                        case 8:
                            fwriter.Write((ulong)0x10203040AAAAAAAA);
                            fwriter.Write((ulong)0x4BBBBBBB05060708);
                            fwriter.Write((ulong)0x50A0B0C0CCCCCCCC);
                            fwriter.Write((ulong)0x1DDDDDDD0D0E0F00);
                            break;
                    }

                    fwriter.Flush();


                    //Act
                    ndf5.Metadata.SuperBlockProvider
                        fSuperblockPorvider = new ndf5.Metadata.SuperBlockProvider(
                            fStreamProvider.Object,
                            fSig);

                    ndf5.Metadata.ISuperBlock 
                        fSuperBlock = fSuperblockPorvider.SuperBlock;

                    //Assert
                    Assert.That(fSuperBlock.SuperBlockVersion, Is.EqualTo(aVerson), "Incorrect Superblock Version");
                    Assert.That(fSuperBlock.FreeSpaceStorageVersion, Is.EqualTo(aFsVersion), "Incorrect Version # of File’s Free Space Storage");
                    Assert.That(fSuperBlock.RootGroupSymbolTableVersion, Is.EqualTo(aRgVersion), "Version # of Root Group Symbol Table Entry ");
                    Assert.That(fSuperBlock.SharedHeaderMessageFormatVersion, Is.EqualTo(aShvVersion), "Incorrect Version Number of Shared Header Message Format");
                    Assert.That(fSuperBlock.SizeOfOffsets, Is.EqualTo(aOffset), "Incorrect Size of Offsets");
                    Assert.That(fSuperBlock.SizeOfLengths, Is.EqualTo(aLength), "Incorrect Size of Lengths");

                    Assert.That(fSuperBlock.GroupLeafNodeK, Is.EqualTo(aLeafK), "Incorrect Group Leaf Node K");
                    Assert.That(fSuperBlock.GroupInternalNodeK, Is.EqualTo(aInternalK), "Incorrect Group Internal Node K");

                    if (aVerson == 1)
                        Assert.That(fSuperBlock.IndexedStorageInternalNodeK, Is.EqualTo(aStorageK), "Incorrect Indexed Storage Internal Node K");

                    switch(aOffset)
                    {
                        case 2:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x1234), "Incorrect Base Address");
                            Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x5678), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x9ABC), "Incorrect End of File Address");
                            Assert.That(fSuperBlock.DriverInformationBlockAddress, Is.EqualTo(0xDEF0), "Incorrect Driver Information Block Address");
                            break;

                        case 4:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x10203040), "Incorrect Base Address");
                            Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x05060708), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x90A0B0C0), "Incorrect End of File Address");
                            Assert.That(fSuperBlock.DriverInformationBlockAddress, Is.EqualTo(0x0D0E0F00), "Incorrect Driver Information Block Address");
                            break;

                        case 8:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x10203040AAAAAAAA), "Incorrect Base Address");
                            Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x4BBBBBBB05060708), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x50A0B0C0CCCCCCCC), "Incorrect End of File Address");
                            Assert.That(fSuperBlock.DriverInformationBlockAddress, Is.EqualTo(0x1DDDDDDD0D0E0F00), "Incorrect Driver Information Block Address");
                            break;
                    }
                }
            }
        }

        [Test, Combinatorial]
        public void Test_Basic_V2_and_V3_Parse(
            [Values(2, 3)] int aVerson,
            [Values(2, 4, 8)] int aOffset,
            [Values(2, 4, 8)] int aLength,
            [Values(
                ndf5.Metadata.FileConsistencyFlags.SwmrAccessEngaged,
                ndf5.Metadata.FileConsistencyFlags.WriteAccessOpen,
                ndf5.Metadata.FileConsistencyFlags.SwmrAccessEngaged | ndf5.Metadata.FileConsistencyFlags.WriteAccessOpen)]
            ndf5.Metadata.FileConsistencyFlags aFlags)
        {
            using (MemoryStream fBuffer = new MemoryStream())
            {
                //Arrange
                Mock<ndf5.Streams.IStreamProvider>
                         fStreamProvider = new Mock<Streams.IStreamProvider>(MockBehavior.Loose);
                fStreamProvider.Setup(a => a.GetStream(It.IsAny<ndf5.Streams.StreamRequestArguments>())).Returns(fBuffer);


                using (BinaryWriter fwriter = new BinaryWriter(fBuffer))
                {
                    ndf5.Metadata.FormatSignatureAndVersionInfo
                        fSig = new ndf5.Metadata.FormatSignatureAndVersionInfo((byte)aVerson, 0);
                    byte[]
                        fFormatBlock = fSig.AsBytes;
                    fwriter.Write(fFormatBlock);
                    fwriter.Write((byte)aOffset);
                    fwriter.Write((byte)aLength);
                    fwriter.Write((byte)aFlags);

                    switch (aOffset)
                    {
                        case 2:
                            fwriter.Write((ushort)0x1234);
                            fwriter.Write((ushort)0x5678);
                            fwriter.Write((ushort)0x9ABC);
                            fwriter.Write((ushort)0xDEF0);
                            break;

                        case 4:
                            fwriter.Write((uint)0x10203040);
                            fwriter.Write((uint)0x05060708);
                            fwriter.Write((uint)0x90A0B0C0);
                            fwriter.Write((uint)0x0D0E0F00);
                            break;

                        case 8:
                            fwriter.Write((ulong)0x10203040AAAAAAAA);
                            fwriter.Write((ulong)0x4BBBBBBB05060708);
                            fwriter.Write((ulong)0x50A0B0C0CCCCCCCC);
                            fwriter.Write((ulong)0x1DDDDDDD0D0E0F00);
                            break;
                    }

                    fwriter.Flush();


                    //Act
                    ndf5.Metadata.SuperBlockProvider
                        fSuperblockPorvider = new ndf5.Metadata.SuperBlockProvider(
                            fStreamProvider.Object,
                            fSig);

                    ndf5.Metadata.ISuperBlock
                        fSuperBlock = fSuperblockPorvider.SuperBlock;

                    //Assert
                    Assert.That(fSuperBlock.SuperBlockVersion, Is.EqualTo(aVerson), "Incorrect Superblock Version");
                    Assert.That(fSuperBlock.SizeOfOffsets, Is.EqualTo(aOffset), "Incorrect Size of Offsets");
                    Assert.That(fSuperBlock.SizeOfLengths, Is.EqualTo(aLength), "Incorrect Size of Lengths");

                    if (aVerson == 3)
                        Assert.That(fSuperBlock.FileConsistencyFlags, Is.EqualTo(aFlags), "Incorrect File Consistency Flags");

                    switch (aOffset)
                    {
                        case 2:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x1234), "Incorrect Base Address");
                            //Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x5678), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x9ABC), "Incorrect End of File Address");
                            //Assert.That(fSuperBlock.DriverInformationBlockAddress, Is.EqualTo(0xDEF0), "Incorrect Driver Information Block Address");
                            break;

                        case 4:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x10203040), "Incorrect Base Address");
                            //Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x05060708), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x90A0B0C0), "Incorrect End of File Address");
                            //Assert.That(fSuperBlock.DriverInformationBlockAddress, Is.EqualTo(0x0D0E0F00), "Incorrect Driver Information Block Address");
                            break;

                        case 8:
                            Assert.That(fSuperBlock.BaseAddress, Is.EqualTo(0x10203040AAAAAAAA), "Incorrect Base Address");
                            // TODO: Test Superblock extension
                            //Assert.That(fSuperBlock.FileFreespaceInfoAddress, Is.EqualTo(0x4BBBBBBB05060708), "Incorrect Address of File Free space Info");
                            Assert.That(fSuperBlock.EndOfFileAddress, Is.EqualTo(0x50A0B0C0CCCCCCCC), "Incorrect End of File Address");
                            Assert.That(fSuperBlock.RootGroupAddress, Is.EqualTo(0x1DDDDDDD0D0E0F00), "Incorrect Super block address");
                            break;
                    }
                }
            }
        }
    }
}

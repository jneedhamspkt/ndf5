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
    public class DataSpace
    {
        private static readonly uObjects.Dimension[]
            TestDimensions = new uObjects.Dimension[]
        {
            new uObjects.Dimension(1, 2),
            new uObjects.Dimension(3, 4),
            new uObjects.Dimension(5),
            new uObjects.Dimension(6),
            new uObjects.Dimension(7, 8)
        };

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_V1_Unlimited_Parsing(
            [Values(2, 4, 8)] int aLengthBytes,
            [Range(1, 5)] int aDims)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<ulong?> fWriteLength = null;
                switch (aLengthBytes)
                {
                    case 2:
                        fWriteLength = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteLength = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteLength = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)1);     //Version 1
                fWriter.Write((byte)aDims); //Dimensionality
                fWriter.Write((byte)0x0);   //Flags
                fWriter.Write((byte)0x0);   //Reserved
                fWriter.Write((int)0x0);    //Reserved

                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong)TestDimensions[i].Size);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act
                long fRead;
                uMessages.Message fTest = ndf5.Messages.Message.Read(
                    fReader,
                    uMessages.MessageType.Dataspace,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.Dataspace
                    fResult = fTest as uMessages.Dataspace;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(fResult.Dimensions,
                    Is.EquivalentTo(TestDimensions.Take(aDims).Select(a => new uObjects.Dimension(a.Size))),
                    "Incorrect Dimensions");
                Assert.That(fResult.Dimensions.Select(a => a.MaxSize),
                    Is.All.Null,
                    "Incorret Max Size");
                Assert.That(fRead,
                    Is.EqualTo(fTestSource.Position),
                    "Incorrect Read Bytes");
                Assert.That(fResult.DataSpaceType,
                    Is.EqualTo(uObjects.DataSpaceType.Simple),
                    "Incorrect object type");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_V1_Limited_Parsing(
            [Values(2, 4, 8)] int aLengthBytes,
            [Range(1, 5)] int aDims)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<ulong?> fWriteLength = null;
                switch (aLengthBytes)
                {
                    case 2:
                        fWriteLength = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteLength = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteLength = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)1);     //Version 1
                fWriter.Write((byte)aDims); //Dimensionality
                fWriter.Write((byte)0x1);   //Flags, Has Max
                fWriter.Write((byte)0x0);   //Reserved
                fWriter.Write((int)0x0);    //Reserved

                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong)TestDimensions[i].Size);
                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong?)TestDimensions[i].MaxSize);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act
                long fRead;
                uMessages.Message fTest = ndf5.Messages.Message.Read(
                    fReader,
                    uMessages.MessageType.Dataspace,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.Dataspace
                    fResult = fTest as uMessages.Dataspace;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(fResult.Dimensions,
                    Is.EquivalentTo(TestDimensions.Take(aDims)),
                    "Incorrect Dimensions");
                Assert.That(fRead,
                    Is.EqualTo(fTestSource.Position),
                    "Incorrect Read Bytes");
                Assert.That(fResult.DataSpaceType,
                    Is.EqualTo(uObjects.DataSpaceType.Simple),
                    "Incorrect object type");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_TooShortError(
            [Values(2, 4, 8)] int aLengthBytes,
            [Range(2, 5)] int aTooShortLength,
            [Range(1, 2)] int aVersion)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                int
                    fDims = TestDimensions.Length;
                Action<Length> fWriteLength = null;
                switch (aLengthBytes)
                {
                    case 2:
                        fWriteLength = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteLength = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteLength = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)aVersion);      //Version
                fWriter.Write((byte)fDims);         //Dimensionality
                fWriter.Write((byte)0x1);           //Flags, Has Max
                fWriter.Write((byte)0x0);           //Reserved
                fWriter.Write((int)0x0);            //Reserved

                for (int i = 0; i < fDims; ++i)
                    fWriteLength((ulong)TestDimensions[i].Size);
                for (int i = 0; i < fDims; ++i)
                    fWriteLength((ulong?)TestDimensions[i].MaxSize);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act/Assert
                long fRead;
                Assert.That(() =>
                {
                    uMessages.Message fTest = ndf5.Messages.Message.Read(
                        fReader,
                        uMessages.MessageType.Dataspace,
                        uMessages.MessageAttributeFlag.None,
                        aTooShortLength,
                        out fRead);
                },
                Throws.ArgumentException,
                "Stream should be too short to read");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_V2_Unlimited_Simple_Parsing(
            [Values(2, 4, 8)] int aLengthBytes,
            [Range(1, 5)] int aDims)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<ulong?> fWriteLength = null;
                switch (aLengthBytes)
                {
                    case 2:
                        fWriteLength = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteLength = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteLength = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)2);     //Version 2
                fWriter.Write((byte)aDims); //Dimensionality
                fWriter.Write((byte)0x0);   //Flags
                fWriter.Write((byte)ndf5.Objects.DataSpaceType.Simple);   //Reserved

                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong)TestDimensions[i].Size);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act
                long fRead;
                uMessages.Message fTest = ndf5.Messages.Message.Read(
                    fReader,
                    uMessages.MessageType.Dataspace,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.Dataspace
                    fResult = fTest as uMessages.Dataspace;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(fResult.Dimensions,
                    Is.EquivalentTo(TestDimensions.Take(aDims).Select(a => new uObjects.Dimension(a.Size))),
                    "Incorrect Dimensions");
                Assert.That(fResult.Dimensions.Select(a => a.MaxSize),
                    Is.All.Null,
                    "Incorret Max Size");
                Assert.That(fRead,
                    Is.EqualTo(fTestSource.Position),
                    "Incorrect Read Bytes");
                Assert.That(fResult.DataSpaceType,
                    Is.EqualTo(uObjects.DataSpaceType.Simple),
                    "Incorrect object type");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_V2_Limited_Simple_Parsing(
            [Values(2, 4, 8)] int aLengthBytes,
            [Range(1, 5)] int aDims)
        {
            //Arrange
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                Action<Length> fWriteLength = null;
                switch (aLengthBytes)
                {
                    case 2:
                        fWriteLength = a => fWriter.Write((ushort)(a ?? ushort.MaxValue));
                        break;
                    case 4:
                        fWriteLength = a => fWriter.Write((uint)(a ?? uint.MaxValue));
                        break;
                    case 8:
                        fWriteLength = a => fWriter.Write((ulong)(a ?? ulong.MaxValue));
                        break;
                }
                fWriter.Write((byte)2);     //Version 2
                fWriter.Write((byte)aDims); //Dimensionality
                fWriter.Write((byte)0x1);   //Flags, Has Max
                fWriter.Write((byte)uObjects.DataSpaceType.Simple);

                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong)TestDimensions[i].Size);
                for (int i = 0; i < aDims; ++i)
                    fWriteLength((ulong?)TestDimensions[i].MaxSize);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)aLengthBytes);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act
                long fRead;
                uMessages.Message fTest = ndf5.Messages.Message.Read(
                    fReader,
                    uMessages.MessageType.Dataspace,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.Dataspace
                    fResult = fTest as uMessages.Dataspace;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(fResult.Dimensions,
                    Is.EquivalentTo(TestDimensions.Take(aDims)),
                    "Incorrect Dimensions");
                Assert.That(fRead,
                    Is.EqualTo(fTestSource.Position),
                    "Incorrect Read Bytes");
                Assert.That(fResult.DataSpaceType,
                    Is.EqualTo(uObjects.DataSpaceType.Simple),
                    "Incorrect object type");
            }
        }

        [Test, TestOf(typeof(uMessages.Dataspace))]
        public void Test_Data_Space_V2_Special_Parsing(
            [Values(uObjects.DataSpaceType.Null,
                    uObjects.DataSpaceType.Scaler)]
            uObjects.DataSpaceType aType)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)2);     //Version 2
                fWriter.Write((byte)0); //Dimensionality
                fWriter.Write((byte)0x1);   //Flags, Has Max
                fWriter.Write((byte)aType);

                fTestSource.Seek(0, SeekOrigin.Begin);

                Mock<ndf5.Metadata.ISuperBlock>
                    fSuperBlock = new Mock<ndf5.Metadata.ISuperBlock>(MockBehavior.Loose);
                fSuperBlock.SetupGet(a => a.SizeOfOffsets).Returns((byte)8);
                fSuperBlock.SetupGet(a => a.SizeOfLengths).Returns((byte)8);

                Hdf5Reader
                    fReader = new Hdf5Reader(fTestSource, fSuperBlock.Object);

                //Act
                long fRead;
                uMessages.Message fTest = ndf5.Messages.Message.Read(
                    fReader,
                    uMessages.MessageType.Dataspace,
                    uMessages.MessageAttributeFlag.None,
                    null,
                    out fRead);

                uMessages.Dataspace
                    fResult = fTest as uMessages.Dataspace;

                //Assert
                Assert.That(fResult,
                    Is.Not.Null,
                    "Incorrect Message Type returned");
                Assert.That(fResult.Dimensions.Count,
                    Is.EqualTo(0),
                    "Incorrect Dimensions");
                Assert.That(fRead,
                    Is.EqualTo(fTestSource.Position),
                    "Incorrect Read Bytes");
                Assert.That(fResult.DataSpaceType,
                    Is.EqualTo(aType),
                    "Incorrect object type");
            }
        }

    }
}

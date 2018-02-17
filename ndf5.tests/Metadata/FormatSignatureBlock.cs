using System;
using System.IO;
using NUnit.Framework;
namespace ndf5.tests.Metadata
{
    [TestFixture]
    public class FormatSignatureBlock
    {
        [Test, Sequential]
        public void Test_Round_Trip(
            [Random(0, 255, 2)]int aSuperBlockVersion,
            [Random(0, 255, 2)]int aFreeSpaceStorageVersion,
            [Random(0, 255, 2)]int aRootGroupSymbolTableVersion)
        {
            //Arrange
            ndf5.Metadata.FormatSignatureAndVersionInfo
                fInfo = new ndf5.Metadata.FormatSignatureAndVersionInfo(
                    (byte)aSuperBlockVersion,
                    (byte)aFreeSpaceStorageVersion,
                    (byte)aRootGroupSymbolTableVersion);

            byte[]
                fBytes = fInfo.AsBytes;
            MemoryStream
                fTestStream = new MemoryStream(fBytes);

            //Act

            ndf5.Metadata.FormatSignatureAndVersionInfo
                fTest;

            bool
                fRead = ndf5.Metadata.FormatSignatureAndVersionInfo.TryRead(
                    fTestStream, out fTest);

            //Asset
            Assert.That(fRead, Is.True, "TryRead Failed");
            Assert.That(fTest, Is.Not.Null, "Null Ouput");
            Assert.That(
                fTest.SuperBlockVersion, 
                Is.EqualTo(aSuperBlockVersion), 
                "Incorrect SuperBlockVersion"); 
            Assert.That(
                fTest.FreeSpaceStorageVersion, 
                Is.EqualTo(aFreeSpaceStorageVersion), 
                "Incorrect FreeSpaceStorageVersion"); 
            Assert.That(
                fTest.RootGroupSymbolTableVersion, 
                Is.EqualTo(aRootGroupSymbolTableVersion), 
                "Incorrect RootGroupSymbolTableVersion"); 

        }

        [Test, Sequential]
        public void Test_Bad_Data(
            [Range(0,7)][Values(11)]int aCorruptByte)
        {
            byte[]
                fData = new byte[]{ 137,72,68,70,13,10,26,10,0,0,0,0};

            fData[aCorruptByte]++;

            //Arrange
            Stream
                fTestData = new MemoryStream(fData); 

            //Act

            ndf5.Metadata.FormatSignatureAndVersionInfo
                fTest;

            bool
                fRead = ndf5.Metadata.FormatSignatureAndVersionInfo.TryRead(
                    fTestData, out fTest);

            //Asset
            Assert.That(fRead, Is.False, "TryRead Should have Failed");
            Assert.That(fTest, Is.Null, "Non Null Ouput");

        }
    }
}

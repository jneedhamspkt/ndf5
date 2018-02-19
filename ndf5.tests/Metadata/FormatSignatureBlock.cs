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
            [Random(0, 255, 2)]int aSuperBlockVersion)
        {
            //Arrange
            ndf5.Metadata.FormatSignatureAndVersionInfo
                fInfo = new ndf5.Metadata.FormatSignatureAndVersionInfo(
                    (byte)aSuperBlockVersion);

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

        }

        [Test, Sequential]
        public void Test_Bad_Data(
            [Range(0,7)]int aCorruptByte)
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

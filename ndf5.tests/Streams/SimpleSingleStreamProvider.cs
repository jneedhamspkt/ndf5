using System;
using System.IO;
using NUnit.Framework;


using TestStreamProvider = ndf5.Streams.SimpleSingleStreamProvider;
using TestArgs = ndf5.Streams.StreamRequestArguments;


namespace ndf5.tests.Streams
{
    [TestFixture]
    public class SimpleSingleStreamProvider
    {
        [Test]
        public static void Test_Wrapping_Contraint()
        {
            using (MemoryStream fBuffer = new MemoryStream())
            {
                const string
                    fcTestData = "Hello world";
                using (StreamWriter fWiter = new StreamWriter(fBuffer))
                {
                    fWiter.WriteLine(fcTestData);
                    fWiter.Flush();
                    fBuffer.Seek(0, SeekOrigin.Begin);

                    using (TestStreamProvider fTest = new TestStreamProvider(fBuffer))
                    {
                        Stream
                            fStream = fTest.GetStream(new TestArgs()),
                            fStream2 = fTest.GetStream(new TestArgs(
                                aAccessTimeout: TimeSpan.FromSeconds(0)));

                        Assert.That(fBuffer, Is.Not.EqualTo(fStream), "Returned stream shold be wrapped");
                        Assert.That(fStream2, Is.Not.EqualTo(fStream), "Returned wrapper should always be new");

                        using(StreamReader fReader = new StreamReader(fStream))
                        {
                            Assert.That(fReader.ReadLine(), Is.EqualTo(fcTestData), "Bad Data");

                            Assert.That(fBuffer.Position, Is.EqualTo(fBuffer.Position), "Position not passed");
                            fBuffer.Seek(0,SeekOrigin.Begin);

                            Assert.That(fBuffer.Position, Is.EqualTo(0), "Seek Not passed");
                        }

                        using (StreamReader fReader2 = new StreamReader(fStream2))
                        {
                            Assert.That(fReader2.ReadLine(), Is.EqualTo(fcTestData), "Bad Data");

                            Assert.That(fBuffer.Position, Is.EqualTo(fBuffer.Position), "Position not passed");
                            fBuffer.Seek(0, SeekOrigin.Begin);

                            Assert.That(fBuffer.Position, Is.EqualTo(0), "Seek Not passed");
                        }

                    }

                }

            }
        }
    }
}

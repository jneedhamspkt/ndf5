using System;
using System.IO;
using ndf5.Streams;
using NUnit.Framework;

using uMessages = ndf5.Messages;

namespace ndf5.tests.Messages
{
    [TestFixture]
	public class DataType
	{
        [Test, TestOf(typeof(uMessages.Datatype))]
        public void Short_Stream_Check(
            [Values] uMessages.DatatypeClass aDatatypeClass,
            [Values] uMessages.DatatypeVersion aVersion)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)(((byte)aVersion << 4) | ((byte)aDatatypeClass)));
                fWriter.Write(new byte[]{
                    0x00, 0x00,
                });
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
                    Assert.That(() =>
                    {
                        ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            12,
                            out fReadBytes);
                    }, Throws.Exception.TypeOf(typeof(System.IO.EndOfStreamException)),
                    "Length not checked");


                    //Add three more bytes to get past the header length
                    fWriter.Write(new byte[]{
                        0x10, 0x00, 0x00,
                    });

                    fTestSource.Seek(0, SeekOrigin.Begin);

                    Assert.That(() =>
                    {
                        ndf5.Messages.Message.Read(
                            fReader,
                            uMessages.MessageType.Datatype,
                            uMessages.MessageAttributeFlag.None,
                            12,
                            out fReadBytes);
                    }, Throws.Exception.TypeOf(typeof(System.IO.EndOfStreamException)),
                    "Length not checked");
                }
            }
        }
	}
}

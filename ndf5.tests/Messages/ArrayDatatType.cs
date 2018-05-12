using System;
using System.IO;

using ndf5.Streams;
using ndf5.Messages;

using NUnit.Framework;

using uTest = ndf5.Messages.ArrayDataType;
using System.Collections.Generic;
using System.Linq;

namespace ndf5.tests.Messages
{
    [TestFixture]
    public static class ArrayDataType
    {
        [Test]
        public static void Test_V1_Parse_Fail(
            [Values(0, 1, 12, 37, 42)]int aLength)
        {
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
                fWriter.Write((byte)(((byte)DatatypeVersion.Version1 << 4) | 0x0A));
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);

                fWriter.Write((byte)aLength);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);

                System.Random
                      fRandom = new Random();

                for (int fIndex = 0; fIndex < aLength; ++fIndex)
                {
                    int fRnd = fRandom.Next();
                    fWriter.Write(fRnd);
                }

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

                        uTest
                            fResult = ndf5.Messages.Message.Read(
                                fReader,
                                MessageType.Datatype,
                                MessageAttributeFlag.None,
                                null,
                                out fReadBytes) as uTest;
                    }, 
                    Throws.Exception,
                    "Version 1 should not parse");
                }
            }
        }

        [Test]
        public static void Test_V2_Parse(
            [Random(2, Distinct =true)] int aSize,
            [Values( 1, 2, 12, 37, 42)]int aDimCount)
        {
			uint
		        fSize = (uint)aSize;
            using (Stream fTestSource = new MemoryStream())
            using (BinaryWriter fWriter = new BinaryWriter(fTestSource))
            {
				fWriter.Write((byte)(((byte)DatatypeVersion.Version2 << 4) | 0x0A));
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);

                fWriter.Write(fSize);

				fWriter.Write((byte)aDimCount);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);
                fWriter.Write((byte)0);

                System.Random
                    fRandom = new Random();
                List<uint>
                    fDimensions = new List<uint>();
                for (int fIndex = 0; fIndex < aDimCount; ++fIndex)
                {
                    uint 
                        fRnd = (uint)fRandom.Next();
                    fWriter.Write(fRnd);
                    fDimensions.Add(fRnd);
                }
				for (int fIndex = 0; fIndex < aDimCount; ++fIndex)
					fWriter.Write((uint)fIndex);
                //BaseType is a 16 bit fixed point
                fWriter.Write(new byte[]{
                    0x10, 0x08, 0x00, 0x00,
                    0x02, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x10, 0x00,
                });

				ndf5.Messages.Datatype
                    fBaseType = new ndf5.Messages.FixedPointDataType(
                        2,
                        Objects.ByteOrdering.LittleEndian,
                        0,
                        0,
                        true,
                        0,
                        16),
				fNonEqualBaseType = new ndf5.Messages.FixedPointDataType(
                        4,
                        Objects.ByteOrdering.LittleEndian,
                        0,
                        0,
                        true,
                        0,
                        32);

                    fTestSource.Seek(0L, SeekOrigin.Begin);

                Moq.Mock<ndf5.Metadata.ISuperBlock>
                   fSuperblock = new Moq.Mock<ndf5.Metadata.ISuperBlock>(
                       Moq.MockBehavior.Loose);


                using (Hdf5Reader fReader = new Hdf5Reader(
                    fTestSource,
                    fSuperblock.Object))
                {
                    long
                        fReadBytes;

                    uTest
                        fExpected = new uTest(
                            fDimensions.ToArray(), 
                            fBaseType,
						    fSize),
                        fResult = ndf5.Messages.Message.Read(
                            fReader,
                            MessageType.Datatype,
                            MessageAttributeFlag.None,
                            null,
                            out fReadBytes) as uTest;
					
					Assert.That(
                        fResult,
                        Is.EqualTo(fExpected),
                        "Equality check failed");

					Assert.That(
						fResult.BaseType,
						Is.EqualTo(fBaseType),
                        "Incorrect Base Type");

					Assert.That(
						fResult.DimensionSizes.ToArray(),
						Is.EquivalentTo(fDimensions.ToArray()),
                        "Incorrect Dimensions");
					
					Assert.That(
						fResult.Size,
						Is.EqualTo(fSize),
                        "Incorrect Size");

					Assert.That(
                        fResult,
						Is.Not.EqualTo(
							new uTest(
								fDimensions.Concat(new uint[]{1}).ToArray(),
                                fBaseType,
								fSize)),
						"Inequality check failed (Dimensions)");

					Assert.That(
                        fResult,
                        Is.Not.EqualTo(
                            new uTest(
								fDimensions.ToArray(),
								fNonEqualBaseType,
                                fSize)),
						"Inequality check failed (Base Type)");

					Assert.That(
                        fResult,
                        Is.Not.EqualTo(
                            new uTest(
                                fDimensions.ToArray(),
                                fBaseType,
                                fSize+1)),
						"Inequality check failed (Size)");

                }
            }
        }
    }
}

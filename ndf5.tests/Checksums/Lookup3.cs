using System;
using System.Text;
using System.Linq;
using NUnit.Framework;

using Test = ndf5.Checksums.Lookup3;

namespace ndf5.tests.Checksums
{
    [TestFixture]
    public class Lookup3
    {
        [Test]
        [TestOf(typeof(Test))]
        public static void ZeroTest()
        {
            Assert.That(Test.ComputeHash(new byte[0]),
                        Is.EqualTo(0xDEADBEEF),
                        "Incorrect Zero Byte Hash");
        }

        [Test]
        [TestOf(typeof(Test))]
        public static void Confirm_driver3_from_original()
        {
            byte[]
                fTestBytes = Encoding.ASCII.GetBytes(
                "This is the time for all good men to come to the aid of their country...");

            uint[]
                fExpected = //obtained by running Lookup3.c
            {
                0x499ae8fa,
                0xb9bef31c,
                0x8efefdfd,
                0xa56b7aab,
                0xb1946734,
                0x9f31c5ce,
                0x0826585d,
                0x55b69dea,
                0xf4688dd0,
                0xe87eb146,
                0xb202fb17,
                0x711fe56a
            };
            for (int i = 0; i < 12; ++i)
            {
                Assert.That(
                    Test.ComputeHash(fTestBytes.Take(fTestBytes.Length - i).ToArray(), 13),
                    Is.EqualTo(fExpected[i]),
                    "Incorrect Hash");
            }
        }

        [Test]
        [TestOf(typeof(Test))]
        public static void Test_continuation_of_hash()
        {
            byte[]
                fTestBytes = Encoding.ASCII.GetBytes(
                    "This is the time for all good men to come to the aid of their country..."),
            fTestBytes2 = fTestBytes.Reverse().ToArray();

            uint
                fSingle = Test.ComputeHash(fTestBytes.Concat(fTestBytes2).ToArray());

            Test
                fTwoPartHash = new Test();
            fTwoPartHash.ComputeHash(fTestBytes, 0, fTestBytes.Length);
            fTwoPartHash.ComputeHash(fTestBytes2, 0, fTestBytes2.Length);

            uint
                fTwoPart;
            using (System.IO.MemoryStream fStream = new System.IO.MemoryStream(fTwoPartHash.Hash))
                fTwoPart = new System.IO.BinaryReader(fStream).ReadUInt32();
            Assert.That(fTwoPart, Is.EqualTo(fSingle), "Init value should allow continuation");
        }
    }
}

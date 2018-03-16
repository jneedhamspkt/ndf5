using System;

using NUnit.Framework;

using Length = ndf5.Length;
using Offset = ndf5.Offset;

namespace ndf5.tests
{
    /// <summary>
    /// Verify wrappng of ulong in Lengths and Offsets
    /// </summary>
    [TestFixture]
    public class UnsignedNumber
    {
        [Test, TestOf(typeof(Length))]
        public void Test_Lengths()
        {
            Assert.That((ulong)(Length)2, Is.EqualTo(2), "Casting Path broken");
            Assert.That((Length)2, Is.EqualTo((Length)2), "Casting Equality broken");

            Assert.That(new Length(2), Is.EqualTo(new Length(2)), "Casting Equality broken");
            Assert.That(new Length(2), Is.EqualTo((Length)2), "Casting Equality broken");
            Assert.That((Length)2, Is.EqualTo(new Length(2)), "Casting Equality broken");

            Assert.That(new Length(2), Is.Not.EqualTo(new Length(5)), "Casting Equality broken");
            Assert.That(new Length(2), Is.Not.EqualTo((Length)5), "Casting Equality broken");
            Assert.That((Length)2, Is.Not.EqualTo(new Length(5)), "Casting Equality broken");

            Assert.That(new Length(2) > new Length(0), Is.True, "Broken inequality check");
            Assert.That(new Length(0) > new Length(2), Is.False, "Broken inequality check");
            Assert.That(new Length(2), Is.GreaterThan( new Length(0) ), "Broken inequality check");
            Assert.That(new Length(0), Is.Not.GreaterThan( new Length(2) ), "Broken inequality check");
            Assert.That(new Length(0) < new Length(2), Is.True, "Broken inequality check");
            Assert.That(new Length(2) < new Length(0), Is.False, "Broken inequality check");
            Assert.That(new Length(0), Is.LessThan(new Length(2)), "Broken inequality check");
            Assert.That(new Length(2), Is.Not.LessThan(new Length(0)), "Broken inequality check");

            Assert.That(new Length(2) >= new Length(0), Is.True, "Broken inequality check");
            Assert.That(new Length(0) >= new Length(2), Is.False, "Broken inequality check");
            Assert.That(new Length(2), Is.GreaterThanOrEqualTo(new Length(0)), "Broken inequality check");
            Assert.That(new Length(0), Is.Not.GreaterThanOrEqualTo(new Length(2)), "Broken inequality check");
            Assert.That(new Length(0) <= new Length(2), Is.True, "Broken inequality check");
            Assert.That(new Length(2) <= new Length(0), Is.False, "Broken inequality check");
            Assert.That(new Length(0), Is.LessThanOrEqualTo(new Length(2)), "Broken inequality check");
            Assert.That(new Length(2), Is.Not.LessThanOrEqualTo(new Length(0)), "Broken inequality check");

            Assert.That((Length)2 + (Length)2, Is.EqualTo(new Length(4)), "Addtion broken");
            Assert.That((Length)2 - (Length)2, Is.EqualTo(new Length(0)), "Subtraction broken");
        }

        [Test, TestOf(typeof(Offset))]
        public void Test_Offsets()
        {
            Assert.That((ulong)(Offset)2, Is.EqualTo(2), "Casting Path broken");
            Assert.That((Offset)2, Is.EqualTo((Offset)2), "Casting Equality broken");

            Assert.That(new Offset(2), Is.EqualTo(new Offset(2)), "Casting Equality broken");
            Assert.That(new Offset(2), Is.EqualTo((Offset)2), "Casting Equality broken");
            Assert.That((Offset)2, Is.EqualTo(new Offset(2)), "Casting Equality broken");

            Assert.That(new Length(2), Is.Not.EqualTo(new Offset(5)), "Casting Equality broken");
            Assert.That(new Offset(2), Is.Not.EqualTo((Offset)5), "Casting Equality broken");
            Assert.That((Offset)2, Is.Not.EqualTo(new Offset(5)), "Casting Equality broken");

            Assert.That(new Offset(2) > new Offset(0), Is.True, "Broken inequality check");
            Assert.That(new Offset(0) > new Offset(2), Is.False, "Broken inequality check");
            Assert.That(new Offset(2), Is.GreaterThan(new Offset(0)), "Broken inequality check");
            Assert.That(new Offset(0), Is.Not.GreaterThan(new Offset(2)), "Broken inequality check");
            Assert.That(new Offset(0) < new Offset(2), Is.True, "Broken inequality check");
            Assert.That(new Offset(2) < new Offset(0), Is.False, "Broken inequality check");
            Assert.That(new Offset(0), Is.LessThan(new Offset(2)), "Broken inequality check");
            Assert.That(new Offset(2), Is.Not.LessThan(new Offset(0)), "Broken inequality check");

            Assert.That(new Offset(2) >= new Offset(0), Is.True, "Broken inequality check");
            Assert.That(new Offset(0) >= new Offset(2), Is.False, "Broken inequality check");
            Assert.That(new Offset(2), Is.GreaterThanOrEqualTo(new Offset(0)), "Broken inequality check");
            Assert.That(new Offset(0), Is.Not.GreaterThanOrEqualTo(new Offset(2)), "Broken inequality check");
            Assert.That(new Offset(0) <= new Offset(2), Is.True, "Broken inequality check");
            Assert.That(new Offset(2) <= new Offset(0), Is.False, "Broken inequality check");
            Assert.That(new Offset(0), Is.LessThanOrEqualTo(new Offset(2)), "Broken inequality check");
            Assert.That(new Offset(2), Is.Not.LessThanOrEqualTo(new Offset(0)), "Broken inequality check");

            Assert.That(new Offset(2) + (Offset)2, Is.EqualTo(new Offset(4)), "Addtion broken");
            Assert.That(new Offset(2) - (Offset)2, Is.EqualTo(new Offset(0)), "Subtraction broken");
        }
        
    }
}

using System;
using NUnit.Framework;
namespace ndf5.tests
{
    /// <summary>
    /// Verify object extensions
    /// </summary>
    [TestFixture, TestOf(typeof(ObjectExtensions))]
    public class ObjectTests
    {
        [Test]
        public void Test_Null_Checking()
        {
            object
                fNull = null,
                fNotNull = new object();
            Assert.That(fNull.IsNull, Is.True);
            Assert.That(fNull.IsNotNull, Is.False);
            Assert.That(fNotNull.IsNull, Is.False);
            Assert.That(fNotNull.IsNotNull, Is.True);
        }
    }
}

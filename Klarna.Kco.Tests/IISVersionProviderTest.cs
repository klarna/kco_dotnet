namespace Klarna.Checkout.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class IISVersionProviderTest
    {
        private IISVersionProvider target;

        [SetUp]
        public void Init()
        {
            this.target = new IISVersionProvider();
        }

        [Test]
        public void FindIISVersion_UnixPlatform_ReturnedDefaultIISVersion()
        {
            var actual = this.target.FindIISVersion(PlatformID.Unix);

            Assert.AreEqual(IISVersionProvider.DefaultIISVersion, actual);
        }
    }
}
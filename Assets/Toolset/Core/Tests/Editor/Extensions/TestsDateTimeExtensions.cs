using NUnit.Framework;
using System;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Test class for validating DateTime extensions.
    /// </summary>
    public class TestsDateTimeExtensions
    {
        private readonly DateTime epochTime = new DateTime(1970, 1, 1);

        [Test]
        public void TestEpochTime()
        {
            Assert.AreEqual(epochTime, DateTimeExtensions.EpochTime);
        }

        [Test]
        public void TestMillisecondsSinceUnixEpoch()
        {
            Assert.AreEqual(10000L, epochTime.AddSeconds(10).MillisecondsSinceUnixEpoch());

            Assert.AreEqual(-10000L, epochTime.AddSeconds(-10).MillisecondsSinceUnixEpoch());

            Assert.AreEqual(0L, epochTime.MillisecondsSinceUnixEpoch());
        }
    }
}

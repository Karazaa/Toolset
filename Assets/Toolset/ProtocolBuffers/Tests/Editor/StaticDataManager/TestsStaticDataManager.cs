using NUnit.Framework;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests for the Static Data Manager.
    /// </summary>
    public class TestsStaticDataManager
    {
        private const string c_guid = "a56c2391-052a-4947-b393-7f1ccd7ee3c4";
        private static ToolsetGuid s_toolsetGuid;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            StaticDataManager.Initialize();
            s_toolsetGuid = new ToolsetGuid() { Guid = c_guid};
        }
        
        [Test]
        public void TestLoadRecord()
        {
            Assert.IsTrue(StaticDataManager.TryGet(s_toolsetGuid, out ProtoWithGuid output));
            Assert.IsNotNull(output);
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            StaticDataManager.Uninitialize();
        }
    }
}

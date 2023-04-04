using NUnit.Framework;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests for the Static Data Manager.
    /// </summary>
    public class TestsStaticDataManager
    {
        private const string c_guid = "ee4eba5d-4d82-4f4c-ad3d-88c08c830658";
        private static ToolsetGuid s_toolsetGuid;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            StaticDataManager.Initialize();
            s_toolsetGuid = ToolsetGuid.ConstructForType<ProtoWithGuid>(c_guid);
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

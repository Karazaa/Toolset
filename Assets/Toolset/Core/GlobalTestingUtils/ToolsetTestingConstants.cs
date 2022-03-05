namespace Toolset.Global.Utils
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset tests.
    /// </summary>
    public class ToolsetTestingConstants
    {
        public const int c_mediumTimeoutMilliseconds = 10000;
        public const int c_longTimeoutMilliseconds = 30000;
        public const string c_exampleSceneNameRoutines = "ExampleSceneRoutines";
        public const string c_searchTargetNameRoutines = "SearchTarget";
        public static readonly string s_pathToTestingProtoModelSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/ProtoFiles";
        public static readonly string s_pathToTestingProtoModelGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/Generated";
    }
}
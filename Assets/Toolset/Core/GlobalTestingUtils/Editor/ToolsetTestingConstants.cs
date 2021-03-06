namespace Toolset.Global.Utils
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset tests.
    /// </summary>
    public static class ToolsetTestingConstants
    {
        public const int c_mediumTimeoutMilliseconds = 10000;
        public const int c_longTimeoutMilliseconds = 30000;
        public const string c_searchTargetNameRoutines = "SearchTarget";
        public const string c_remoteTestingUrl = "https://localhost:44345/";
        public static readonly string s_pathToTestingProtoModelSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Editor/PersistentTesting/ProtoFiles";
        public static readonly string s_pathToTestingProtoModelGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Editor/PersistentTesting/Generated";
    }
}
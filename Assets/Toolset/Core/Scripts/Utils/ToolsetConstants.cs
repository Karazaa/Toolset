namespace Toolset.Core
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset.
    /// </summary>
    public static class ToolsetConstants
    {
        public static readonly string s_pathToProtoModelSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Scripts/SaveManager/ProtoFiles";
        public static readonly string s_pathToProtoModelGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Scripts/SaveManager/Generated";
        public static readonly string s_pathToJsonInstanceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Scripts/SaveManager/JSON";
    }
}

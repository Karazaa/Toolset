#if UNITY_EDITOR
namespace Toolset.Core
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset that are accessible only in UnityEditor.
    /// </summary>
    public static class ToolsetEditorConstants
    {
        public static readonly string s_pathToProtoModelGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Scripts/SaveManager/Generated";
        public static readonly string s_pathToProtoDataDirectory = System.IO.Directory.GetCurrentDirectory() + "/Data/Proto";
        public static readonly string s_pathToJsonDataDirectory = System.IO.Directory.GetCurrentDirectory() + "/Data/JSON";
        public static readonly string s_protoFileSearchPattern = "*.proto";
    }
}
#endif

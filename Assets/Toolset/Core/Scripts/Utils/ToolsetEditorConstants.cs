using System.IO;

#if UNITY_EDITOR
namespace Toolset.Core
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset that are accessible only in UnityEditor.
    /// </summary>
    public static class ToolsetEditorConstants
    {
        public const int c_editorSpaceIndentLevel = 15;
        public const string c_protoEditorOptOutToken = "//PROTO_EDITOR_OPT_OUT";
        public static readonly string s_pathToProtoModelGeneratedDirectory = Path.Combine(UnityEngine.Application.dataPath, "Toolset", "ProtocolBuffers", "Scripts", "SaveManager", "Generated");
        public static readonly string s_pathToProtoDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Proto");
        public static readonly string s_pathToJsonDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "JSON");
    }
}
#endif

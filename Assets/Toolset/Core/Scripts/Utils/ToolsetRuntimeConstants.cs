namespace Toolset.Core
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset that are accessible at runtime.
    /// </summary>
    public static class ToolsetRuntimeConstants
    {
        public const string c_tsoFileFormat = "{0}/{1}.tso";
        public const string c_tsoSearchPattern = "*.tso";
        public const string c_protoFileFormat = "{0}/{1}.proto";
        public const string c_protoSearchPattern = "*.proto";
        public const string c_jsonSearchPattern = "*.json";
        public const string c_jsonFileExtension = ".json";
        
        public static readonly string s_pathToStreamingAssetsDirectory = UnityEngine.Application.dataPath + "/StreamingAssets";
        public static readonly string s_pathToJsonStreamingAssetsDirectory = s_pathToStreamingAssetsDirectory + "/JSON";
        public static readonly string s_staticDataManifestFileName = "StaticDataManifest.manifest";
    }
}

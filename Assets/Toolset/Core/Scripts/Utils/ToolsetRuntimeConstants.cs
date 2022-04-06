namespace Toolset.Core
{
    /// <summary>
    /// Static class to save some global constants associated with Toolset that are accessible at runtime.
    /// </summary>
    public static class ToolsetRuntimeConstants
    {
        public static readonly string s_pathToJsonStreamingAssetsDirectory = UnityEngine.Application.dataPath + "/StreamingAssets/JSON";
    }
}

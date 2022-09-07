using System.Collections.Generic;
using Toolset.Core;

namespace Toolset.ProtocolBuffers
{
    public class StaticDataManager : ToolsetMonoBehaviorSingleton<StaticDataManager>
    {
        ///// ----------------------
        // private static readonly Dictionary<string, TYPENAME> s_TYPENAMEData = new Dictionary<string, TYPENAME>();
        ///// ----------------------

        protected override void Awake()
        {
            ///// ----------------------
            // List<TYPENAME> TYPENAMEDataList = SaveManager.DeserializeJsonModelsOfType<TYPENAME>(ToolsetRuntimeConstants.s_pathToJsonStreamingAssetsDirectory);
            // foreach (var item in TYPENAMEDataList)
            // {
            //     s_TYPENAMEData.Add(item.Guid, item);
            // }
            ///// ----------------------
        }
        
        ///// ----------------------
        // public static bool TryGetTYPENAME(string guid, out TYPENAME output)
        // {
        //     bool result = s_TYPENAMEData.TryGetValue(guid, out TYPENAME internalResult);
        //     output = internalResult;
        //     return result;
        // }
        ///// ----------------------
    }
}

using System;

namespace Toolset.Core
{
    public static class ToolsetGlobalUtils
    {
        /// <summary>
        /// TODO FILL ME OUT
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeAcrossAllAssemblies(string type)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type returnedType = assembly.GetType(type);
                if (returnedType != null)
                    return returnedType;
            }
            return null;
        }
    }
}

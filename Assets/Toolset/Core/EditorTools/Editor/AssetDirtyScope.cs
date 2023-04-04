using UnityEditor;
using UnityEngine;

namespace Toolset.Core.EditorTools
{
    /// <summary>
    /// At the end of the scope, calls AssetDatabase.Refresh if SetDirty was ever called.
    /// </summary>
    public class AssetDirtyScope : GUI.Scope
    {
        private bool m_dirty = false;

        public void SetDirty()
        {
            m_dirty = true;
        }
        
        protected override void CloseScope()
        {
            if (m_dirty)
                AssetDatabase.Refresh();
        }
    }
}
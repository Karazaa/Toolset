using UnityEditor;
using UnityEngine;

namespace Toolset.Core.EditorTools
{
    /// <summary>
    /// Simple base class for common EditorWindow helper utilities
    /// </summary>
    public abstract class ToolsetEditorWindow : EditorWindow
    {
        protected void BaseLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            GUILayout.Label(GetContent(text, tooltip, icon));
        }
        
        protected void BoldLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            GUILayout.Label(GetContent(text, tooltip, icon), EditorStyles.boldLabel);
        }

        protected bool BaseButton(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            return GUILayout.Button(GetContent(text, tooltip, icon));
        }

        private GUIContent GetContent(string text, string tooltip, EditorIcons.Presets icon)
        {
            if (icon == EditorIcons.Presets.None)
                return new GUIContent(text, tooltip);
            
            Texture iconTexture = EditorIcons.GetIcon(icon);
            return new GUIContent(text, iconTexture, tooltip);
        }
    }
}
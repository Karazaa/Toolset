using UnityEditor;
using UnityEngine;

namespace Toolset.Core.EditorTools
{
    /// <summary>
    /// Simple static class for common EditorWindow helper utilities
    /// </summary>
    public static class ToolsetEditorUtilities
    {
        public static void BaseLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            GUILayout.Label(GetContent(text, tooltip, icon));
        }
        
        public static void BoldLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            GUILayout.Label(GetContent(text, tooltip, icon), EditorStyles.boldLabel);
        }

        public static bool BaseButton(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
        {
            return GUILayout.Button(GetContent(text, tooltip, icon));
        }

        private static GUIContent GetContent(string text, string tooltip, EditorIcons.Presets icon)
        {
            if (icon == EditorIcons.Presets.None)
                return new GUIContent(text, tooltip);
            
            Texture iconTexture = EditorIcons.GetIcon(icon);
            return new GUIContent(text, iconTexture, tooltip);
        }
    }
    
    /// <summary>
    /// Simple base class for EditorWindows to easily access util Editor functions
    /// </summary>
    public abstract class ToolsetEditorWindow : EditorWindow
    {
        protected void BaseLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseLabel(text, tooltip, icon);

        protected void BoldLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BoldLabel(text, tooltip, icon);

        protected bool BaseButton(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseButton(text, tooltip, icon);
    }
    
    /// <summary>
    /// Simple base class for Editors to easily access util Editor functions
    /// </summary>
    public abstract class ToolsetEditor : Editor
    {
        protected void BaseLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseLabel(text, tooltip, icon);

        protected void BoldLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BoldLabel(text, tooltip, icon);

        protected bool BaseButton(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseButton(text, tooltip, icon);
    }
    
    /// <summary>
    /// Simple base class for any class to easily access util Editor functions
    /// </summary>
    public abstract class ToolsetEditorGUI
    {
        protected void BaseLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseLabel(text, tooltip, icon);

        protected void BoldLabel(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BoldLabel(text, tooltip, icon);

        protected bool BaseButton(string text, string tooltip = null, EditorIcons.Presets icon = EditorIcons.Presets.None)
            => ToolsetEditorUtilities.BaseButton(text, tooltip, icon);
    }
}
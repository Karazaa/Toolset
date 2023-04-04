using System;
using UnityEditor;
using UnityEngine;

namespace Toolset.Core.EditorTools
{
    /// <summary>
    /// Static helper class for making use of Unity's built-in Editor icons.
    /// </summary>
    public static class EditorIcons
    {
        public enum Presets
        {
            None,
            Info,
            InfoSmall,
            InfoInactive,
            InfoInactiveSmall,
            Warning,
            WarningSmall,
            WarningInactive,
            WarningInactiveSmall,
            Error,
            ErrorSmall,
            ErrorInactive,
            ErrorInactiveSmall,
        }
        
        public static Texture GetIcon(string iconName)
        {
            return EditorGUIUtility.FindTexture(iconName);
        }

        public static Texture GetIcon(Presets presetIcon)
        {
            // For a community-compiled list of available built-in icons
            // See https://github.com/halak/unity-editor-icons
            return presetIcon switch
            {
                Presets.None => null,
                Presets.Info => GetIcon("console.infoicon"),
                Presets.InfoSmall => GetIcon("console.infoicon.sml"),
                Presets.InfoInactive => GetIcon("console.infoicon.inactive.sml@2x"),
                Presets.InfoInactiveSmall => GetIcon("console.infoicon.inactive.sml"),
                Presets.Warning => GetIcon("console.warnicon"),
                Presets.WarningSmall => GetIcon("console.warnicon.sml"),
                Presets.WarningInactive => GetIcon("console.warnicon.inactive.sml@2x"),
                Presets.WarningInactiveSmall => GetIcon("console.warnicon.inactive.sml"),
                Presets.Error => GetIcon("console.erroricon"),
                Presets.ErrorSmall => GetIcon("console.erroricon.sml"),
                Presets.ErrorInactive => GetIcon("console.erroricon.inactive.sml@2x"),
                Presets.ErrorInactiveSmall => GetIcon("console.erroricon.inactive.sml"),
                _ => throw new ArgumentOutOfRangeException(nameof(presetIcon), presetIcon, null)
            };
        }
    }
}
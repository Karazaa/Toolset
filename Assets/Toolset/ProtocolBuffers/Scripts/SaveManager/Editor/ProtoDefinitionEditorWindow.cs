using System.Collections.Generic;
using Toolset.Core;
using Toolset.Core.EditorTools;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A custom editor that allows developers to define/edit new proto records. This makes it so that
    /// users don't have to memorize Proto3 syntax and can instead have an easier way of making proto records.
    /// </summary>
    public class ProtoDefinitionEditorWindow : ToolsetEditorWindow
    {
        private readonly List<ProtoDefinitionListItem> m_protoDefinitionListItems = new List<ProtoDefinitionListItem>();
        private Vector2 m_scrollPosition = Vector2.zero;
        
        /// <summary>
        /// Opens the Proto Definition Editor Window.
        /// </summary>
        [MenuItem("Toolset/Static Data/Proto Definition Editor")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<ProtoDefinitionEditorWindow>("Proto Definitions");
        }

        public void OnEnable()
        {
            PopulateWindow();
        }
        
        public void OnGUI()
        {
            BoldLabel("Proto Definitions");

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, false, true);
            foreach (ProtoDefinitionListItem listItem in m_protoDefinitionListItems)
            {
                listItem.OnGui();
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.BeginHorizontal();
            if (BaseButton("Add New Proto Definition"))
            {
                m_protoDefinitionListItems.Add(new ProtoDefinitionListItem());
            }
                
            if (m_protoDefinitionListItems.Count > 0 && BaseButton("Remove Last Proto Definition"))
            {
                DeleteProtoFileAtIndex(m_protoDefinitionListItems.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (BaseButton("Refresh"))
            {
                PopulateWindow();
            }
            if (BaseButton("Save and Generate Proto Files"))
            {
                SaveProtoFiles();
                StaticDataControlUtils.GenerateProto();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SaveProtoFiles()
        {
            foreach (ProtoDefinitionListItem listItem in m_protoDefinitionListItems)
            {
                listItem.Save();
            }
        }

        private void DeleteProtoFileAtIndex(int index)
        {
            m_protoDefinitionListItems[index].Delete();
            m_protoDefinitionListItems.RemoveAt(index);
        }

        private void PopulateWindow()
        {
            m_protoDefinitionListItems.Clear();
            
            List<string> fileContents = SaveManager.LoadAllFileContentsFromDirectory(ToolsetEditorConstants.s_pathToProtoDataDirectory, ToolsetRuntimeConstants.c_protoSearchPattern);
            foreach (string fileContent in fileContents)
            {
                if (!fileContent.StartsWith(ToolsetEditorConstants.c_protoEditorOptOutToken))
                 m_protoDefinitionListItems.Add(new ProtoDefinitionListItem(fileContent));
            }
        }
    }
}

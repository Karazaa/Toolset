using System.Collections.Generic;
using Toolset.Core;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A custom editor that allows developers to define/edit new proto records. This makes it so that
    /// users don't have to memorize Proto3 syntax and can instead have an easier way of making proto records.
    /// </summary>
    public class ProtoDefinitionEditorWindow : EditorWindow
    {
        private const string c_editorOptOutToken = "//PROTO_EDITOR_OPT_OUT";
        private readonly List<ProtoDefinitionListItem> m_protoDefinitionListItems = new List<ProtoDefinitionListItem>();
        private Vector2 m_scrollPosition = Vector2.zero;
        
        /// <summary>
        /// Opens the Proto Definition Window.
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
            GUILayout.Label("Proto Definitions", EditorStyles.boldLabel);

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, false, true);
            foreach (ProtoDefinitionListItem listItem in m_protoDefinitionListItems)
            {
                listItem.OnGui();
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Proto Definition"))
            {
                m_protoDefinitionListItems.Add(new ProtoDefinitionListItem());
            }
                
            if (m_protoDefinitionListItems.Count > 0 && GUILayout.Button("Remove Last Proto Definition"))
            {
                m_protoDefinitionListItems.RemoveAt(m_protoDefinitionListItems.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                PopulateWindow();
            }
            if (GUILayout.Button("Generate Proto Files"))
            {
                CreateSaveProtoFiles();
                StaticDataControlUtils.GenerateProto();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateSaveProtoFiles()
        {
            foreach (ProtoDefinitionListItem listItem in m_protoDefinitionListItems)
            {
                string fileName = listItem.GetProtoFileName();
                SaveManager.SaveContentsToFile(fileName, ToolsetEditorConstants.s_pathToProtoDataDirectory, listItem.GetProtoFileContents());
                Debug.Log("Saved new Proto Definition: ".StringBuilderAppend(fileName));
            }
        }

        private void PopulateWindow()
        {
            m_protoDefinitionListItems.Clear();
            
            List<string> fileContents = SaveManager.LoadAllFileContentsFromDirectory(ToolsetEditorConstants.s_pathToProtoDataDirectory, ToolsetEditorConstants.s_protoFileSearchPattern);
            foreach (string fileContent in fileContents)
            {
                if (!fileContent.StartsWith(c_editorOptOutToken))
                 m_protoDefinitionListItems.Add(new ProtoDefinitionListItem(fileContent));
            }
        }
    }
}

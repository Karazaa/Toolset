using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class ProtoDefinitionWindow : EditorWindow
    {
        private List<ProtoDefinitionListItem> m_protoDefinitionListItems = new List<ProtoDefinitionListItem>();
        private Vector2 m_scrollPosition = Vector2.zero;
        
        /// <summary>
        /// Opens the Proto Definition Window.
        /// </summary>
        [MenuItem("Toolset/Data/Open Proto Definition Window")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<ProtoDefinitionWindow>("Proto Definitions");
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
        }
    }
}
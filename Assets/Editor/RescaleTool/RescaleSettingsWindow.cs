//Copyright (C) MichaelLam, 2024, All Rights Reserved.
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VFS.Tools{
    public class RescaleSettingsWindow : EditorWindow
    {
        public void ShowWindow()
        {
            GetWindow<RescaleSettingsWindow>("Rescale Settings");
        }

        void OnGUI()
        {

            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(RescalePrefab.BATCHPATH))
            {
                string[] _guids = AssetDatabase.FindAssets("CommandScript");
                if (_guids != null && _guids.Length > 0)
                {
                    RescalePrefab.BATCHPATH = Path.GetDirectoryName(Application.dataPath) + "/" + AssetDatabase.GUIDToAssetPath(_guids[0]);
                }
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Batch File Path:");
            RescalePrefab.BATCHPATH = EditorGUILayout.TextField(RescalePrefab.BATCHPATH, GUILayout.MaxWidth(200));


            if (GUILayout.Button("Browse", GUILayout.MaxWidth(100)))
            {
                RescalePrefab.BATCHPATH = EditorUtility.OpenFilePanel("Select bat", Application.dataPath, "bat");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Maya Location:");
            RescalePrefab.MAYAPATH = EditorGUILayout.TextField(RescalePrefab.MAYAPATH, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Browse",GUILayout.MaxWidth(100)))
            {

                RescalePrefab.MAYAPATH = EditorUtility.OpenFilePanel("Select bat", "C:/Program Files/Autodesk/Maya2024/bin", "bat");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Preference", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Auto scale back to 1 when reimported");
            RescalePrefab._autoScaleBack = EditorGUILayout.Toggle("", RescalePrefab._autoScaleBack);
            EditorGUILayout.EndHorizontal();

        }

    }
}

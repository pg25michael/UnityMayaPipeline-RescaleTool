//Copyright (C) MichaelLam, 2024, All Rights Reserved.
using UnityEditor;
using UnityEngine;

namespace VFS.Tools
{
    public class RescaleGameObjectsWindow : EditorWindow
    {
        public bool[] _gameObjectToggles;
        public int selectedObjectCount;

        public void ShowWindow()
        {
            _gameObjectToggles = new bool[RescalePrefab.gos.Length];
            for (int i = 0; i < RescalePrefab.gos.Length; i++)
            {
                _gameObjectToggles[i] = true;
            }
            GetWindow<RescaleGameObjectsWindow>("Set Scale");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Selected Objects");


            for(int i = 0;i< RescalePrefab.gos.Length;i++)
            {
                EditorGUILayout.BeginHorizontal();
                _gameObjectToggles[i] = EditorGUILayout.Toggle("", _gameObjectToggles[i],GUILayout.MaxWidth(10));
                EditorGUILayout.LabelField(RescalePrefab.gos[i].name);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Confirmed"))
            {
                RescalePrefab.RunScript();
                this.Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
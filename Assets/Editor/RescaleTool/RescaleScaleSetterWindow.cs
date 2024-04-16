//Copyright (C) MichaelLam, 2024, All Rights Reserved.
using UnityEditor;
using UnityEngine;

namespace VFS.Tools
{

    public class RescaleScaleSetterWindow : EditorWindow
    {
        private float _scaleX = 1f;
        private float _scaleY = 1f;
        private float _scaleZ = 1f;
        private bool confirmed;

        public void ShowWindow()
        {
            GetWindow<RescaleScaleSetterWindow>("Set Scale");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Relative Scale");
            EditorGUIUtility.labelWidth = 10;
            _scaleX = EditorGUILayout.FloatField("X", _scaleX, GUILayout.ExpandWidth(false));
            _scaleY = EditorGUILayout.FloatField("Y", _scaleY, GUILayout.ExpandWidth(false));
            _scaleZ = EditorGUILayout.FloatField("Z", _scaleZ, GUILayout.ExpandWidth(false));



            EditorGUILayout.EndHorizontal();

            if(GUILayout.Button("Confirmed"))
            {
                RescalePrefab._scaleString = GetScaleString();
                RescalePrefab.RunScript();
                this.Close();
            }
            

            EditorGUILayout.EndVertical();
        }

        public string GetScaleString()
        {
            return RescalePrefab.GetScaleString(_scaleX, _scaleY, _scaleZ);
        }

        public bool Confirmed()
        {
            return confirmed;
        }
    }
}

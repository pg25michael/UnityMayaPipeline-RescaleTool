//Copyright (C) MichaelLam, 2024, All Rights Reserved.
using System.IO;
using UnityEditor;
using UnityEngine;




namespace VFS.Tools
{
    public class RescalePrefab
    {
        static public string BATCHPATH = "";
        static public string MAYAPATH = "C:\\Program Files\\Autodesk\\Maya2024\\bin";
        static public string LOADPLUGIN = "loadPlugin fbxmaya.mll;";
        static public string _objectName = "";
        static public string _scaleString = "";
        static public bool _autoScaleBack = true;
        
        private static string Arguments => "\"" + MAYAPATH + "\" \"" + LOADPLUGIN + MelCommands + "\"";

        public static string MelCommands = "";
        public static RescaleScaleSetterWindow _setterWindow;
        public static RescaleSettingsWindow _settings;
        public static RescaleGameObjectsWindow _gameObjectsWindow;
        public static GameObject[] gos;

        //setting Window
        [MenuItem("Tools/VFS/RescaleTool/Settings")]
        public static void SettingWindow()
        {
            if (_settings == null)
            _settings = (RescaleSettingsWindow)RescaleSettingsWindow.CreateInstance("RescaleSettingsWindow");
            _settings.ShowWindow();
        }

        //direct select prefab in Asset folder, show scale selector window, single object only
        [MenuItem("Assets/RescaleTool/Rescale")]
        public static void ReScalePrefab()
        {
            //working single prefab

            //_selectedPrefab = Selection.activeGameObject;
            gos[0] = Selection.activeGameObject;

            if (_setterWindow == null)
                _setterWindow = (RescaleScaleSetterWindow)RescaleScaleSetterWindow.CreateInstance("RescaleScaleSetterWindow");
            
            _setterWindow.ShowWindow();
            _scaleString = _setterWindow.GetScaleString();
        }

        //select transform to perform rescale, single object only
        [MenuItem("CONTEXT/Transform/Rescale/Using This Scale")]
        public static void ReScaleThisTransform(MenuCommand menuCommand)
        {
            Transform transform = menuCommand.context as Transform;
            //ReScaleWithCurrentScale(transform);
            Vector3 scale = transform.localScale;

            //Debug.Log(PrefabUtility.IsPartOfAnyPrefab(transform.gameObject));
            //Debug.Log(PrefabUtility.IsPartOfPrefabAsset(transform.gameObject));

                gos = new GameObject[1];
                gos[0] = transform.gameObject;
                RunScript();
        }

        //select game object in hierarchy
        [MenuItem("GameObject/Prefab/Rescale/GameObjects")]
        public static void ReScaleThisGameObject(MenuCommand menuCommand)
        {
            /*
            GameObject go = menuCommand.context as GameObject;
            ReScaleWithCurrentScale(go.transform);
            */
            if(gos == null)
                gos = Selection.gameObjects;
            if(_gameObjectsWindow == null) { 
                _gameObjectsWindow = (RescaleGameObjectsWindow)RescaleGameObjectsWindow.CreateInstance("RescaleGameObjectsWindow");
                _gameObjectsWindow.ShowWindow();
            }
        }

        //run batch script with confirmation
        public static void RunScript()
        {
            bool runBatch = EditorUtility.DisplayDialog("Confirmation", "Rescale selected " + gos.Length + " objects ?", "Yes", "Cancel");
            if (runBatch)
            {
                if (GetCombinedMelCommand())
                {
                    if (string.IsNullOrEmpty(BATCHPATH))
                    {
                        string[] _guids = AssetDatabase.FindAssets("CommandScript");
                        if (_guids != null && _guids.Length > 0)
                        {
                            BATCHPATH = Path.GetDirectoryName(Application.dataPath) + "/" + AssetDatabase.GUIDToAssetPath(_guids[0]);
                            Debug.Log(BATCHPATH);
                        }
                    }
                    if (!string.IsNullOrEmpty(BATCHPATH))
                    {
                        
                        var process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = BATCHPATH;
                        process.StartInfo.Arguments = Arguments;
                        process.Start();

                        process.WaitForExit(20000);

                        if (process.HasExited)
                        {
                            Debug.Log("Script End");
                            OnScriptEnd();
                        }
                        else
                        {
                            Debug.Log("Kill Process");
                            process.Kill();
                            EditorUtility.DisplayDialog("Error", "Process took too long to complete", "OK");
                            CleanUp();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "file path is empty!", "OK");
                        CleanUp();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Mel command empty!", "OK");
                }
            }
            else
            {
                CleanUp();
            }

        }

        //clean up strings and arrays
        public static void CleanUp()
        {
            gos = null;
            _scaleString = "";
            MelCommands = "";
        }

        //call upon script end
        public static void OnScriptEnd()
        {
            AssetDatabase.Refresh();
            if (_autoScaleBack)
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    gos[i].transform.localScale = Vector3.one;
                }
            }
            CleanUp();
        }

        //get Melcommand for single object
        public static string MelCommand(GameObject selectedGameObject,string scaleString)
        {
            Object selectedPrefab = null;
            if (PrefabUtility.IsPartOfAnyPrefab(selectedGameObject))
            {
                selectedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(selectedGameObject);
            }
            else
            {
                selectedPrefab = selectedGameObject;
            }
            string directory = Path.GetDirectoryName(Application.dataPath);
            string assetPath = AssetDatabase.GetAssetPath(selectedPrefab);
            
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(directory)) {
                EditorUtility.DisplayDialog("Error", $"Asset Path is Empty for {selectedGameObject.name}!", "OK");
                return ""; 
            }
            
            string objPath =  directory + "\\" + assetPath;
            objPath = objPath.Replace("\\","/");

            string melCommand = $"FBXImport -f \"\"{objPath}\"\";select -allDagObjects;scale {scaleString};DeleteHistory;FreezeTransformations;FBXExport -f \"\"{objPath}\"\";quit -f;";

            return melCommand;
        }

        //get the combined Mel command for every object selected
        public static bool GetCombinedMelCommand()
        {
            bool filterBool = true;
            for (int i = 0; i < gos.Length; i++)
            {
                if(_gameObjectsWindow != null)
                {
                    if(_gameObjectsWindow._gameObjectToggles != null)
                    {
                        filterBool = _gameObjectsWindow._gameObjectToggles[i];
                    }
                }
                if (filterBool)
                {
                    if(i > 0)
                    {
                        MelCommands += "delete;";
                    }
                    Transform t = gos[i].transform;
                    if (string.IsNullOrEmpty(_scaleString)) _scaleString = GetScaleString(t.localScale.x, t.localScale.y, t.localScale.z);
                    MelCommands += MelCommand(gos[i], _scaleString );
                    _scaleString = "";
                }
            }
            return !string.IsNullOrEmpty(MelCommands);
            
        }

        //get the according scale string for the mel command 
        public static string GetScaleString(float x, float y, float z)
        {
            return $"{x} {y} {z}";
        }



        //----------------DEBUG-------
        public static void PrintList()
        {
            for(int i = 0;i < gos.Length;i++)
            {
                Debug.Log(gos[i].name);
            }
            Debug.Log(_scaleString);
        }


    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JSW.SceneListManager;

namespace JSW.SceneListManager.Editor
{
    public class SceneListEditor : EditorWindow
    {
        private Vector2 scrollPosition;

        private UnityEngine.Object selectedAsset;

        static SceneListSO sceneListSo;


        [MenuItem("Window/Scene List Manager")]
        public static void ShowWindow()
        {
            // SceneListSo 데이터 파일 있는지 확인
            string[] paths = AssetDatabase.FindAssets("t:" + typeof(SceneListSO).Name);
            if (paths.Length == 0)
            {
                string sceneListSoFolderPath = "Assets/Unity-SceneListManager/Data";

                sceneListSo = ScriptableObject.CreateInstance<SceneListSO>();

                if (AssetDatabase.IsValidFolder(sceneListSoFolderPath))
                {
                    AssetDatabase.CreateAsset(sceneListSo, sceneListSoFolderPath + "/SceneListSO.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    AssetDatabase.CreateFolder("Assets/Unity-SceneListManager", "Data");

                    AssetDatabase.CreateAsset(sceneListSo, sceneListSoFolderPath + "/SceneListSO.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                string sceneListSOPath = AssetDatabase.GUIDToAssetPath(paths[0]);
                sceneListSo = AssetDatabase.LoadAssetAtPath<SceneListSO>(sceneListSOPath);
            }

            GetWindow<SceneListEditor>("Scene List");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Scene List", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            List<SceneAsset> scenesToRemove = new List<SceneAsset>();
            List<SceneAsset> scenesToRearrange = new List<SceneAsset>();
            SceneAsset upToScene = null;
            SceneAsset downToScene = null;

            foreach (SceneAsset scene in sceneListSo.sceneList)
            {
                string scenePath = AssetDatabase.GetAssetPath(scene);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(scenePath);

                GUIStyle openButtonStyle = new GUIStyle(GUI.skin.button);
                openButtonStyle.fixedWidth = 80;

                if (GUILayout.Button("Open", openButtonStyle))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }

                GUIStyle additiveButtonStyle = new GUIStyle(GUI.skin.button);
                additiveButtonStyle.fixedWidth = 80;

                GUILayout.Space(5);
                if (GUILayout.Button("Additive", additiveButtonStyle))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                GUIStyle arrowButton = new GUIStyle(GUI.skin.button);
                GUILayout.Space(10);
                arrowButton.fixedWidth = 30;
                if (GUILayout.Button('\u25B2'.ToString(), arrowButton))
                {
                    upToScene = scene;
                }

                if (GUILayout.Button('\u25BC'.ToString(), arrowButton))
                {
                    downToScene = scene;
                }

                GUIStyle removeButtonStyle = new GUIStyle(GUI.skin.button);
                removeButtonStyle.fixedWidth = 25;
                removeButtonStyle.normal.textColor = Color.yellow;
                GUILayout.Space(10);

                if (GUILayout.Button("X", removeButtonStyle))
                {
                    scenesToRemove.Add(scene);
                }
                EditorGUILayout.EndHorizontal();
            }

            foreach (var removedScene in scenesToRemove)
            {
                sceneListSo.sceneList.Remove(removedScene);
                sceneListSo.Save();
            }

            if (upToScene != null)
            {
                sceneListSo.UpToIndex(upToScene);
            }

            if (downToScene != null)
            {
                sceneListSo.DownToIndex(downToScene);
            }

            EditorGUILayout.EndScrollView();
            selectedAsset = EditorGUILayout.ObjectField("Scene:", selectedAsset, typeof(SceneAsset), false);

            if (GUILayout.Button("Add Scene"))
            {
                if (selectedAsset == null) return;
                if (sceneListSo.sceneList.Any(x => x == selectedAsset)) return;

                string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
                sceneListSo.sceneList.Add((SceneAsset)selectedAsset);
                sceneListSo.Save();
            }
        }
    }

}

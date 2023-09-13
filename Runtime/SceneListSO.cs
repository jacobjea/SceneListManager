using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JSW.SceneListManager
{
    public class SceneListSO : ScriptableObject
    {
        public List<SceneAsset> sceneList = new List<SceneAsset>();

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }

        public void UpToIndex(SceneAsset scene)
        {
            int targetIndex = sceneList.IndexOf(scene);
            if (targetIndex == 0) return;

            RePlaceList(targetIndex, targetIndex - 1);
        }

        public void DownToIndex(SceneAsset scene)
        {
            int targetIndex = sceneList.IndexOf(scene);
            if (targetIndex == sceneList.Count - 1) return;

            RePlaceList(targetIndex, targetIndex + 1);
        }

        private void RePlaceList(int targetIndex, int replaceIndex)
        {
            SceneAsset temp = sceneList[replaceIndex];
            sceneList[replaceIndex] = sceneList[targetIndex];
            sceneList[targetIndex] = temp;

            Save();
        }
    }
}

using UnityEngine;
using UnityEditor;

namespace MarwanZaky
{
    public class ReplaceGameObjects : ScriptableWizard
    {
        public GameObject newGameObject;
        public GameObject[] oldGameObjects;

        [MenuItem("Tools/Replace GameObjects")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
        }

        void OnWizardCreate()
        {
            foreach (GameObject go in oldGameObjects)
            {
                var newGo = (GameObject)EditorUtility.InstantiatePrefab(newGameObject);

                newGo.transform.position = go.transform.position;
                newGo.transform.rotation = go.transform.rotation;
                newGo.transform.parent = go.transform.parent;

                DestroyImmediate(go);
            }
        }
    }
}
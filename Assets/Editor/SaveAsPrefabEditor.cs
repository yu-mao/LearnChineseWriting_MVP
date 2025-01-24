using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(PrefabSaver))]
public class SaveAsPrefabEditor : Editor
{
    private PrefabSaver prefabSaver;
    public string prefabPath = "Assets/Prefabs/Planes/PrefabName.prefab";

    private void OnEnable()
    {
        prefabSaver = (PrefabSaver)target;
    }

    public override void OnInspectorGUI()
    {
        prefabSaver.objectToSave = (GameObject)EditorGUILayout.ObjectField("Object to Save", prefabSaver.objectToSave, typeof(GameObject), true);
        prefabPath = EditorGUILayout.TextField("Prefab Path", prefabPath);

        if (GUILayout.Button("Save"))
        {
            SavePrefab();
        }

        base.OnInspectorGUI();
    }

    private void SavePrefab()
    {
        if (prefabSaver.objectToSave != null)
        {
            string directory = Path.GetDirectoryName(prefabPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            PrefabUtility.SaveAsPrefabAsset(prefabSaver.objectToSave, prefabPath);
            Debug.Log("Prefab saved in: " + prefabPath);
        }
        else
        {
            Debug.LogError("there is not an game object to save...");
        }
    }
}

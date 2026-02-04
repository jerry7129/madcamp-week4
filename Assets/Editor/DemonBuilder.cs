using UnityEngine;
using UnityEditor;

public class DemonBuilder
{
    [MenuItem("Tools/Create Demon")]
    public static void CreateDemon()
    {
        string prefabPath = "Assets/Prefabs/Enemy/Demon.prefab";
        GameObject demonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (demonPrefab == null)
        {
            Debug.LogError($"Could not find Demon Prefab at {prefabPath}. Please check the path.");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(demonPrefab);
        instance.name = "Demon"; // Optional: Reset name
        instance.transform.position = Vector3.zero; // Or near Scene View center

        Undo.RegisterCreatedObjectUndo(instance, "Create Demon");
        Selection.activeGameObject = instance;
        
        Debug.Log("Demon Prefab Instantiated!");
    }
}

using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility to remove missing script components from all GameObjects
/// </summary>
public static class RemoveMissingScripts
{
    [MenuItem("Tools/Remove Missing Scripts From Scene")]
    public static void RemoveMissingScriptsFromScene()
    {
        int totalRemoved = 0;
        
        // Get all GameObjects in the scene (including inactive)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            // Skip prefabs in the project (only process scene objects)
            if (EditorUtility.IsPersistent(go))
                continue;
                
            // Count missing scripts
            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            
            if (missingCount > 0)
            {
                // Remove missing scripts
                Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                totalRemoved += missingCount;
                Debug.Log($"Removed {missingCount} missing script(s) from: {go.name}");
            }
        }
        
        if (totalRemoved > 0)
        {
            Debug.Log($"<color=green>Total removed: {totalRemoved} missing script(s)</color>");
            EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        }
        else
        {
            Debug.Log("No missing scripts found in scene!");
        }
    }
    
    [MenuItem("Tools/Remove Missing Scripts From All Prefabs")]
    public static void RemoveMissingScriptsFromAllPrefabs()
    {
        int totalRemoved = 0;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                int removed = RemoveMissingScriptsFromPrefab(prefab, path);
                totalRemoved += removed;
            }
        }
        
        if (totalRemoved > 0)
        {
            Debug.Log($"<color=green>Total removed from prefabs: {totalRemoved} missing script(s)</color>");
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.Log("No missing scripts found in prefabs!");
        }
    }
    
    private static int RemoveMissingScriptsFromPrefab(GameObject prefab, string path)
    {
        int totalRemoved = 0;
        
        // Get all transforms in the prefab hierarchy
        Transform[] allTransforms = prefab.GetComponentsInChildren<Transform>(true);
        
        foreach (Transform t in allTransforms)
        {
            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
            
            if (missingCount > 0)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                totalRemoved += missingCount;
                Debug.Log($"Removed {missingCount} missing script(s) from prefab: {path} ({t.gameObject.name})");
            }
        }
        
        if (totalRemoved > 0)
        {
            PrefabUtility.SavePrefabAsset(prefab);
        }
        
        return totalRemoved;
    }
}

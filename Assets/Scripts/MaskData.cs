using UnityEngine;

/// <summary>
/// ScriptableObject defining mask properties.
/// Create assets via: Right-click → Create → GGJ26 → MaskData
/// </summary>
[CreateAssetMenu(fileName = "NewMask", menuName = "GGJ26/MaskData")]
public class MaskData : ScriptableObject
{
    [Tooltip("Display name for this mask")]
    public string maskName;
    
    [Tooltip("Icon for UI display")]
    public Sprite icon;
    
    [Tooltip("Prefab for visual representation when equipped")]
    public GameObject prefab;
}
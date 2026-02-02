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

    [Tooltip("Icon to display when this mask is selected")]
    public Sprite selectedIcon;
    
    [Tooltip("Prefab for visual representation when equipped")]
    public GameObject prefab;

    [Header("Ability")]
    [Tooltip("The ability granted by this mask.")]
    public GGJ26.Abilities.MaskAbility ability;
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's mask inventory (3 slots max).
/// Attach to Player GameObject.
/// 
/// Design notes:
/// - Stores MaskData (ScriptableObjects), not GameObjects
/// - Duplicate masks ARE allowed (same MaskData can be collected multiple times)
/// - Inventory owns all add/remove logic
/// </summary>
public class MaskInventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxSlots = 3;
    
    [Header("Debug")]
    [SerializeField] private List<MaskData> collectedMasks = new();
    
    // Events for UI/VFX - fires with mask data and slot index
    public event Action<MaskData, int> OnMaskAdded;
    public event Action<MaskData, int> OnMaskRemoved;
    public event Action OnInventoryFull;
    
    // Properties
    public int MaxSlots => maxSlots;
    public int MaskCount => collectedMasks.Count;
    public bool IsFull => collectedMasks.Count >= maxSlots;
    public IReadOnlyList<MaskData> Masks => collectedMasks;
    
    /// <summary>
    /// Attempts to add a mask to inventory.
    /// </summary>
    /// <returns>True if added successfully, false if inventory full or mask is null</returns>
    public bool TryAddMask(MaskData mask)
    {
        if (mask == null)
        {
            Debug.LogWarning("MaskInventory: Attempted to add null mask");
            return false;
        }
        
        if (IsFull)
        {
            Debug.Log($"MaskInventory: Inventory full ({MaskCount}/{maxSlots})");
            OnInventoryFull?.Invoke();
            return false;
        }
        
        int slotIndex = collectedMasks.Count;
        collectedMasks.Add(mask);
        
        Debug.Log($"MaskInventory: Added '{mask.maskName}' to slot {slotIndex} ({MaskCount}/{maxSlots})");
        OnMaskAdded?.Invoke(mask, slotIndex);
        
        return true;
    }
    
    /// <summary>
    /// Removes a mask at the specified slot index.
    /// </summary>
    public bool TryRemoveMask(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= collectedMasks.Count)
        {
            Debug.LogWarning($"MaskInventory: Invalid slot index {slotIndex}");
            return false;
        }
        
        MaskData mask = collectedMasks[slotIndex];
        collectedMasks.RemoveAt(slotIndex);
        
        Debug.Log($"MaskInventory: Removed '{mask.maskName}' from slot {slotIndex}");
        OnMaskRemoved?.Invoke(mask, slotIndex);
        
        return true;
    }
    
    /// <summary>
    /// Gets mask at specific slot, or null if slot is empty/invalid.
    /// </summary>
    public MaskData GetMaskAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= collectedMasks.Count)
            return null;
        return collectedMasks[slotIndex];
    }
    
    /// <summary>
    /// Checks if inventory contains a specific mask type.
    /// </summary>
    public bool HasMask(MaskData mask)
    {
        return collectedMasks.Contains(mask);
    }
}
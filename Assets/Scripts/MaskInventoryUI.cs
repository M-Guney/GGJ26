using System.Collections.Generic;
using UnityEngine;

public class MaskInventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MaskInventory maskInventory;
    [SerializeField] private List<MaskInventorySlot> slots;

    private void Start()
    {
        // Try to find inventory if not assigned
        if (maskInventory == null)
        {
            maskInventory = FindObjectOfType<MaskInventory>();
        }

        if (maskInventory != null)
        {
            // Subscribe to events
            maskInventory.OnMaskAdded += HandleMaskAdded;
            maskInventory.OnMaskRemoved += HandleMaskRemoved;
            
            // Initialize view
            RefreshUI();
        }
        else
        {
            Debug.LogError("MaskInventoryUI: Could not find MaskInventory!");
        }
    }

    private void OnDestroy()
    {
        if (maskInventory != null)
        {
            maskInventory.OnMaskAdded -= HandleMaskAdded;
            maskInventory.OnMaskRemoved -= HandleMaskRemoved;
        }
    }

    private void HandleMaskAdded(MaskData mask, int index)
    {
        UpdateSlot(index, mask);
    }

    private void HandleMaskRemoved(MaskData mask, int index)
    {
        // When a mask is removed, we might need to shift items or just clear that slot.
        // For now, let's just refresh everything to match the inventory state exactly.
        RefreshUI();
    }

    private void UpdateSlot(int index, MaskData mask)
    {
        if (index >= 0 && index < slots.Count)
        {
            slots[index].SetMask(mask);
        }
        else
        {
            Debug.LogWarning($"MaskInventoryUI: Slot index {index} out of range (Slots: {slots.Count})");
        }
    }

    private void RefreshUI()
    {
        if (maskInventory == null) return;

        // Iterate through all slots
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < maskInventory.MaskCount)
            {
                // Slot has item
                slots[i].SetMask(maskInventory.GetMaskAtSlot(i));
            }
            else
            {
                // Slot is empty
                slots[i].ClearSlot();
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MaskInventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MaskInventory maskInventory;
    [SerializeField] private MaskSelector maskSelector; // Need this to know which is selected
    [SerializeField] private List<MaskInventorySlot> slots;

    private int _currentSelectedIndex = -1;

    private void Start()
    {
        // Try to find inventory if not assigned
        if (maskInventory == null) maskInventory = FindObjectOfType<MaskInventory>();
        if (maskSelector == null) maskSelector = FindObjectOfType<MaskSelector>();

        if (maskInventory != null)
        {
            // Subscribe to inventory events
            maskInventory.OnMaskAdded += HandleMaskAdded;
            maskInventory.OnMaskRemoved += HandleMaskRemoved;
        }

        if (maskSelector != null)
        {
             // Subscribe to selection events
             maskSelector.OnMaskEquipped += HandleMaskEquipped;
        }
        
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (maskInventory != null)
        {
            maskInventory.OnMaskAdded -= HandleMaskAdded;
            maskInventory.OnMaskRemoved -= HandleMaskRemoved;
        }

        if (maskSelector != null)
        {
            maskSelector.OnMaskEquipped -= HandleMaskEquipped;
        }
    }

    private void HandleMaskAdded(MaskData mask, int index)
    {
        UpdateSlot(index, mask);
    }

    private void HandleMaskRemoved(MaskData mask, int index)
    {
        RefreshUI();
    }

    private void HandleMaskEquipped(int index)
    {
        _currentSelectedIndex = index;
        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            // It is selected ONLY if it matches index AND has a valid mask (optional check, but good for safety)
            bool isSelected = (i == _currentSelectedIndex);
            slots[i].SetSelected(isSelected);
        }
    }

    public void ShowCooldownFeedback(int index)
    {
        if (index >= 0 && index < slots.Count)
        {
            slots[index].AnimateError();
        }
    }

    private void UpdateSlot(int index, MaskData mask)
    {
        if (index >= 0 && index < slots.Count)
        {
            slots[index].SetMask(mask);
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
        
        // Ensure selection is correct after refresh
        UpdateSelectionVisuals();
    }
}

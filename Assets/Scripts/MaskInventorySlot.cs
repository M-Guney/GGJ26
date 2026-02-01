using UnityEngine;
using UnityEngine.UI;

public class MaskInventorySlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite emptyIcon; 

    private MaskData _currentMask;

    public void SetMask(MaskData mask)
    {
        _currentMask = mask;
        
        if (mask != null && iconImage != null)
        {
            iconImage.sprite = mask.icon;
            iconImage.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        _currentMask = null;
        
        if (iconImage != null)
        {
            iconImage.sprite = emptyIcon;
            iconImage.enabled = emptyIcon != null;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MaskInventorySlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite emptyIcon; 
    [SerializeField] private GameObject selectionHighlight; // New reference for the border/glow

    private MaskData _currentMask;

    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(isSelected);
    }

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
            iconImage.color = Color.white; // Reset color
        }
    }

    public void AnimateError()
    {
        if (iconImage != null)
        {
            // Kill any existing color tweens to prevent stuck colors
            iconImage.DOKill();
            iconImage.color = Color.white;
            
            // Flash Red then back to White
            iconImage.DOColor(Color.red, 0.2f).OnComplete(() =>
            {
                iconImage.DOColor(Color.white, 0.2f);
            });
            
            // Optional: Small shake of the icon itself
            iconImage.rectTransform.DOShakeAnchorPos(10f, 0.3f, 10, 90);
        }
    }
}

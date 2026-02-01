using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // Logic requires DOTween
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MaskSelector : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform where the mask prefab will be instantiated (e.g., Head/Face).")]
    [SerializeField] private Transform _maskRoot;
    
    [Tooltip("Reference to the MaskInventory.")]
    [SerializeField] private MaskInventory _inventory;

    [Header("Settings")]
    [Tooltip("If true, automatically selects the new mask when picked up.")]
    [SerializeField] private bool _autoEquipOnPickup = true;

    [Header("Animations")]
    [SerializeField] private bool _useAnimations = true;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpPower = 2f;
    [SerializeField] private float _jumpDuration = 0.5f;
    [Tooltip("Optional: If assigned, mask starts from here (e.g., Hand)")]
    [SerializeField] private Transform _animationOrigin;

    [Header("Scale Settings")]
    [SerializeField] private bool _useScaleAnimation = true;
    [SerializeField] private Vector3 _targetScale = Vector3.one;
    [SerializeField] private float _scaleDuration = 0.5f;

    [Header("Feedback")]
    [SerializeField] private MaskInventoryUI _ui;
    [SerializeField] private Transform _cameraShakeTarget;
    [SerializeField] private float _shakeIntensity = 0.5f;
    [SerializeField] private float _shakeDuration = 0.3f;

    private int _currentIndex = -1;
    private GameObject _currentMaskObject;
    private MaskData _currentMaskData;

    public event System.Action<int> OnMaskEquipped; // Event for UI

    private void Start()
    {
        if (_inventory == null) _inventory = GetComponent<MaskInventory>();
        if (_ui == null) _ui = FindObjectOfType<MaskInventoryUI>(); // Auto-find UI

        // Try to find default camera target if not assigned (specific for StarterAssets)
        if (_cameraShakeTarget == null)
        {
            var tpc = GetComponent<StarterAssets.ThirdPersonController>();
            if (tpc != null) _cameraShakeTarget = tpc.CinemachineCameraTarget.transform;
        }

        if (_inventory != null)
        {
            _inventory.OnMaskAdded += HandleMaskAdded;
            _inventory.OnMaskRemoved += HandleMaskRemoved;
        }
        else
        {
            Debug.LogError("MaskSelector: No MaskInventory found!");
        }
    }

    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnMaskAdded -= HandleMaskAdded;
            _inventory.OnMaskRemoved -= HandleMaskRemoved;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Direct Input System polling to avoid modifying InputActions asset for now
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
        }

        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll > 0) ScrollSlot(-1); // Previous
            if (scroll < 0) ScrollSlot(1);  // Next
        }
#else
        // Legacy input fallback
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0) ScrollSlot(-1);
        if (scroll < 0) ScrollSlot(1);
#endif

        // Ability Trigger
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryActivateAbility();
        }
    }



    private Dictionary<GGJ26.Abilities.MaskAbility, float> _nextAbilityUsageTime = new Dictionary<GGJ26.Abilities.MaskAbility, float>();

    private void TryActivateAbility()
    {
        if (_currentMaskData == null || _currentMaskData.ability == null) return;

        var ability = _currentMaskData.ability;

        // Check Cooldown
        if (_nextAbilityUsageTime.TryGetValue(ability, out float nextReadyTime))
        {
            if (Time.time < nextReadyTime)
            {
                Debug.Log($"Ability '{ability.name}' on cooldown. Ready in {nextReadyTime - Time.time:F1}s");
                
                // Feedback
                if (_ui != null) _ui.ShowCooldownFeedback(_currentIndex);
                if (_cameraShakeTarget != null)
                {
                    _cameraShakeTarget.DOShakePosition(_shakeDuration, _shakeIntensity);
                }

                return;
            }
        }

        // Activate
        ability.Activate(gameObject);
        
        // Set Cooldown
        if (ability.cooldown > 0)
        {
            _nextAbilityUsageTime[ability] = Time.time + ability.cooldown;
        }
    }

    private void ScrollSlot(int direction)
    {
        if (_inventory == null || _inventory.MaskCount == 0) return;

        int newIndex = _currentIndex + direction;
        
        // Wrap around valid slots
        if (newIndex < 0) newIndex = _inventory.MaskCount - 1;
        if (newIndex >= _inventory.MaskCount) newIndex = 0;

        SelectSlot(newIndex);
    }

    public void SelectSlot(int index)
    {
        if (_inventory == null) return;

        // Validation
        if (index < 0 || index >= _inventory.MaxSlots) return;

        // If selecting empty slot (and it's not the current one), just ignore or handle as deselect
        if (index >= _inventory.MaskCount)
        {
             Debug.Log($"MaskSelector: Cannot select Slot {index + 1}. Inventory Count: {_inventory.MaskCount}. (Index {index})");
             return;
        }
        
        if (index == _currentIndex && _currentMaskObject != null)
        {
             // Debug.Log($"MaskSelector: Slot {index} is already selected.");
             return;
        }

        _currentIndex = index;
        EquipMask(_inventory.GetMaskAtSlot(index));
    }

    private void EquipMask(MaskData maskData)
    {
        // 1. Destroy current visual
        if (_currentMaskObject != null)
        {
            _currentMaskObject.transform.DOKill(); // Stop valid animations
            Destroy(_currentMaskObject);
            _currentMaskObject = null;
        }

        _currentMaskData = maskData;

        if (maskData == null) return;

        if (_maskRoot == null)
        {
            Debug.LogError("MaskSelector: CRITICAL ERROR - 'Mask Root' is not assigned! Drag the Head bone into the Mask Selector component in Inspector.");
            return;
        }

        if (maskData.prefab == null)
        {
            Debug.LogError($"MaskSelector: Mask '{maskData.maskName}' has no PREFAB assigned in its Data asset.");
            return;
        }

        // 2. Instantiate new visual if data exists
        _currentMaskObject = Instantiate(maskData.prefab, _maskRoot);
            
        // Reset transforms to align with root
        _currentMaskObject.transform.localPosition = Vector3.zero;
        _currentMaskObject.transform.localRotation = Quaternion.identity;
        _currentMaskObject.transform.localScale = Vector3.one;

        // Important: Disable collider/physics on expected mask if it has them
        var rb = _currentMaskObject.GetComponent<Rigidbody>();
        if (rb) Destroy(rb);
        
        var col = _currentMaskObject.GetComponent<Collider>();
        if (col) Destroy(col);
        
        // Should also remove pickup scripts to prevent self-pickup
        var pickup = _currentMaskObject.GetComponent<MaskPickup>();
        if (pickup) Destroy(pickup);

        Debug.Log($"MaskSelector: Equipped '{maskData.maskName}'");

        // --- Animation ---
        if (_useAnimations)
        {
            // Calculate Start Position
            Vector3 startLocalPos = new Vector3(0, 5, 0); // Default fallback if no origin
            if (_animationOrigin != null)
            {
                startLocalPos = _maskRoot.InverseTransformPoint(_animationOrigin.position);
            }

            // Initial State
            _currentMaskObject.transform.localPosition = startLocalPos;
            _currentMaskObject.transform.localScale = Vector3.zero;

            // Animate to local zero (Head position) with Jump
            _currentMaskObject.transform.DOLocalJump(Vector3.zero, _jumpPower, 1, _jumpDuration);
            
            // Animate Scale
            if (_useScaleAnimation)
            {
                _currentMaskObject.transform.DOScale(_targetScale, _scaleDuration);
            }
            else
            {
                 _currentMaskObject.transform.localScale = _targetScale;
            }
        }
        else
        {
             // If animations disabled, ensure correct scale
             _currentMaskObject.transform.localScale = _targetScale;
        }
        
        OnMaskEquipped?.Invoke(_currentIndex);
    }

    private void HandleMaskAdded(MaskData mask, int index)
    {
        if (_autoEquipOnPickup)
        {
            SelectSlot(index);
        }
    }

    private void HandleMaskRemoved(MaskData mask, int index)
    {
        // If we removed the currently selected mask
        if (index == _currentIndex)
        {
            // Clear current visual
            EquipMask(null);
            _currentIndex = -1;
        }
        else if (index < _currentIndex)
        {
            // Shift index if a lower slot was removed
            _currentIndex--;
        }
    }
}

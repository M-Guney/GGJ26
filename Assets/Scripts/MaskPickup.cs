using DG.Tweening;
using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Header("Inventory Data")]
    [Tooltip("The MaskData associated with this pickup.")]
    [SerializeField] private MaskData _maskData;
    
    [Header("Pickup Settings")]
    [Tooltip("If true, the mask will be picked up automatically when player enters trigger. If false, requires attract button press.")]
    [SerializeField] private bool _pickupOnTrigger = false;
    [SerializeField] private CollectableMove _collectableMove; // kept for reference/movement if needed
    
    private float _lastInteractionTime;
    private const float COOLDOWN = 1.0f;

    private void Start()
    {
        // _initialScale not needed if we destroy on pickup
    }

    private void Update()
    {
        // Drop logic removed as it is now handled by Inventory UI/System
    }

    private void OnTriggerStay(Collider other)
    {
        TryPickup(other.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryPickup(collision.gameObject);
    }

    private void TryPickup(GameObject other)
    {
        if (Time.time < _lastInteractionTime + COOLDOWN)
            return;

        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<StarterAssets.StarterAssetsInputs>();
            
            // Check pickup conditions
            bool pickupInput = input != null && input.attract;
            bool shouldPickup = _pickupOnTrigger || pickupInput;
            
            if (shouldPickup)
            {
                AttemptAddToInventory(other);
            }
        }
    }

    private void AttemptAddToInventory(GameObject player)
    {
        if (_maskData == null)
        {
            Debug.LogError($"MaskPickup: No MaskData assigned to {gameObject.name}");
            return;
        }

        var inventory = player.GetComponent<MaskInventory>();
        if (inventory != null)
        {
            // Try adding to inventory
            if (inventory.TryAddMask(_maskData))
            {
                Debug.Log($"MaskPickup: Collected {_maskData.maskName}");
                
                // Optional: Spawn collection effect here
                
                // Destroy the world object
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("MaskPickup: Inventory Full!");
                _lastInteractionTime = Time.time; // Add cooldown so we don't spam logs
            }
        }
        else
        {
            Debug.LogError("MaskPickup: Player has no MaskInventory component!");
        }
    }
}

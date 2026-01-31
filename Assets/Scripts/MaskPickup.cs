using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Tooltip("The Transform on the player where the mask should attach.")]
    [SerializeField] private Transform _maskRoot;
    
    [Header("Pickup Settings")]
    [Tooltip("If true, the mask will be picked up automatically when player enters trigger. If false, requires attract button press.")]
    [SerializeField] private bool _pickupOnTrigger = false;
    
    private bool _isPickedUp = false;
    private float _lastInteractionTime;
    private const float COOLDOWN = 1.0f;
    private StarterAssets.StarterAssetsInputs _currentInput;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        if (_isPickedUp && _currentInput != null)
        {
            if (_currentInput.attract && Time.time > _lastInteractionTime + COOLDOWN)
            {
                Drop();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        TryPickup(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If we are not picked up, and we hit something that isn't the player (like the ground)
        if (!_isPickedUp && !collision.gameObject.CompareTag("Player"))
        {
            // Restore "Floating" state (Non-Kinematic, No Gravity) matches initial state
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.isKinematic = false;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero; // Stop moving
                rb.angularVelocity = Vector3.zero;
            }

            // Turn on trigger
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            
            Debug.Log("Mask landed: Restored 'Floating' Trigger mode.");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        TryPickup(collision.gameObject);
    }

    private void TryPickup(GameObject other)
    {
        if (_isPickedUp)
        {
            Debug.Log("TryPickup: Already picked up, skipping.");
            return;
        }
        
        if (Time.time < _lastInteractionTime + COOLDOWN)
        {
            Debug.Log($"TryPickup: Cooldown active. Time remaining: {(_lastInteractionTime + COOLDOWN - Time.time):F2}s");
            return;
        }

        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<StarterAssets.StarterAssetsInputs>();
            
            if (input == null)
            {
                Debug.LogWarning("Player doesn't have StarterAssetsInputs component!");
                return;
            }
            
            // Check if we should pickup based on settings
            bool shouldPickup = _pickupOnTrigger || input.attract;
            
            Debug.Log($"TryPickup: Player detected. PickupOnTrigger: {_pickupOnTrigger}, Attract: {input.attract}, ShouldPickup: {shouldPickup}");

            if (shouldPickup)
            {
                PickUp(input);
            }
        }
        else
        {
            Debug.Log($"TryPickup: Object '{other.name}' is not tagged as Player. Tag: '{other.tag}'");
        }
    }

    private void PickUp(StarterAssets.StarterAssetsInputs input)
    {
        Debug.Log($"PickUp called. _maskRoot assigned: {_maskRoot != null}");
        
        if (_maskRoot != null)
        {
            Debug.Log($"Attempting to parent mask to: {_maskRoot.name}");
            
            _currentInput = input;
            _isPickedUp = true;
            _lastInteractionTime = Time.time;

            // Handle Collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true; // Turn into trigger while equipped (or disable)
                col.enabled = false;  // Disable to prevent self-collision
            }

            // Handle Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false; // Disable gravity while holding
            }

            // Parent to the Mask-Root
            transform.SetParent(_maskRoot);
            Debug.Log($"Mask parented! Parent is now: {transform.parent?.name ?? "NULL"}");

            // Reset local position and rotation
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Debug.Log("Mask picked up successfully!");
        }
        else
        {
            Debug.LogError("Mask-Root is not assigned in the inspector! Please assign it in the MaskPickup component.");
        }
    }

    private void Drop()
    {
        _isPickedUp = false;
        _currentInput = null;
        _lastInteractionTime = Time.time;

        transform.SetParent(null);
        transform.localScale = _initialScale; // Restore scale in case parent distorted it

        // Handle Collider
        Collider col = GetComponent<Collider>();
        if (col != null) 
        {
            col.enabled = true;
            col.isTrigger = false; // Ensure it's solid to hit the ground
        }

        // Handle Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true; // Ensure gravity is ON so it falls
            rb.WakeUp();
        }

        Debug.Log("Mask dropped!");
    }
}

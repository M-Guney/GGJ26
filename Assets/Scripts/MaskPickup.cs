using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Tooltip("The Transform on the player where the mask should attach.")]
    [SerializeField] private Transform _maskRoot;
    
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
        if (_isPickedUp) return;
        if (Time.time < _lastInteractionTime + COOLDOWN) return;

        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<StarterAssets.StarterAssetsInputs>();
            
            // Debugging re-pickup
            // Debug.Log($"TryPickup: Touched by Player. Attract Input: {input?.attract}");

            if (input != null && input.attract)
            {
                PickUp(input);
            }
        }
    }

    private void PickUp(StarterAssets.StarterAssetsInputs input)
    {
        if (_maskRoot != null)
        {
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

            // Reset local position and rotation
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Debug.Log("Mask picked up!");
        }
        else
        {
            Debug.LogError("Mask-Root is not assigned in the inspector!");
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

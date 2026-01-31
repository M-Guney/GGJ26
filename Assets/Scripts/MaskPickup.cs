using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Tooltip("The Transform on the player where the mask should attach.")]
    [SerializeField] private Transform _maskRoot;
    
    private bool _isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (_maskRoot != null)
        {
            _isPickedUp = true;

            // Disable physics/collision interactions
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Parent to the Mask-Root
            transform.SetParent(_maskRoot);

            // Reset local position and rotation to snap to the root
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Debug.Log("Mask picked up!");
        }
        else
        {
            Debug.LogError("Mask-Root is not assigned in the inspector!");
        }
    }
}

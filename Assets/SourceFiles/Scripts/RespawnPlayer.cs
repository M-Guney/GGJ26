using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class RespawnPlayer : MonoBehaviour
    {
        [Tooltip("The Y position threshold at which the player will respawn.")]
        public float yThreshold = -5f; 

        private Vector3 _startingPosition;

        private Quaternion _startingRotation;

        private CharacterController _characterController;

        private ThirdPersonController _thirdPersonController;
        public AudioClip respawnSound;


        private void Start()
{
    // Save the starting position and rotation
    _startingPosition = transform.position;
    _startingRotation = transform.rotation;

    // Get the CharacterController reference
    _characterController = GetComponent<CharacterController>();
    if (_characterController == null)
    {
        Debug.LogError("CharacterController component is required for RespawnPlayer script!");
    }

    // Get ThirdPersonController reference
    _thirdPersonController = GetComponent<ThirdPersonController>();
    if (_thirdPersonController == null)
    {
        Debug.LogError("ThirdPersonController component is required for RespawnPlayer!");
    }
}

        private void Update()
        {
            // Check if the player's Y position has fallen below the threshold
            if (transform.position.y < yThreshold)
            {
                Respawn();
            }
        }

        private void Respawn()
{
    // Disable the CharacterController so we can manually adjust position
    if (_characterController != null)
    {
        _characterController.enabled = false; // Disable to reset position/rotation correctly
    }

    // Reset the player's position and rotation
    transform.position = _startingPosition;
    transform.rotation = Quaternion.Euler(0f, 90f, 0f); // Reset player Y rotation to 90 degrees

    if (_characterController != null)
    {
        _characterController.enabled = true; // Enable it back after resetting position
    }

    AudioSource.PlayClipAtPoint(respawnSound, transform.position);

}

    }
}

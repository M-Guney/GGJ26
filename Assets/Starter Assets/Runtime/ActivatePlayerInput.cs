using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    /// <summary>
    /// Ensures PlayerInput is activated and actions are enabled
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class ActivatePlayerInput : MonoBehaviour
    {
        private PlayerInput playerInput;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.ActivateInput();
                Debug.Log("PlayerInput activated in Awake!");
            }
        }

        private void Start()
        {
            if (playerInput != null)
            {
                playerInput.ActivateInput();
                
                // Also enable the actions if they exist
                if (playerInput.actions != null)
                {
                    playerInput.actions.Enable();
                    Debug.Log("PlayerInput actions enabled!");
                }
                
                Debug.Log($"PlayerInput status - Active: {playerInput.inputIsActive}, Actions: {playerInput.actions != null}");
            }
        }

        private void OnEnable()
        {
            if (playerInput != null)
            {
                playerInput.ActivateInput();
            }
        }
    }
}


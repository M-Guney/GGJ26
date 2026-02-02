using System.Collections;
using UnityEngine;
using StarterAssets; // Needed for ThirdPersonController

namespace GGJ26.Abilities
{
    [CreateAssetMenu(fileName = "NewBigJumpAbility", menuName = "GGJ26/Abilities/Big Jump")]
    public class Ability_BigJump : MaskAbility
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpMultiplier = 3f;
        [SerializeField] private float duration = 5f;

        public override void Activate(GameObject user)
        {
            PlayEffects(user);

            var controller = user.GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                // We need to run the coroutine on the USER (MonoBehaviour), not this ScriptableObject
                controller.StartCoroutine(JumpRoutine(controller));
            }
            else
            {
                Debug.LogWarning("Ability_BigJump: No ThirdPersonController found on user!");
            }
        }

        private IEnumerator JumpRoutine(ThirdPersonController controller)
        {
            float originalHeight = controller.JumpHeight;
            
            // Apply Boost
            controller.JumpHeight *= jumpMultiplier;
            Debug.Log($"BigJump Activated! Height: {controller.JumpHeight}");

            yield return new WaitForSeconds(duration);

            // Revert
            controller.JumpHeight = originalHeight;
            Debug.Log($"BigJump Ended. Height: {originalHeight}");
        }
    }
}

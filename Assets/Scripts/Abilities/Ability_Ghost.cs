using System.Collections;
using UnityEngine;

namespace GGJ26.Abilities
{
    [CreateAssetMenu(fileName = "NewGhostAbility", menuName = "GGJ26/Abilities/Ghost")]
    public class Ability_Ghost : MaskAbility
    {
        [Header("Ghost Settings")]
        [SerializeField] private float duration = 5f;
        [Tooltip("The tag to switch to (so enemies ignore player).")]
        [SerializeField] private string ghostTag = "Untagged";

        // To store original tag (usually "Player")
        private string _originalTag;

        public override void Activate(GameObject user)
        {
            PlayEffects(user);
            
            // Start the routine on the MonoBehaviour (User)
            // We use a helper component or just the MonoBehaviour itself if available.
            // Since User is likely the Player, it is a MonoBehaviour.
            MonoBehaviour surrogate = user.GetComponent<MonoBehaviour>();
            if (surrogate != null)
            {
                surrogate.StartCoroutine(GhostRoutine(user));
            }
        }

        private IEnumerator GhostRoutine(GameObject user)
        {
            _originalTag = user.tag;
            
            // 1. Become Ghost
            user.tag = ghostTag;
            Debug.Log($"Ghost Mode Activated! Tag changed to '{ghostTag}'. Enemies should ignore you.");

            // Optional: Visual feedback (Semi-transparent)
            // This is complex depending on materials, but we can try a simple keyword toggle or color change if supported.
            // For now, relies on the VFX spawned by base MaskAbility.
            yield return new WaitForSeconds(duration);
 
            // 2. Revert
            user.tag = _originalTag;
            Debug.Log($"Ghost Mode Ended. Tag reverted to '{_originalTag}'.");
        }
    }
}

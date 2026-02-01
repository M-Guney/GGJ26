using UnityEngine;

namespace GGJ26.Abilities
{
    /// <summary>
    /// Base class for all mask abilities.
    /// Create specific abilities by inheriting from this class.
    /// </summary>
    public abstract class MaskAbility : ScriptableObject
    {
        [TextArea] 
        public string description;

        [Tooltip("Time in seconds before this ability can be used again.")]
        [Min(0f)]
        public float cooldown = 0f;

        [Header("Effects")]
        [Tooltip("Sound to play when activated")]
        public AudioClip activationSound;
        
        [Tooltip("Visual effect to spawn when activated")]
        public GameObject activationVFX;

        /// <summary>
        /// Activates the ability.
        /// </summary>
        /// <param name="user">The GameObject using the ability (usually the Player).</param>
        public abstract void Activate(GameObject user);

        protected virtual void PlayEffects(GameObject user)
        {
            if (user == null) return;
            
            // Play Sound
            if (activationSound != null)
            {
                AudioSource.PlayClipAtPoint(activationSound, user.transform.position);
            }

            // Play VFX
            if (activationVFX != null)
            {
                // Spawn at user's feet or center? Center usually better for general abilities
                Instantiate(activationVFX, user.transform.position, Quaternion.identity);
            }
        }
    }
}

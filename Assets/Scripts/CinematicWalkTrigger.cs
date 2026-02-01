using System.Collections;
using UnityEngine;
using StarterAssets;

public class CinematicWalkTrigger : MonoBehaviour
{
    [Header("Cinematic Settings")]
    [Tooltip("The point the player will walk to.")]
    [SerializeField] private Transform targetPoint;
    
    [Tooltip("Walk speed (must check Animator parameters suitable values, typically 2.0).")]
    [SerializeField] private float walkSpeed = 2.0f;
    
    [Tooltip("How fast to rotate towards the target.")]
    [SerializeField] private float turnSpeed = 5.0f;
    
    [Tooltip("If true, control is returned to player after reaching the point.")]
    [SerializeField] private bool returnControlAfter = false;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            _hasTriggered = true;
            StartCoroutine(CinematicWalkRoutine(other.gameObject));
        }
    }

    private IEnumerator CinematicWalkRoutine(GameObject player)
    {
        // 1. Get Components
        var tpc = player.GetComponent<ThirdPersonController>();
        var controller = player.GetComponent<CharacterController>();
        var animator = player.GetComponent<Animator>();
        var inputs = player.GetComponent<StarterAssetsInputs>();

        if (tpc == null || controller == null)
        {
            Debug.LogError("CinematicWalkTrigger: Player missing TPC or CharacterController!");
            yield break;
        }

        // 2. Disable Controls
        tpc.enabled = false;
        if (inputs != null) 
        {
            inputs.move = Vector2.zero; // Clear inputs
            inputs.look = Vector2.zero;
            inputs.cursorLocked = true; // Keep locked ideally
            inputs.cursorInputForLook = false; // Stop camera mouse look (optional)
        }

        // 3. Animation IDs (Using standard StarterAsset IDs)
        int animSpeed = Animator.StringToHash("Speed");
        int animMotionSpeed = Animator.StringToHash("MotionSpeed");

        Debug.Log("Cinematic Walk Started...");

        // 4. Move Loop
        while (Vector3.Distance(player.transform.position, targetPoint.position) > 0.5f)
        {
            // Calculate Direction (Ignore Y for rotation)
            Vector3 direction = (targetPoint.position - player.transform.position);
            direction.y = 0; 
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                // Rotate
                Quaternion targetRot = Quaternion.LookRotation(direction);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRot, Time.deltaTime * turnSpeed);
            }

            // Move (With simple gravity)
            Vector3 move = direction * walkSpeed;
            // Note: CharacterController needs gravity if not on ground, assuming flat ground for simplicity or adding gravity:
            move.y = -9.81f; // Stick to ground

            controller.Move(move * Time.deltaTime);

            // Animate
            if (animator != null)
            {
                animator.SetFloat(animSpeed, walkSpeed);
                animator.SetFloat(animMotionSpeed, 1f);
            }

            yield return null;
        }

        // 5. Arrived
        Debug.Log("Cinematic Walk Reached Destination.");
        
        // Stop Animation
        if (animator != null)
        {
            animator.SetFloat(animSpeed, 0f);
            animator.SetFloat(animMotionSpeed, 1f);
        }

        // 6. Return Control?
        if (returnControlAfter)
        {
            tpc.enabled = true;
            if (inputs != null) inputs.cursorInputForLook = true;
        }
    }
}

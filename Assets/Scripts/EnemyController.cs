using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Reference to the Animator component")]
    [SerializeField] private Animator _animator;
    
    [Tooltip("Name of the speed parameter in the Animator")]
    [SerializeField] private string _speedParameterName = "Speed";
    
    [Tooltip("Multiplier to adjust animation speed relative to movement speed")]
    [SerializeField] private float _animationSpeedMultiplier = 1f;
    
    private NavMeshAgent agent;
    [SerializeField] GameObject player;
    private Transform playerTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTarget = player.transform;
        // Auto-get animator if not assigned
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        
        //GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        //if (playerObj != null) playerTarget = playerObj.transform;
    }

    void Update()
    {
        // Player yok edilirse (null olursa) hata vermemesi için kontrol ediyoruz
        // Check if target exists AND is still tagged "Player" (for Ghost ability support)
        if (playerTarget != null)
        {
            if (playerTarget.CompareTag("Player"))
            {
                agent.isStopped = false; // Ensure moving
                agent.SetDestination(playerTarget.position);
            }
            else
            {
                // Target exists but is hiding (Ghost Mode)
                agent.isStopped = true; // Stop moving
                agent.ResetPath(); // Clear path so it doesn't drift
            }
        }
        
        // Update animation speed based on NavMeshAgent velocity
        UpdateAnimationSpeed();
    }
    
    private void UpdateAnimationSpeed()
    {
        if (_animator == null || agent == null) return;

        float currentSpeed = agent.velocity.magnitude;

        float normalizedSpeed = currentSpeed / agent.speed;
        normalizedSpeed *= _animationSpeedMultiplier;

        _animator.SetFloat(
            _speedParameterName,
            normalizedSpeed,
            0.1f,
            Time.deltaTime
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        // Çarptığı şey Oyuncu mu?
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("OYUNCU YAKALANDI VE YOK OLDU!");
            // Stop chasing
            //playerTarget = null;
            //agent.ResetPath();
            player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        }
    }
}
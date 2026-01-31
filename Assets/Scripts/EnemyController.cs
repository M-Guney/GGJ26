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
    private Transform playerTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Auto-get animator if not assigned
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
    }

    void Update()
    {
        // Player yok edilirse (null olursa) hata vermemesi için kontrol ediyoruz
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
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

            // Oyuncuyu sahneden siler
            Destroy(collision.gameObject);
        }
    }
}
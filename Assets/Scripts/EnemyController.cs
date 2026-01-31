using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
    }

    void OnCollisionEnter(Collision collision)
    {
        // Çarptýðý þey Oyuncu mu?
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("OYUNCU YAKALANDI VE YOK OLDU!");

            // Oyuncuyu sahneden siler
            Destroy(collision.gameObject);
        }
    }
}
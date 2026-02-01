using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Diske (PlayerPrefs) yazmak yerine, RAM'deki static deðiþkene yazýyoruz.
            CheckpointManager.sonCheckpointKonumu = transform.position;
            CheckpointManager.checkpointAlindiMi = true;

            Debug.Log("Checkpoint RAM'e kaydedildi! (Oyun kapanýrsa silinir)");
        }
    }
}
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // STATIC: Bu deðiþkenler sahne deðiþse bile silinmez, ama oyun kapanýnca silinir.
    public static Vector3 sonCheckpointKonumu;
    public static bool checkpointAlindiMi = false;

    [Header("Debug")]
    public Vector3 suankiHedef; // Sadece editörde görmek için

    void Start()
    {
        // Editörde görmek için static veriyi buraya çekiyoruz
        suankiHedef = sonCheckpointKonumu;

        // 1. Eðer daha önce bir checkpoint alýndýysa oraya ýþýnlan
        if (checkpointAlindiMi)
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = sonCheckpointKonumu;

            if (cc != null) cc.enabled = true;

            Debug.Log("Ölümden dönüldü, son checkpoint'e ýþýnlandý: " + sonCheckpointKonumu);
        }
        else
        {
            // Checkpoint yoksa oyunun kendi baþlangýç noktasýnda (0,0,0 veya neresiyse) baþlar.
            Debug.Log("Oyun ilk kez açýldý, baþlangýç noktasýndasýn.");
        }
    }

    // Oyun tamamen kapatýlmadan manuel olarak sýfýrlamak istersen diye
    public static void CheckpointleriSifirla()
    {
        checkpointAlindiMi = false;
        sonCheckpointKonumu = Vector3.zero;
    }
}
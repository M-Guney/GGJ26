using UnityEngine;

public class CollectableMove : MonoBehaviour
{
    [Header("Ayarlar")]
    public float donmeHizi = 50f;
    public float suzulmeHizi = 1.5f;
    public float suzulmeMiktari = 0.3f;

    private Vector3 baslangicPozisyonu;
    public bool animated = true;

    void OnEnable()
    {
        // YÖNTEM: "Ailemde 'CharacterController' bileşeni olan biri var mı?"
        // Yerdeyken yoktur. Kafadayken (mainchar) vardır.
        if (GetComponentInParent<CharacterController>() != null)
        {
            // Varsa, demek ki oyuncunun üstündeyim.
            Debug.Log("✅ OYUNCU TESPİT EDİLDİ! Hareket iptal ediliyor.");
            Destroy(this); // Scripti imha et
            return;
        }

        // Yoksa yerdeki maskedir, çalışmaya devam.
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        if (!animated) return;
        transform.Rotate(Vector3.up * donmeHizi * Time.deltaTime);
        float yeniY = baslangicPozisyonu.y + Mathf.Sin(Time.time * suzulmeHizi) * suzulmeMiktari;
        transform.position = new Vector3(transform.position.x, yeniY, transform.position.z);
    }
}
using UnityEngine;
using StarterAssets; // Eðer Starter Assets kullanýyorsan bunu ekle, yoksa sil.
using System.Collections;

public class PlayerAbilitySystem : MonoBehaviour
{
    [Header("Ayarlar")]
    public int seciliSlot = 1; // 1: Jump, 2: Invisibility, 3: ForceField
    public bool ozelGucAktif = false;

    [Header("Referanslar")]
    public GameObject forceFieldPrefab; // Az önce yaptýðýn kalkan prefab'i
    public Material invisibleMaterial;  // Görünmez olunca karakterin alacaðý materyal (Ghost)
    public Material normalMaterial;     // Karakterin normal materyali
    public SkinnedMeshRenderer characterMesh; // Karakterin Mesh'i (Rengini deðiþtirmek için)

    // Starter Assets ThirdPersonController referansý
    // Eðer farklý bir hareket kodu kullanýyorsan burayý deðiþtirmen gerekebilir.
    private ThirdPersonController controller;
    private float defaultJumpHeight;

    void Start()
    {
        // Karakterin hareket scriptini al (Zýplama gücünü deðiþtirmek için)
        controller = GetComponent<ThirdPersonController>();
        if (controller != null) defaultJumpHeight = controller.JumpHeight;

        // Baþlangýçta normal materyali hafýzaya alalým (eðer elle atamadýysan)
        if (characterMesh != null && normalMaterial == null)
            normalMaterial = characterMesh.material;
    }

    void Update()
    {
        // 1-2-3 Tuþlarý ile Slot Deðiþtirme (Mobilde UI butonlarý ile baðlanmalý)
        if (Input.GetKeyDown(KeyCode.Alpha1)) { seciliSlot = 1; Debug.Log("Güç: Süper Zýplama Seçildi"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { seciliSlot = 2; Debug.Log("Güç: Görünmezlik Seçildi"); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { seciliSlot = 3; Debug.Log("Güç: Koruma Kalkaný Seçildi"); }

        // E Tuþu ile Aktif Etme
        if (Input.GetKeyDown(KeyCode.E) && !ozelGucAktif)
        {
            StartCoroutine(OzelGucuCalistir());
        }
    }

    IEnumerator OzelGucuCalistir()
    {
        ozelGucAktif = true;

        switch (seciliSlot)
        {
            case 1: // SUPER JUMP
                Debug.Log("Süper Zýplama Aktif!");
                if (controller != null) controller.JumpHeight = defaultJumpHeight * 3.5f; // 3.5 kat zýpla

                yield return new WaitForSeconds(1.0f); // 1 saniye sonra eski haline dön

                if (controller != null) controller.JumpHeight = defaultJumpHeight;
                break;

            case 2: // INVISIBILITY (Görünmezlik)
                Debug.Log("Görünmezlik Aktif! (10sn)");

                // 1. Tag'i deðiþtir (Düþmanlar bulamasýn)
                gameObject.tag = "Untagged";

                // 2. Görseli deðiþtir (Hayalet gibi olsun)
                if (characterMesh != null && invisibleMaterial != null)
                    characterMesh.material = invisibleMaterial;

                yield return new WaitForSeconds(10f); // 10 saniye bekle

                // Geri al
                gameObject.tag = "Player";
                if (characterMesh != null) characterMesh.material = normalMaterial;
                Debug.Log("Görünmezlik Bitti.");
                break;

            case 3: // FORCE FIELD (Koruma Kalkaný)
                Debug.Log("Kalkan Aktif! (10sn)");

                // Kalkaný oyuncunun olduðu yerde oluþtur
                GameObject kalkan = Instantiate(forceFieldPrefab, transform.position, Quaternion.identity);

                yield return new WaitForSeconds(10f);

                // Kalkaný yok et
                Destroy(kalkan);
                Debug.Log("Kalkan Bitti.");
                break;
        }

        // Soðuma süresi (Cooldown) istersen buraya ekleyebilirsin
        ozelGucAktif = false;
    }
}
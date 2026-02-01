using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþimi için þart!

public class MainMenuManager : MonoBehaviour
{
    // OYNA BUTONU ÝÇÝN
    public void OyunuBaslat()
    {
        // Build Settings listesindeki 1 numaralý sahneyi açar.
        // Genelde: 0 -> MainMenu, 1 -> Oyun Sahnesi olur.
        SceneManager.LoadScene(1);
    }

    // ÇIKIÞ BUTONU ÝÇÝN
    public void OyundanCikis()
    {
        Debug.Log("ÇIKIÞ butonuna basýldý! (Oyun Build alýndýðýnda kapanýr)");
        Application.Quit();
    }
}
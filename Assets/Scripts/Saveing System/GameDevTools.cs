using UnityEngine;
using UnityEngine.SceneManagement; // Sahneyi yeniden yüklemek için þart

public class GameDevTools : MonoBehaviour
{
    void Update()
    {
        // Klavyeden 'R' tuþuna basýlýnca çalýþýr
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R'ye basýldý! Sahne baþa sarýlýyor...");

            // Þu an açýk olan sahneyi (Leveli) kapatýp aynýsýný baþtan açar
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
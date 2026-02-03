using UnityEngine;
using UnityEngine.SceneManagement; // Sahneyi yeniden yüklemek için þart
using UnityEngine.InputSystem;

public class GameDevTools : MonoBehaviour
{
    void Update()
    {
        // Klavyeden 'R' tuþuna basýlýnca çalýþýr
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R'ye basýldý! Sahne baþa sarýlýyor...");

            // Þu an açýk olan sahneyi (Leveli) kapatýp aynýsýný baþtan açar
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
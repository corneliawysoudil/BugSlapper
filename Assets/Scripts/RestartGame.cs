using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;



public class RestartGame : MonoBehaviour
{
    public TextMeshPro scoreText;

    // Diese Methode wird aufgerufen, wenn der Button geklickt wird
    public void OnRestartButtonClicked()
    {
        // Szene wechseln
        SceneManager.LoadScene("PlayPassthrough");
    }


}

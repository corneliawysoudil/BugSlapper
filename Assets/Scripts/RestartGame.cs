using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class RestartGame : MonoBehaviour
{
    public TextMeshPro scoreText;

    public void Start()
    {
        // Setze den Text des Score-UI-Elements
        scoreText.text = ScoreKeeper.score.ToString();
    }

    // Diese Methode wird aufgerufen, wenn der Button geklickt wird
    public void OnRestartButtonClicked()
    {
        // Szene wechseln
        SceneManager.LoadScene("PlayPassthrough");
        ScoreKeeper.score = 0; // Setze den Score zurück
    }


}

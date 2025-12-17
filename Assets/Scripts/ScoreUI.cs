using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public TextMeshPro scoreText; // TextMesh Pro Komponente
    void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + ScoreKeeper.score;
        }
    }
}

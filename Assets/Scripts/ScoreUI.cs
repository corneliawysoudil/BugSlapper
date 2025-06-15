using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public BugSpawner spawner;
    public TextMeshProUGUI scoreText; // TextMesh Pro Komponente

    void Update()
    {
        scoreText.text = "Score: " + spawner.GetScore();
    }
}

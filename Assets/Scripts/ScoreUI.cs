using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }
    public int GameScore;
    public BugSpawner spawner;
    public TextMeshPro scoreText; // TextMesh Pro Komponente



    private void Awake()
    {
        // Ensure this object persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        scoreText.text = "Score: " + spawner.GetScore();
        GameScore = spawner.GetScore();
    }
}

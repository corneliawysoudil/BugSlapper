using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3; // Maximale Anzahl an Leben
    private int currentLives; // Aktuelle Anzahl an Leben

    public Image[] heartImages; // Array der Herz-Sprites (UI-Images)
    public Sprite fullHeartSprite; // Sprite für ein volles Herz
    public Sprite emptyHeartSprite; // Sprite für ein leeres Herz

    public AudioClip hitSound; // AudioClip für den Treffer-Sound
    public AudioSource audioSource; // AudioSource zum Abspielen des Sounds


    void Start()
    {
        // Initialisiere die Lebenspunkte
        currentLives = maxLives;

        // Setze alle Herzen auf "voll"
        UpdateHeartUI();
    }

    private void Awake()
    {
        currentLives = maxLives; // Setze die Lebenspunkte auf die maximale Anzahl
        UpdateHeartUI(); // Aktualisiere die Herz-Sprites
    }

    void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob das kollidierende Objekt den Tag "Bug" hat

        if (other.CompareTag("Bug"))
        {
            ReduceLife();
            PlayHitSound(); // Spiele den Treffer-Sound ab
            Destroy(other.gameObject); // Zerstöre das kollidierende Objekt
            ScoreKeeper.score--;
        }
    }

    void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound); // Spiele den Treffer-Sound ab
    }
    

    void ReduceLife()
    {
        if (currentLives > 0)
        {
            currentLives--; // Reduziere die Lebenspunkte
            UpdateHeartUI(); // Aktualisiere die Herz-Sprites

            Debug.Log($"Player lives: {currentLives}");
        }

        // Überprüfe, ob der Spieler keine Leben mehr hat
        if (currentLives <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("EndScene"); // Lade die Neustart-Szene
        }
    }

    void UpdateHeartUI()
    {
        // Aktualisiere die Herz-Sprites basierend auf den aktuellen Lebenspunkten
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].sprite = fullHeartSprite; // Volles Herz
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite; // Leeres Herz
            }
        }
    }
}

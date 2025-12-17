using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3; // Maximale Anzahl an Leben
    private int currentLives; // Aktuelle Anzahl an Leben

    public Image[] heartImages; // Array der Herz-Sprites (UI-Images)
    public Sprite fullHeartSprite; // Sprite fr ein volles Herz
    public Sprite emptyHeartSprite; // Sprite fr ein leeres Herz

    public AudioClip hitSound; // AudioClip fr den Treffer-Sound
    public AudioSource audioSource; // AudioSource zum Abspielen des Sounds


    private void Awake()
    {
        currentLives = maxLives; // Setze die Lebenspunkte auf die maximale Anzahl
        UpdateHeartUI(); // Aktualisiere die Herz-Sprites
    }

    void OnTriggerEnter(Collider other)
    {
        //berprfe, ob das kollidierende Objekt den Tag "Bug" hat

        if (other.CompareTag("Bug"))
        {
            // Mark bug as killed by player hit to prevent death handlers from incrementing score
            BugDeathHandler bugHandler = other.GetComponent<BugDeathHandler>();
            if (bugHandler != null)
            {
                bugHandler.MarkAsKilledByPlayerHit();
            }
            
            WaveBugDeathHandler waveHandler = other.GetComponent<WaveBugDeathHandler>();
            if (waveHandler != null)
            {
                waveHandler.MarkAsKilledByPlayerHit();
            }

            ReduceLife();
            PlayHitSound(); // Spiele den Treffer-Sound ab
            
            // Decrement score when bug hits player (prevent negative scores)
            /*
            if (ScoreKeeper.score > 0)
            {
                ScoreKeeper.score--;
            }
            */
            
            Destroy(other.gameObject); // Zerstre das kollidierende Objekt
        }
    }

    void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound); // Spiele den Treffer-Sound ab
        }
    }
    

    void ReduceLife()
    {
        if (currentLives > 0)
        {
            currentLives--; // Reduziere die Lebenspunkte
            UpdateHeartUI(); // Aktualisiere die Herz-Sprites

            Debug.Log($"Player lives: {currentLives}");
        }

        //berprfe, ob der Spieler keine Leben mehr hat
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

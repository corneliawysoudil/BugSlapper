using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3; // Maximale Anzahl an Leben
    private int currentLives; // Aktuelle Anzahl an Leben

    [Header("Swatter Cleanup")]
    public float bugCleanupRadius = 20f; // Max distance before bugs are killed
    public float bugCleanupInterval = 1f; // How often to check (seconds)
    private float bugCleanupTimer = 0f;

    [Header("Damage Handling")]
    public float hitCooldown = 0.2f; // Prevent multiple hits from the same overlap
    private float lastHitTime = -999f;

    [Header("Damage Effect")]
    public Image damageOverlay; // Full-screen red flash overlay (optional)
    public Color damageColor = new Color(1f, 0f, 0f, 0.5f); // Red with 50% opacity
    public float damageFlashDuration = 0.3f; // How long the flash lasts
    private float damageFlashTimer = 0f;

    public Image[] heartImages; // Array der Herz-Sprites (UI-Images)
    public Sprite fullHeartSprite; // Sprite f�r ein volles Herz
    public Sprite emptyHeartSprite; // Sprite f�r ein leeres Herz

    public AudioClip hitSound; // AudioClip f�r den Treffer-Sound
    public AudioSource audioSource; // AudioSource zum Abspielen des Sounds


    void Start()
    {
        // Initialisiere die Lebenspunkte
        currentLives = maxLives;

        // Setze alle Herzen auf "voll"
        UpdateHeartUI();
        
        // Initialize damage overlay to transparent
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
            
            // Auto-assign camera if needed (for Screen Space - Camera or World Space)
            Canvas parentCanvas = damageOverlay.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera || 
                    parentCanvas.renderMode == RenderMode.WorldSpace)
                {
                    if (parentCanvas.worldCamera == null)
                    {
                        // Try to find the main camera or VR head camera
                        Camera mainCam = Camera.main;
                        if (mainCam == null)
                        {
                            // Try to find VR head camera
                            GameObject headCam = GameObject.FindGameObjectWithTag("MainCamera");
                            if (headCam != null)
                                mainCam = headCam.GetComponent<Camera>();
                        }
                        
                        if (mainCam != null)
                        {
                            parentCanvas.worldCamera = mainCam;
                            Debug.Log($"Auto-assigned camera to damage overlay canvas: {mainCam.name}");
                        }
                        else
                        {
                            Debug.LogWarning("Damage overlay canvas needs a camera but none found! Please assign the VR head camera manually in the Canvas component.");
                        }
                    }
                }
                
                Debug.Log($"Damage overlay initialized: {damageOverlay.name}, Canvas: {parentCanvas.name}, Render Mode: {parentCanvas.renderMode}, Camera: {parentCanvas.worldCamera?.name ?? "None (Overlay mode)"}");
            }
        }
        else
        {
            Debug.LogWarning("Damage overlay is NULL in Start! Make sure to assign it in the inspector.");
        }
    }

    private void Awake()
    {
        currentLives = maxLives; // Setze die Lebenspunkte auf die maximale Anzahl
        UpdateHeartUI(); // Aktualisiere die Herz-Sprites
    }

    void Update()
    {
        bugCleanupTimer += Time.deltaTime;
        if (bugCleanupTimer >= bugCleanupInterval)
        {
            bugCleanupTimer = 0f;
            CleanupFarSwatters();
        }

        // Update damage flash effect
        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.deltaTime;
            if (damageOverlay != null)
            {
                float alpha = Mathf.Clamp01(damageFlashTimer / damageFlashDuration);
                Color currentColor = damageColor;
                currentColor.a = alpha * damageColor.a;
                damageOverlay.color = currentColor;
            }
        }
        else if (damageOverlay != null && damageOverlay.color.a > 0f)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �berpr�fe, ob das kollidierende Objekt den Tag "Bug" hat

        if (other.CompareTag("Bug"))
        {
            // Simple cooldown to avoid double-damage from multiple colliders/frames
            if (Time.time - lastHitTime < hitCooldown)
                return;
            lastHitTime = Time.time;

            Debug.Log($"Bug hit detected! Collider: {other.gameObject.name}, Tag: {other.tag}");
            
            // Disable the collider immediately to prevent multiple hits
            other.enabled = false;
            
            // Also remove the collider from physics to ensure it exits the trigger
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.isKinematic = true;
                other.attachedRigidbody.linearVelocity = Vector3.zero;
                other.attachedRigidbody.angularVelocity = Vector3.zero;
            }
            
            ReduceLife();
            PlayHitSound(); // Spiele den Treffer-Sound ab
            
            // Find the root bug object to destroy - try multiple methods
            GameObject bugToDestroy = null;
            
            // Method 1: Check if the collider's object itself has the Bug tag
            if (other.gameObject.CompareTag("Bug"))
            {
                bugToDestroy = other.gameObject;
            }
            
            // Method 2: Walk up the parent chain looking for Bug tag
            if (bugToDestroy == null)
            {
                Transform current = other.transform.parent;
                while (current != null)
                {
                    if (current.CompareTag("Bug"))
                    {
                        bugToDestroy = current.gameObject;
                        break;
                    }
                    current = current.parent;
                }
            }
            
            // Method 3: Use root transform
            if (bugToDestroy == null)
            {
                bugToDestroy = other.transform.root.gameObject;
            }
            
            // Method 4: Last resort - use the collider's object
            if (bugToDestroy == null)
            {
                bugToDestroy = other.gameObject;
            }
            
            Debug.Log($"Root object found: {bugToDestroy.name}, Hierarchy: {GetFullPath(bugToDestroy.transform)}");
            
            // Destroy the bug object - more aggressive for builds
            if (bugToDestroy != null)
            {
                Debug.Log($"Calling Destroy on: {bugToDestroy.name}");
                
                // Immediately disable all colliders to prevent further triggers
                Collider[] allColliders = bugToDestroy.GetComponentsInChildren<Collider>();
                foreach (Collider col in allColliders)
                {
                    if (col != null)
                        col.enabled = false;
                }
                
                // Disable all rigidbodies to stop physics
                Rigidbody[] allRigidbodies = bugToDestroy.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in allRigidbodies)
                {
                    if (rb != null)
                        rb.isKinematic = true;
                }
                
                // Disable all scripts to stop behavior
                MonoBehaviour[] allScripts = bugToDestroy.GetComponentsInChildren<MonoBehaviour>();
                foreach (MonoBehaviour script in allScripts)
                {
                    if (script != null && script != this)
                        script.enabled = false;
                }
                
                // Move it far away immediately to prevent visual/audio issues
                bugToDestroy.transform.position = new Vector3(10000, 10000, 10000);
                
                // Disable the GameObject
                bugToDestroy.SetActive(false);
                
                // Now destroy it
                Destroy(bugToDestroy);
                
                // Backup: if still exists after a frame, try again
                StartCoroutine(ForceDestroyIfStillExists(bugToDestroy));
            }
            
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
            TriggerDamageEffect(); // Show damage visual effect

            Debug.Log($"Player lives: {currentLives}");
        }

        // �berpr�fe, ob der Spieler keine Leben mehr hat
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

    void TriggerDamageEffect()
    {
        // Start the damage flash effect
        damageFlashTimer = damageFlashDuration;
        if (damageOverlay != null)
        {
            damageOverlay.color = damageColor;
            Debug.Log($"Damage effect triggered! Overlay color set to: {damageColor}");
        }
        else
        {
            Debug.LogWarning("Damage overlay is not assigned! Please assign a UI Image to the damageOverlay field.");
        }
    }

    // Kill swatters that wandered too far from the player (prevents stragglers)
    void CleanupFarSwatters()
    {
        CleanupByTag("Swatter");
    }

    void CleanupByTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist > bugCleanupRadius)
            {
                Destroy(obj);
            }
        }
    }

    IEnumerator ForceDestroyIfStillExists(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        if (obj != null)
        {
            Debug.LogWarning($"Bug {obj.name} still exists after Destroy call! Attempting force destruction.");
            Debug.LogWarning($"Object active: {obj.activeSelf}, activeInHierarchy: {obj.activeInHierarchy}");
            
            // Move even further away
            obj.transform.position = new Vector3(20000, 20000, 20000);
            
            // Disable all components
            MonoBehaviour[] components = obj.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                if (comp != null)
                    comp.enabled = false;
            }
            
            // Disable all colliders
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                if (col != null)
                    col.enabled = false;
            }
            
            // Disable and destroy
            obj.SetActive(false);
            Destroy(obj);
            
            // Final check after another frame
            yield return new WaitForEndOfFrame();
            if (obj != null)
            {
                Debug.LogError($"CRITICAL: Bug {obj.name} STILL EXISTS! This should not happen. Removing from scene.");
                // Last resort - remove from hierarchy
                obj.transform.SetParent(null);
                obj.SetActive(false);
            }
        }
    }

    string GetFullPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}

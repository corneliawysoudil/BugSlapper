using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public GameObject[] bugPrefabs;      // Array mit möglichen Bug-Prefabs
    public float startSpawnInterval = 3f; // Start-Intervall in Sekunden
    public float minSpawnInterval = 0.5f; // Minimaler Abstand zwischen Spawns
    public float difficultyIncreaseRate = 0.05f; // Wie schnell das Intervall sinkt

    // New: speed and distance ramp settings
    public float speedIncreaseRate = 0.05f; // multiplier increase per second (1.0 + time * rate)
    public float maxDifficultyMultiplier = 3f; // cap for multiplier
    public float initialSpawnDistance = 5f; // base distance to player
    public float distanceDecreaseRate = 0.02f; // how much spawn distance shrinks per second
    public float minSpawnDistance = 3f; // don't spawn closer than this

    // Global difficulty multiplier readable by BugAI (updated each frame)
    public static float difficultyMultiplier = 1f;

    private float currentSpawnInterval;
    private float timeSinceLastSpawn;
    private float timeSurvived;

    public GameObject headObject;
    Vector3 spawnPosition;

    public int bugsKilled; // Zähler für getötete Bugs

    void Start()
    {
        currentSpawnInterval = startSpawnInterval;
        timeSinceLastSpawn = 0f;
        timeSurvived = 0f;
        bugsKilled = 0;
        difficultyMultiplier = 1f;
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;
        timeSinceLastSpawn += Time.deltaTime;

        // Intervall verringern, je länger der Spieler überlebt
        currentSpawnInterval = Mathf.Max(
            startSpawnInterval - timeSurvived * difficultyIncreaseRate,
            minSpawnInterval
        );

        // Update global difficulty multiplier for BugAI speed scaling
        difficultyMultiplier = Mathf.Min(1f + timeSurvived * speedIncreaseRate, maxDifficultyMultiplier);

        if (timeSinceLastSpawn >= currentSpawnInterval)
        {
            SpawnRandomBug();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnRandomBug()
    {
        if (bugPrefabs.Length == 0 || headObject == null)
            return;

        // Compute a random horizontal (XZ) direction and ensure it's normalized.
        Vector2 randomCircle = Random.insideUnitCircle;
        if (randomCircle.sqrMagnitude < 0.0001f)
        {
            // Fallback to a random angle if the vector is too small
            float angle = Random.Range(0f, Mathf.PI * 2f);
            randomCircle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        else
        {
            randomCircle.Normalize();
        }

        // Compute current spawn distance (allows bugs to come a little closer over time)
        float currentSpawnDistance = Mathf.Max(initialSpawnDistance - timeSurvived * distanceDecreaseRate, minSpawnDistance);

        // Build horizontal offset and apply exact spawnDistance in XZ plane
        Vector3 horizontalOffset = new Vector3(randomCircle.x, 0f, randomCircle.y) * currentSpawnDistance;

        // Place spawn position at the player's XZ position plus offset.
        // Keep Y at 0 (ground). If you want the bug at the player's height, use headObject.transform.position.y.
        Vector3 headPos = headObject.transform.position;
        spawnPosition = new Vector3(headPos.x + horizontalOffset.x, 0f, headPos.z + horizontalOffset.z);

        int bugIndex = Random.Range(0, bugPrefabs.Length);

        GameObject bug = Instantiate(
            bugPrefabs[bugIndex],
            spawnPosition,
            Quaternion.identity
        );

        // BugDeathHandler-Komponente hinzufügen und Spawner referenzieren
        BugDeathHandler handler = bug.AddComponent<BugDeathHandler>();
        handler.spawner = this;
    }

    // Diese Methode kann von BugDeathHandler aufgerufen werden
    public void OnBugKilled()
    {
        bugsKilled++;
        ScoreKeeper.score++; // Increment score instead of overwriting
        // Debug.Log("Bugs getötet: " + bugsKilled);
    }
    public int GetScore()
    {
        // Beispiel: 1 Punkt pro getötetem Bug, kann angepasst werden
        return bugsKilled;
    }
}

// Hilfsskript für Bugs
public class BugDeathHandler : MonoBehaviour
{
    [HideInInspector]
    public BugSpawner spawner;
    private bool wasKilledByPlayerHit = false; // Flag to prevent score increment when bug hits player

    // Call this method when bug hits player (prevents score increment)
    public void MarkAsKilledByPlayerHit()
    {
        wasKilledByPlayerHit = true;
    }

    private void OnDestroy()
    {
        // Only increment score if bug was killed by swatter, not if it hit the player
        if (spawner != null && !wasKilledByPlayerHit)
        {
            spawner.OnBugKilled();
        }
    }
}

using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;      // Array mit m�glichen Spawnpunkten
    public GameObject[] bugPrefabs;      // Array mit m�glichen Bug-Prefabs
    public float startSpawnInterval = 3f; // Start-Intervall in Sekunden
    public float minSpawnInterval = 0.5f; // Minimaler Abstand zwischen Spawns
    public float difficultyIncreaseRate = 0.05f; // Wie schnell das Intervall sinkt

    private float currentSpawnInterval;
    private float timeSinceLastSpawn;
    private float timeSurvived;

    public int bugsKilled { get; private set; } // Z�hler f�r get�tete Bugs

    void Start()
    {
        currentSpawnInterval = startSpawnInterval;
        timeSinceLastSpawn = 0f;
        timeSurvived = 0f;
        bugsKilled = 0;
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;
        timeSinceLastSpawn += Time.deltaTime;

        // Intervall verringern, je l�nger der Spieler �berlebt
        currentSpawnInterval = Mathf.Max(
            startSpawnInterval - timeSurvived * difficultyIncreaseRate,
            minSpawnInterval
        );

        if (timeSinceLastSpawn >= currentSpawnInterval)
        {
            SpawnRandomBug();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnRandomBug()
    {
        if (spawnPoints.Length == 0 || bugPrefabs.Length == 0)
            return;

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int bugIndex = Random.Range(0, bugPrefabs.Length);

        GameObject bug = Instantiate(
            bugPrefabs[bugIndex],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );

        // BugDeathHandler-Komponente hinzuf�gen und Spawner referenzieren
        BugDeathHandler handler = bug.AddComponent<BugDeathHandler>();
        handler.spawner = this;
    }

    // Diese Methode kann von BugDeathHandler aufgerufen werden
    public void OnBugKilled()
    {
        bugsKilled++;
        // Debug.Log("Bugs get�tet: " + bugsKilled);
    }
    public int GetScore()
    {
        // Beispiel: 1 Punkt pro get�tetem Bug, kann angepasst werden
        return bugsKilled;
    }
}

// Hilfsskript f�r Bugs
public class BugDeathHandler : MonoBehaviour
{
    [HideInInspector]
    public BugSpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnBugKilled();
        }
    }
}

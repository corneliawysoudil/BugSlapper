using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public GameObject[] bugPrefabs;      // Array mit m�glichen Bug-Prefabs
    public float startSpawnInterval = 3f; // Start-Intervall in Sekunden
    public float minSpawnInterval = 0.5f; // Minimaler Abstand zwischen Spawns
    public float difficultyIncreaseRate = 0.05f; // Wie schnell das Intervall sinkt
    public float spawnDistance = 5f; // Abstand zum Spieler, an dem Bugs spawnen

    private float currentSpawnInterval;
    private float timeSinceLastSpawn;
    private float timeSurvived;

    public GameObject headObject;
    Vector3 spawnPosition;

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
        if (bugPrefabs.Length == 0)
            return;

        // Berechne die Spawn-Position in einem zuf�lligen Richtung um den Spieler
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        spawnPosition = headObject.transform.position + randomDirection * spawnDistance;
        spawnPosition.y = 0; // Setze die Y-Position auf 0, falls n�tig

        int bugIndex = Random.Range(0, bugPrefabs.Length);

        GameObject bug = Instantiate(
            bugPrefabs[bugIndex],
            spawnPosition,
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

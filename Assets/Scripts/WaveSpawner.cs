using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int bugCount;
        public float spawnRate; // Bugs pro Sekunde
    }

    public GameObject headObject;
    Vector3 spawnCenter;

    public Transform[] spawnPoints; // 8 Spawnpunkte im Inspector zuweisen
    public List<GameObject> bugPrefabs; // Liste von Bug-Prefabs
    public List<Wave> waves = new List<Wave>();

    private int currentWave = 0;
    private bool spawning = false;
    public int bugsKilled = 0; // Zähler für getötete Bugs

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        if (headObject != null)
        {
            spawnCenter = headObject.transform.position;
            transform.position = new Vector3(spawnCenter.x, 0, spawnCenter.z);
        }
    }

    IEnumerator SpawnWave()
    {
        while (currentWave < waves.Count)
        {
            spawning = true;
            Wave wave = waves[currentWave];

            for (int i = 0; i < wave.bugCount; i++)
            {
                SpawnBug();
                yield return new WaitForSeconds(1f / wave.spawnRate);
            }

            spawning = false;
            currentWave++;

            // Warte, bis alle Bugs besiegt sind, bevor die n�chste Welle startet
            while (GameObject.FindGameObjectsWithTag("Bug").Length > 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2f); // Kurze Pause zwischen den Wellen
        }
    }

    void SpawnBug()
    {
        if (bugPrefabs == null || bugPrefabs.Count == 0)
            return;
        
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int prefabIndex = Random.Range(0, bugPrefabs.Count);
        GameObject bug = Instantiate(
            bugPrefabs[prefabIndex],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );
        bug.tag = "Bug";

        // WaveBugDeathHandler-Komponente hinzufügen und Spawner referenzieren
        WaveBugDeathHandler handler = bug.AddComponent<WaveBugDeathHandler>();
        handler.spawner = this;
    }

    // Diese Methode kann von WaveBugDeathHandler aufgerufen werden
    public void OnBugKilled()
    {
        bugsKilled++;
        ScoreKeeper.score++; // Increment score instead of overwriting
    }
}

// Hilfsskript für Bugs spawned by WaveSpawner
public class WaveBugDeathHandler : MonoBehaviour
{
    [HideInInspector]
    public WaveSpawner spawner;
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

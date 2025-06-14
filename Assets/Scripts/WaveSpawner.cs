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

    public Transform[] spawnPoints; // 8 Spawnpunkte im Inspector zuweisen
    public List<GameObject> bugPrefabs; // Liste von Bug-Prefabs
    public List<Wave> waves = new List<Wave>();

    private int currentWave = 0;
    private bool spawning = false;

    void Start()
    {
        StartCoroutine(SpawnWave());
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

            // Warte, bis alle Bugs besiegt sind, bevor die nächste Welle startet
            while (GameObject.FindGameObjectsWithTag("Bug").Length > 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2f); // Kurze Pause zwischen den Wellen
        }
    }

    void SpawnBug()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int prefabIndex = Random.Range(0, bugPrefabs.Count);
        GameObject bug = Instantiate(
            bugPrefabs[prefabIndex],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );
        bug.tag = "Bug";
    }
}

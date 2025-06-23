using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform player; // Referenz auf den Spieler (z. B. die VR-Kamera)

    void Update()
    {
        if (player != null)
        {
            // Die Anzeigetafel soll immer zum Spieler schauen
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Optional: Ignoriere die Y-Achse, damit die Tafel nicht kippt
            transform.rotation = Quaternion.LookRotation(-direction); // Negiere die Richtung, um die Rückseite zu zeigen
        }
    }
}

using UnityEngine;

public class CanvasFollowPlayer : MonoBehaviour
{
    public Transform playerCamera; // Die Kamera des Spielers
    public float distanceFromPlayer = 2.0f; // Abstand der Canvas von der Kamera

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player Camera ist nicht zugewiesen!");
            return;
        }

        // Positioniere die Canvas vor der Kamera
        Vector3 forwardDirection = playerCamera.forward; // Blickrichtung der Kamera
        transform.position = playerCamera.position + forwardDirection * distanceFromPlayer;

        // Richte die Canvas zur Kamera aus
        transform.rotation = Quaternion.LookRotation(forwardDirection);
    }
}
using UnityEngine;

public class ScoreViewPort : MonoBehaviour
{
    public Transform playerCam; // Die Kamera des Spielers
    public float distanceFromCam = 1.0f; // Abstand der Canvas von der Kamera
    public Vector2 screenOffset = new Vector2(0.9f, 0.1f); // Offset für die rechte untere Ecke (0-1 Werte für Bildschirmverhältnisse)

    void Update()
    {
        if (playerCam == null)
        {
            Debug.LogWarning("Player Camera ist nicht zugewiesen!");
            return;
        }

        // Positioniere die Canvas in der rechten unteren Ecke des Bildschirms
        Vector3 screenPoint = new Vector3(Screen.width * screenOffset.x, Screen.height * screenOffset.y, distanceFromCam);
        Vector3 worldPosition = playerCam.GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
        transform.position = worldPosition;

        // Richte die Canvas zur Kamera aus
        transform.rotation = Quaternion.LookRotation(playerCam.forward);
    }
}

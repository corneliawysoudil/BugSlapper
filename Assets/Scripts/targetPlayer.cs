using UnityEngine;

public class BugAI : MonoBehaviour
{
    public Transform target; // Ziel, z.B. der Spieler
    public float flySpeed = 5f;
    public float circleRadius = 3f;
    public float circleSpeed = 2f;
    public float attackDistance = 1.5f;
    public float attackSpeed = 8f;
    public float circleDuration = 2f; // Wie lange soll gekreist werden (in Sekunden)
    public float minCircleFraction = 0.3f; // Mindestens ein Teilkreis (0.0 - 1.0)

    private float circleTimer = 0f;
    private bool isAttacking = false;
    private Vector3 circleOffset;
    private float circleAngle;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        // Zufälliger Startwinkel für die Kreisbewegung
        circleAngle = Random.Range(0f, 360f);
        // Zufällige Richtung (Uhrzeigersinn oder Gegenuhrzeigersinn)
        circleSpeed *= Random.value > 0.5f ? 1 : -1;
        // Zufällige Kreisdauer (voller Kreis oder Teilkreis)
        circleDuration *= Random.Range(minCircleFraction, 1f);
    }

    void Update()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                return; // Kein Ziel gefunden, Rest überspringen
        }

        if (!isAttacking)
        {
            // Kreisbewegung um das Ziel
            circleTimer += Time.deltaTime;
            circleAngle += circleSpeed * Time.deltaTime * 360f / circleDuration;
            float rad = circleAngle * Mathf.Deg2Rad;
            circleOffset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * circleRadius;
            Vector3 desiredPosition = target.position + circleOffset;

            // Bewege dich zur aktuellen Kreisposition
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, flySpeed * Time.deltaTime);

            // Nach Ablauf der Kreiszeit oder wenn nah genug, Angriff starten
            if (circleTimer >= circleDuration || Vector3.Distance(transform.position, target.position) < attackDistance * 2)
            {
                isAttacking = true;
            }
        }
        else
        {
            // Direkter Angriff auf das Ziel
            transform.position = Vector3.MoveTowards(transform.position, target.position, attackSpeed * Time.deltaTime);

            // Optional: Nach dem Angriff zurücksetzen oder zerstören
            if (Vector3.Distance(transform.position, target.position) < attackDistance)
            {
                // Hier kann z.B. Schaden zugefügt oder das Objekt zerstört werden
                // Destroy(gameObject);
            }
        }

        // Optional: Blickrichtung anpassen
        Vector3 lookDir = (target.position - transform.position).normalized;
        lookDir.y = 0; // Nur auf der XZ-Ebene drehen
        if (lookDir != Vector3.zero)
            transform.forward = lookDir;
    }
}

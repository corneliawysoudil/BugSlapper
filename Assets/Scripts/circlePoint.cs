using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;



public class surroundFlight : MonoBehaviour
{
    public GameObject headObject;
    public float radius;
    public float flyingSpeed = 1f;
    public Vector3 centerPoint = new Vector3(0,0,0); // The point around which the bugs will fly
    private float angle;

    void Start()
    {
        // Set a random starting angle and height
        angle = Random.Range(0f, 360f);
        transform.position = new Vector3(centerPoint.x, Random.Range(0, 3f), centerPoint.z);
        radius = Random.Range(2f, 5f);
    }

    void Update()
    {
        // Calculate the new position based on the angle
        angle += flyingSpeed * Time.deltaTime;
        if (angle >= 360f) angle -= 360f; // Keep angle within 0-360 degrees

        float x = centerPoint.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = centerPoint.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // Update the position of the GameObject
        transform.position = new Vector3(x, transform.position.y, z);
        transform.rotation = Quaternion.LookRotation(centerPoint - transform.position, Vector3.up) * Quaternion.Euler(0, 90, 0);
    }
}

 
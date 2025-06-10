using UnityEngine;
using UnityEngine.XR;



public class flyingCircle : MonoBehaviour
{

    public GameObject headObject;
    Vector3 headPosition;
    public float radius = 5f;
    public float flyingSpeed = 1f;

    private float angle = 0f;

    void Update()
    {
        headPosition = headObject.transform.position;

        // Calculate the new position based on the angle
        angle += flyingSpeed * Time.deltaTime;
        if (angle >= 360f) angle -= 360f; // Keep angle within 0-360 degrees

        float x = headPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = headPosition.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // Update the position of the GameObject
        transform.position = new Vector3(x, headPosition.y, z);
        transform.rotation = Quaternion.LookRotation(headPosition - transform.position, Vector3.up) * Quaternion.Euler(0, 90, 0);

        //Debug.Log("Update:" + transform.position);
    }
}

 
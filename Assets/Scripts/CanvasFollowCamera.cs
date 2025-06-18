using UnityEngine;

public class CanvasFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offset = new Vector3(0, -0.2f, 1.0f);

    void LateUpdate()
    {
        transform.position = cameraTransform.position + cameraTransform.TransformDirection(offset);
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
using UnityEngine;
using Autohand; // Make sure this is in your project

/// <summary>
/// Handles automatic recentering of the game world when the user holds the Meta button.
/// The root player rig moves so the player's head lands at World Zero (0, 0, 0).
/// </summary>
public class RecenterOnPlayer : MonoBehaviour
{
    [Header("Assign References")]
    [Tooltip("Assign the camera object inside the AutoHandPlayer rig (e.g., Main Camera/CenterEyeAnchor)")]
    public Transform headTransform;

    [Tooltip("Assign the Rigidbody component attached to the root AutoHandPlayer object")]
    public Rigidbody playerRigidbody;

    // Optional: Reference to the OVRManager if you need to ensure settings are correct
    // private OVRManager ovrManager; 

    private void Awake()
    {
        // Get references if not manually assigned in the Inspector
        if (headTransform == null)
        {
            // Assumes the camera is a child named 'Main Camera' or similar
            headTransform = Camera.main.transform;
        }
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        // Subscribe to the system event that fires AFTER the headset position has been internally reset
        if (OVRManager.display != null)
        {
            OVRManager.display.RecenteredPose += HandleSystemRecenter;
            Debug.Log("Subscribed to OVRManager Recenter event.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe when the object is disabled or destroyed
        if (OVRManager.display != null)
        {
            OVRManager.display.RecenteredPose -= HandleSystemRecenter;
        }
    }

    /// <summary>
    /// This function is called automatically when the user holds the Meta Menu button.
    /// </summary>
    private void HandleSystemRecenter()
    {
        Debug.Log("System Recenter detected. Adjusting game world position.");

        // 1. Calculate the horizontal offset from the rig's root to the headset's current position
        Vector3 playerOffset = headTransform.position - transform.position;
        playerOffset.y = 0; // Ignore vertical offset, keeping player height consistent

        // 2. Determine the new position for the root rig: 
        // Move the rig backwards by the player's offset to align the head to world zero (0,0,0)
        Vector3 newPosition = Vector3.zero - playerOffset;

        // 3. Teleport the root rig using transform.position (safer for Auto Hand's Rigidbody teleport)
        transform.position = newPosition;

        // 4. Reset velocities to ensure the player doesn't slide after the teleport
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // 5. Re-orient the player to face forward (Optional but recommended)
        float rotationOffset = headTransform.eulerAngles.y;
        transform.RotateAround(headTransform.position, Vector3.up, -rotationOffset);
    }
}

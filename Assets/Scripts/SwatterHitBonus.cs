using UnityEngine;

// Attach to the swatter. Awards extra points when a thrown swatter hits a bug.
public class SwatterHitBonus : MonoBehaviour
{
    [Header("References")]
    [Tooltip("If the swatter is parented under any transform with these tags, it's considered in-hand. Leave empty to skip.")]
    public string[] handTags = new string[] { "LeftHand", "RightHand" };

    [Header("Scoring")]
    public int hitPoints = 1;          // Points for any hit
    public int thrownBonusPoints = 2;  // Extra points when the swatter is not in hand

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    void HandleHit(Collider other)
    {
        if (other == null || !other.CompareTag("Bug"))
            return;

        bool inHand = IsInAnyHandByTag();
        int points = inHand ? hitPoints : hitPoints + thrownBonusPoints;

        ScoreKeeper.score += points;
        Destroy(other.gameObject);
    }

    // Detect if the swatter is parented under a hand/controller by tag.
    bool IsInAnyHandByTag()
    {
        if (handTags == null || handTags.Length == 0)
            return false;

        Transform current = transform;
        while (current != null)
        {
            foreach (var tag in handTags)
            {
                if (!string.IsNullOrEmpty(tag) && current.CompareTag(tag))
                    return true;
            }
            current = current.parent;
        }
        return false;
    }
}


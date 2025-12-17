using UnityEngine;

/// <summary>
/// Ensures this canvas always renders on top by setting a high sort order
/// </summary>
[RequireComponent(typeof(Canvas))]
public class EnsureTopCanvas : MonoBehaviour
{
    [Tooltip("Sort order value (higher = renders on top)")]
    public int sortOrder = 100;

    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = sortOrder;
        }
    }
}


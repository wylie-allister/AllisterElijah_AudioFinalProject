using UnityEngine;

/// Tracks whether the scooter is in contact with the Boundary layer/tag.
[RequireComponent(typeof(Rigidbody2D))]
public class ScooterContactState : MonoBehaviour
{
    [Header("Filtering")]
    public bool useTag = true;
    public string boundaryTag = "Boundary";
    public bool useLayer = false;
    public LayerMask boundaryLayers;

    [HideInInspector] public bool touchingBoundary;

    void OnCollisionEnter2D(Collision2D c) { Check(c.collider, true); }
    void OnCollisionStay2D  (Collision2D c) { Check(c.collider, true); }
    void OnCollisionExit2D  (Collision2D c) { Check(c.collider, false); }

    void Check(Collider2D col, bool value)
    {
        if (!col) return;
        if (useTag && col.CompareTag(boundaryTag))             { touchingBoundary = value; return; }
        if (useLayer && ((boundaryLayers.value & (1 << col.gameObject.layer)) != 0))
        { touchingBoundary = value; return; }
    }
}

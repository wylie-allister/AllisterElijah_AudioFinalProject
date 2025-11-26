using UnityEngine;

/// <summary>
/// CollisionToHUD
/// - Shows a short HUD warning when the player hits (or enters a trigger on) the boundary.
/// - Logs to Console so you can verify it's firing.
/// - Works with BOTH Collision and Trigger events (2D).
/// HOW TO USE:
///   • Put this on the Scooter (must have Rigidbody2D + Collider2D).
///   • Drag Canvas/HUD (with HUDController) into 'hud'.
///   • EITHER set 'useTagFilter' with tag "Boundary" OR set 'useLayerFilter' with the Boundary layer.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CollisionToHUD : MonoBehaviour
{
    [Header("HUD")]
    [Tooltip("Drag Canvas/HUD (HUDController) here.")]
    public HUDController hud;

    [Header("Message")]
    [Tooltip("Message to display briefly when boundary contact occurs.")]
    public string boundaryMessage = "Careful! You hit the rail.";
    [Tooltip("Seconds the message stays on-screen.")]
    public float messageSeconds = 1.2f;

    [Header("Filtering (choose ONE)")]
    public bool useTagFilter = true;
    [Tooltip("Only react if other collider has this tag (e.g., 'Boundary').")]
    public string boundaryTag = "Boundary";

    public bool useLayerFilter = false;
    [Tooltip("Only react if other collider is on one of these layers.")]
    public LayerMask boundaryLayers;

    // --- COLLISION (non-trigger) ---
    void OnCollisionEnter2D(Collision2D c)
    {
        if (!PassesFilter(c.collider)) return;

        Debug.Log($"[CollisionToHUD] OnCollisionEnter2D with '{c.collider.name}' (layer {c.collider.gameObject.layer}).");
        ShowBoundaryHit();
    }

    // --- TRIGGER (in case Boundary is configured as trigger) ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!PassesFilter(other)) return;

        Debug.Log($"[CollisionToHUD] OnTriggerEnter2D with '{other.name}' (layer {other.gameObject.layer}).");
        ShowBoundaryHit();
    }

    // --- Helpers ---
    bool PassesFilter(Collider2D col)
    {
        if (col == null) return false;

        // Tag filter (default)
        if (useTagFilter && !string.IsNullOrEmpty(boundaryTag))
        {
            if (!col.CompareTag(boundaryTag)) return false;
        }

        // Layer filter (optional alternative)
        if (useLayerFilter)
        {
            int layer = col.gameObject.layer;
            if ((boundaryLayers.value & (1 << layer)) == 0) return false;
        }

        return true;
    }

    void ShowBoundaryHit()
    {
        if (hud == null)
        {
            // Fallback: try to find a HUD in scene so you still get feedback while wiring
            hud = FindObjectOfType<HUDController>();
            if (hud == null)
            {
                Debug.LogWarning("[CollisionToHUD] No HUDController assigned/found; message will not display.");
                return;
            }
        }

        hud.ShowCollision(boundaryMessage, messageSeconds);
    }
}

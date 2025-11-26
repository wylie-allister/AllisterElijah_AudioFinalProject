using UnityEngine;
/// Attach to a dynamic body to log collisions (for debugging).
public class CollisionProbe2D : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
        => Debug.Log($"[CollisionProbe2D] HIT {col.collider.name} tag={col.collider.tag} relVel={col.relativeVelocity}");
}

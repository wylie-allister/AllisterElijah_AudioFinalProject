using UnityEngine;

/// Put this on CameraRig (parent of Main Camera).
/// Smoothly follows a 2D target without fighting CameraShake2D on the child camera.
public class CameraRigFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                // drag Scooter here

    [Header("Follow")]
    public float smoothTime = 0.18f;        // lower = snappier
    public Vector2 offset = Vector2.zero;   // screen-space offset in world units

    [Header("Bounds (optional)")]
    public bool clampToBounds = false;
    public Rect worldBounds = new Rect(-999, -999, 1998, 1998); // set from your map

    private Vector3 velocity;               // ref for SmoothDamp

    void LateUpdate()
    {
        if (!target) return;

        // Desired rig position (camera child handles Z and shake)
        Vector3 desired = new Vector3(target.position.x + offset.x,
                                      target.position.y + offset.y,
                                      transform.position.z);

        // Smooth follow
        Vector3 next = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        // Optional clamp
        if (clampToBounds)
        {
            // Clamp rig so its child camera stays within bounds; assumes camera Z stays at -10 on child.
            next.x = Mathf.Clamp(next.x, worldBounds.xMin, worldBounds.xMax);
            next.y = Mathf.Clamp(next.y, worldBounds.yMin, worldBounds.yMax);
        }

        transform.position = next;
    }
}

using UnityEngine;

/// Smooth, jitter-resistant heading for a top-down scooter.
/// Rotates THIS transform (or a parent pivot if you moved the SpriteRenderer to a child).
/// Priority: input axes → velocity → last direction.
/// Damps tiny direction changes, caps turn rate, and optionally ignores velocity on boundary contact.
[RequireComponent(typeof(Rigidbody2D))]
public class ScooterHeadingSmooth : MonoBehaviour
{
    [Header("Sprite Forward")]
    [Tooltip("Angle so the sprite's nose matches heading. 0=RIGHT, 90=UP, 180=LEFT, 270=DOWN.")]
    public float forwardAngleOffset = 270f;

    [Header("Input")]
    public bool useInputAxes = true;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis   = "Vertical";

    [Header("Smoothing")]
    [Tooltip("Direction smoothing time (s). Higher = smoother, lower = snappier.")]
    public float dirSmoothTime = 0.14f;

    [Tooltip("Angle smoothing time (s) used by SmoothDampAngle.")]
    public float angleSmoothTime = 0.10f;

    [Tooltip("Max degrees the scooter can turn per second.")]
    public float maxTurnRateDegPerSec = 360f;

    [Header("Thresholds")]
    [Tooltip("Ignore tiny input/speed (units/s).")]
    public float minDirMag = 0.08f;

    [Tooltip("Ignore tiny angle changes (deg) to prevent micro-oscillation.")]
    public float jitterDeadzoneDeg = 1.2f;

    [Header("Optional Contact (reduces wall jitter)")]
    [Tooltip("If assigned, we ignore velocity while touching boundary colliders.")]
    public ScooterContactState contact;

    private Rigidbody2D rb;
    private Vector2 smoothedDir = Vector2.right; // filtered travel direction
    private Vector3 lastPos;
    private float angleVel; // ref velocity for SmoothDampAngle

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!contact) contact = GetComponent<ScooterContactState>();
        lastPos = transform.position;

        // Initialize smoothedDir to sprite's forward
        float init = forwardAngleOffset * Mathf.Deg2Rad;
        smoothedDir = new Vector2(Mathf.Cos(init), Mathf.Sin(init));
    }

    void Update()
    {
        // Desired direction: input first
        Vector2 dir = Vector2.zero;
        if (useInputAxes)
        {
            float x = Input.GetAxisRaw(horizontalAxis);
            float y = Input.GetAxisRaw(verticalAxis);
            dir = new Vector2(x, y);
        }

        // If no meaningful input, use velocity only when not scraping boundary
        bool canUseVelocity = (contact == null || !contact.touchingBoundary);
        if (dir.sqrMagnitude < minDirMag * minDirMag && canUseVelocity)
        {
            Vector2 v = rb.velocity;
            if (v.sqrMagnitude >= minDirMag * minDirMag) dir = v;
        }

        // Smoothly blend current smoothedDir toward desired direction (if significant)
        if (dir.sqrMagnitude >= minDirMag * minDirMag)
        {
            Vector2 desired = dir.normalized;

            // Exponential smoothing factor: alpha = 1 - exp(-dt/tau)
            float dt = Time.deltaTime;
            float alpha = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, dirSmoothTime));

            // Use Vector3.Slerp for a nice arc blend, then normalize back to Vector2
            Vector3 from = new Vector3(smoothedDir.x, smoothedDir.y, 0f);
            Vector3 to   = new Vector3(desired.x,    desired.y,    0f);
            Vector3 slerped = Vector3.Slerp(from, to, Mathf.Clamp01(alpha));
            if (slerped.sqrMagnitude > 1e-6f)
                smoothedDir = new Vector2(slerped.x, slerped.y).normalized;
        }

        // Compute target angle (+ sprite nose offset)
        float desiredDeg = Mathf.Atan2(smoothedDir.y, smoothedDir.x) * Mathf.Rad2Deg + forwardAngleOffset;

        // Current angle and small deadzone to avoid flicker
        float current = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(current, desiredDeg);
        if (Mathf.Abs(delta) < jitterDeadzoneDeg) return;

        // Smooth toward target and cap max turn rate
        float damped = Mathf.SmoothDampAngle(current, desiredDeg, ref angleVel, angleSmoothTime, Mathf.Infinity, Time.deltaTime);
        float maxStep = maxTurnRateDegPerSec * Time.deltaTime;
        float final = Mathf.MoveTowardsAngle(current, damped, maxStep);

        transform.rotation = Quaternion.Euler(0f, 0f, final);
    }
}

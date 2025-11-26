using UnityEngine;

/// 2D top-down scooter controller with smooth accel/decel and no lurch on direction change.
/// - Consistent top speed in all directions (fixes diagonal √2 issue)
/// - Accel/decel caps per second (no sudden spikes when reversing)
/// - Sprint raises target speed only (doesn't instantly boost velocity)
/// - Sets rb.velocity directly in FixedUpdate (no AddForce needed)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Speeds")]
    [Tooltip("Top speed while not sprinting (units/sec).")]
    public float maxSpeed = 6f;

    [Tooltip("Sprint speed multiplier (applied to target speed).")]
    public float sprintMultiplier = 1.4f;

    [Header("Acceleration")]
    [Tooltip("How fast you can accelerate toward target velocity (units/sec^2).")]
    public float acceleration = 20f;

    [Tooltip("How fast you can slow down when no input (units/sec^2).")]
    public float deceleration = 24f;

    [Tooltip("Extra decel used when input reverses direction (helps prevent ‘snap’).")]
    public float reverseBoostDecel = 36f;

    [Header("Input")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis   = "Vertical";
    public KeyCode sprintKey     = KeyCode.LeftShift;

    [Header("Physics")]
    [Tooltip("Optional extra drag for micro smoothing. Keep small (0–2).")]
    public float linearDrag = 2f;

    Rigidbody2D rb;
    Vector2 cachedInput;   // raw axes from Update

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
        rb.drag = linearDrag;
    }

    void Update()
    {
        // Read raw axes
        float x = Input.GetAxisRaw(horizontalAxis);
        float y = Input.GetAxisRaw(verticalAxis);
        cachedInput = new Vector2(x, y);

        // Clamp magnitude to 1 so diagonals aren't faster
        float mag = cachedInput.magnitude;
        if (mag > 1f) cachedInput /= mag; // normalized without forcing direction when mag<1
    }

    void FixedUpdate()
    {
        Vector2 currentVel = rb.velocity;

        // Desired direction & speed
        Vector2 inputDir = cachedInput.sqrMagnitude > 0.0001f ? cachedInput.normalized : Vector2.zero;
        float targetTopSpeed = maxSpeed * (Input.GetKey(sprintKey) ? sprintMultiplier : 1f);
        Vector2 desiredVel = inputDir * (targetTopSpeed * cachedInput.magnitude); // preserves analog magnitude (0..1)

        // Choose accel budget this frame
        float dt = Time.fixedDeltaTime;
        float accelBudget;

        if (inputDir.sqrMagnitude < 0.0001f)
        {
            // No input: brake toward zero with deceleration
            accelBudget = deceleration * dt;
            rb.velocity = Vector2.MoveTowards(currentVel, Vector2.zero, accelBudget);
            return;
        }

        // If reversing (desired opposite of current), use stronger decel first
        float alignment = 0f;
        if (currentVel.sqrMagnitude > 0.0001f)
            alignment = Vector2.Dot(currentVel.normalized, inputDir); // -1 opposite, 1 same

        if (alignment < 0f)
        {
            // First slow down with boosted decel until not opposing
            accelBudget = reverseBoostDecel * dt;
        }
        else
        {
            // Normal acceleration toward target velocity
            accelBudget = acceleration * dt;
        }

        // Smoothly move velocity toward desired, bounded by accelBudget
        rb.velocity = Vector2.MoveTowards(currentVel, desiredVel, accelBudget);
    }
}

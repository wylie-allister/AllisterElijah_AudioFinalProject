using UnityEngine;

/// Adds spill when steering/accel changes abruptly (jerk).
/// Reads Rigidbody2D velocity each FixedUpdate, computes acceleration delta.
[RequireComponent(typeof(Rigidbody2D))]
public class SpillJerkMonitor : MonoBehaviour
{
    public SpillMeter spill;                 // assign in Inspector (or it will auto-find)
    [Header("Sensitivity")]
    public float jerkToSpill = 0.8f;         // multiplier from jerk magnitude to spill
    public float minJerkThreshold = 2.0f;    // ignore tiny micro-changes
    public float maxPerSecond = 25f;         // safety cap per second from jerk

    private Rigidbody2D rb;
    private Vector2 lastVel;
    private float accumulatedThisFrame;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!spill) spill = FindObjectOfType<SpillMeter>();
        lastVel = rb.velocity;
    }

    void FixedUpdate()
    {
        Vector2 v = rb.velocity;
        float dt = Mathf.Max(Time.fixedDeltaTime, 0.0001f);

        // jerk ~ |a_now - a_last|; approximate via velocity delta over dt
        Vector2 aNow = (v - lastVel) / dt;
        // We can approximate jerk as acceleration magnitude (works well in top-down)
        float jerkMag = aNow.magnitude;

        if (jerkMag > minJerkThreshold && spill != null)
        {
            float add = jerkMag * jerkToSpill * dt;
            accumulatedThisFrame += add;
        }

        lastVel = v;
    }

    void LateUpdate()
    {
        if (spill == null || accumulatedThisFrame <= 0f) { accumulatedThisFrame = 0f; return; }

        // Cap per second contribution
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float cap = maxPerSecond * dt;
        float toApply = Mathf.Min(accumulatedThisFrame, cap);
        spill.Increase(toApply);
        accumulatedThisFrame = 0f;
    }
}

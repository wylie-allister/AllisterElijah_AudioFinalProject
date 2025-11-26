using UnityEngine;

/// Standalone boost/idle dust toggler that reads the rigidbody velocity.
[RequireComponent(typeof(Rigidbody2D))]
public class BoostDustToggle : MonoBehaviour
{
    public ParticleSystem dustBoost;
    public ParticleSystem dustIdle;
    public KeyCode boostKey = KeyCode.LeftShift;
    public float idleMinSpeed = 0.2f;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void FixedUpdate()
    {
        if (!rb) return;
        // Boost on Shift + moving
        if (dustBoost) { var emB = dustBoost.emission; emB.enabled = Input.GetKey(boostKey) && (rb.velocity.sqrMagnitude > 0.01f); }
        // Idle on when moving
        if (dustIdle)  { var emI = dustIdle.emission;  emI.enabled = (rb.velocity.magnitude > idleMinSpeed); }
    }
}


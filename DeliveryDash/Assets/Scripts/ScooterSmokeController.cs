using UnityEngine;

/// Controls idle/boost dust. Idle when moving, boost when accelerating.
public class ScooterSmokeController : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem dustIdle;
    public ParticleSystem dustBoost;

    [Header("Tuning")]
    public float idleMinSpeed = 0.2f;   // movement threshold for idle dust
    public float accelThreshold = 1.0f; // accel magnitude to consider "accelerating"
    public float boostScaleBySpeed = 0.3f;

    private Rigidbody2D rb;
    private Vector2 lastVel;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastVel = rb ? rb.velocity : Vector2.zero;

        // ensure the modules are enabled (in case)
        if (dustIdle)  { var em = dustIdle.emission;  em.enabled = true; }
        if (dustBoost) { var em = dustBoost.emission; em.enabled = false; } // off until accelerating
    }

    void Update()
    {
        if (!rb) return;

        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector2 v = rb.velocity;
        float speed = v.magnitude;

        // approximate acceleration
        float accel = ((v - lastVel) / dt).magnitude;
        bool accelerating = accel > accelThreshold;

        // idle dust on when moving
        ToggleEmission(dustIdle, speed > idleMinSpeed);

        // boost dust on when accelerating
        if (dustBoost)
        {
            var em = dustBoost.emission;
            em.enabled = accelerating;

            if (accelerating)
            {
                // scale density a bit by speed
                float rate = Mathf.Lerp(20f, 50f, Mathf.Clamp01(speed * boostScaleBySpeed));
                var rateOT = em.rateOverTime;
                rateOT.constant = rate;
                em.rateOverTime = rateOT;
            }
        }

        lastVel = v;
    }

    void ToggleEmission(ParticleSystem ps, bool on)
    {
        if (!ps) return;
        var em = ps.emission;
        if (em.enabled != on) em.enabled = on;
        if (on && !ps.isPlaying) ps.Play();
        if (!on && ps.isPlaying) ps.Stop();
    }
}


using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ScooterCollisionFX : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask trafficMask;    // cars
    public LayerMask boundaryMask;   // walls/rails

    [Header("Spill")]
    [Tooltip("Drag the SpillMeter component here (usually on Systems).")]
    public SpillMeter spill;

    [Header("Spill Penalties")]
    public float trafficSpillBase = 14f;
    public float boundarySpillBase = 8f;

    [Header("Impact Scaling")]
    public float referenceSpeed = 4f;             // 0 = disable scaling
    public Vector2 scaleClamp = new Vector2(0.6f, 3.0f);

    [Header("Visual Scale (x baseline prefab)")]
    public float visualBaseMul = 10f;             // ~10× bigger as requested
    public float trafficVisualBoost = 1.0f;
    public float boundaryVisualBoost = 1.0f;

    [Header("Spam Control")]
    public float cooldown = 0.06f;

    private float lastFxTime;

    void Awake()
    {
        if (!spill) spill = FindObjectOfType<SpillMeter>(); // auto-find as fallback
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (Time.time - lastFxTime < cooldown) return;

        int other = col.collider.gameObject.layer;
        bool hitTraffic  = (trafficMask.value  & (1 << other)) != 0;
        bool hitBoundary = (boundaryMask.value & (1 << other)) != 0;
        if (!hitTraffic && !hitBoundary) return;

        // Contact point & normal
        Vector2 pos = transform.position, normal = Vector2.up;
        if (col.contactCount > 0) { var c = col.GetContact(0); pos = c.point; normal = c.normal; }

        // Visuals (~10×) with optional impact scaling
        float scale = 1f;
        if (referenceSpeed > 0f)
        {
            float rel = col.relativeVelocity.magnitude;
            scale = Mathf.Clamp(rel / referenceSpeed, scaleClamp.x, scaleClamp.y);
        }

        if (SparkFXPool.Instance != null)
        {
            float baseMul = Mathf.Max(0.1f, visualBaseMul) * scale;
            if (hitTraffic)
                SparkFXPool.Instance.PlayBigScaled(pos, normal, baseMul * trafficVisualBoost, baseMul * trafficVisualBoost);
            else
                SparkFXPool.Instance.PlaySmallScaled(pos, normal, baseMul * boundaryVisualBoost, baseMul * boundaryVisualBoost);
        }

        // Spill increase
        if (spill != null)
        {
            float amt = (hitTraffic ? trafficSpillBase : boundarySpillBase) * scale;
            if (amt > 0f) spill.Increase(amt);  // <-- direct, no reflection
        }

        lastFxTime = Time.time;
    }
}

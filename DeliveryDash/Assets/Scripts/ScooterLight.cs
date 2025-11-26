using UnityEngine;

/// Headlight cone controller (hold-to-activate).
/// - Shows the beam WHILE the key is held (default: Space).
/// - Positions the cone in front of the scooter (local +X) with optional lateral offset.
/// - Pulses scale a bit when on.
/// - Never modifies the child's rotation (so set your Light_beam's local Z in the Inspector and it will stick).
public class ScooterLight : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer lightBeamRenderer;   // Drag Scooter/Light_beam SpriteRenderer here

    [Header("Input")]
    public KeyCode lightKey = KeyCode.Space;   // Hold this to show the light

    [Header("Local Position (relative to Scooter)")]
    [Tooltip("How far in front of the scooter (local +X).")]
    public float frontOffset = 0.65f;
    [Tooltip("Side offset (local +Y).")]
    public float lateralOffset = 0f;

    [Header("Look")]
    [Range(0f,1f)] public float baseAlpha = 0.5f;
    public Vector2 baseScale = new Vector2(0.25f, 0.40f);
    public float pulseScale = 1.05f;   // 1.0 = no pulse
    public float pulseSpeed = 6f;

    private Transform beamTf;

    void Awake()
    {
        if (lightBeamRenderer) beamTf = lightBeamRenderer.transform;
    }

    void Update()
    {
        if (!beamTf || !lightBeamRenderer) return;

        // Hold-to-activate: on only while key is held
        bool isOn = Input.GetKey(lightKey);

        // Keep the cone in front of the scooter (local +X, +Y)
        beamTf.localPosition = new Vector3(frontOffset, lateralOffset, 0f);

        // DO NOT touch rotation here; set Light_beam's local Z in Inspector and it will remain.

        // Visuals
        float pulse = isOn ? (1f + (pulseScale - 1f) * 0.5f * (1f + Mathf.Sin(Time.time * pulseSpeed))) : 1f;

        // Alpha
        var c = lightBeamRenderer.color;
        c.a = isOn ? baseAlpha : 0f;
        lightBeamRenderer.color = c;

        // Scale
        beamTf.localScale = new Vector3(baseScale.x * pulse, baseScale.y * pulse, 1f);
    }
}

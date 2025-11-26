using System.Collections;
using UnityEngine;

/// Moves a car along a TrafficPath, looping with fade at ends.
/// Supports staggered start, random start point, visual facing offset,
/// and basic "follow distance" avoidance to reduce overlapping.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TrafficCar : MonoBehaviour
{
    [Header("Path")]
    public TrafficPath path;
    [Tooltip("If true, traverses the path in reverse order.")]
    public bool reverseDirection = false;

    [Tooltip("Start from a random waypoint (helps distribute multiple cars).")]
    public bool startAtRandomPoint = true;

    [Tooltip("Delay (seconds) before this car starts moving (stagger starts).")]
    public float startDelay = 0f;

    [Header("Motion")]
    [Tooltip("Units per second along the path.")]
    public float speed = 2.2f;

    [Tooltip("How close to a point to consider it reached.")]
    public float pointEpsilon = 0.03f;

    [Header("Visual Facing")]
    [Tooltip("Angle so the car's nose matches movement. 0=RIGHT, 90=UP, 180=LEFT, 270=DOWN.")]
    public float forwardAngleOffset = 90f;

    [Header("Looping / FX")]
    [Tooltip("Seconds to pause at path end before fading.")]
    public float pauseAtEnds = 0.0f;
    [Tooltip("Seconds to fade out/in at the ends.")]
    public float fadeDuration = 0.35f;

    [Header("Avoid Overlap")]
    [Tooltip("If true, slows down when another Traffic car is directly ahead.")]
    public bool avoidBumping = true;
    [Tooltip("Preferred minimum distance to the car ahead.")]
    public float followDistance = 0.9f;
    [Tooltip("Layer mask for Traffic cars (set to 'Traffic').")]
    public LayerMask trafficMask;

    private SpriteRenderer sr;
    private int index = 0;
    private int dir = 1; // +1 forward, -1 backward
    private bool isFading = false;
    private bool started = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        var rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (path == null || path.Count < 2)
        {
            Debug.LogWarning($"[TrafficCar] '{name}' has no valid path assigned.");
            enabled = false;
            return;
        }

        dir = reverseDirection ? -1 : 1;

        // Choose starting index
        if (startAtRandomPoint)
            index = reverseDirection ? Random.Range(1, path.Count) : Random.Range(0, path.Count - 1);
        else
            index = reverseDirection ? path.Count - 1 : 0;

        // Snap to start
        transform.position = path[index].position;
        SetAlpha(1f);
    }

    IEnumerator Start()
    {
        if (startDelay > 0f)
        {
            // Optional subtle fade-in on start delay
            SetAlpha(0f);
            yield return new WaitForSeconds(startDelay);
            yield return FadeTo(1f, 0.2f);
        }
        started = true;
    }

    void Update()
    {
        if (!started || isFading || path == null || path.Count < 2) return;

        int nextIndex = index + dir;
        if (nextIndex < 0 || nextIndex >= path.Count)
        {
            StartCoroutine(FadeAndRestart());
            return;
        }

        Vector3 a = path[index].position;
        Vector3 b = path[nextIndex].position;
        Vector3 segment = (b - a);
        float baseSpeed = speed;

        // Simple "car in front" avoidance using a short raycast/boxcast
        if (avoidBumping && segment.sqrMagnitude > 0.0001f)
        {
            Vector2 dir2 = segment.normalized;
            float castDist = followDistance;

            // Raycast slightly ahead to detect another Traffic car
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir2, castDist, trafficMask);
            if (hit.collider && hit.collider.gameObject != gameObject)
            {
                // Scale speed down as we get close. At contact, nearly 0.
                float t = Mathf.Clamp01(hit.distance / castDist);
                baseSpeed *= Mathf.Lerp(0.1f, 1f, t);
            }
        }

        // Move toward next point
        transform.position = Vector3.MoveTowards(transform.position, b, baseSpeed * Time.deltaTime);

        // Rotate to face the path direction
        if (segment.sqrMagnitude > 0.0001f)
        {
            float ang = Mathf.Atan2(segment.y, segment.x) * Mathf.Rad2Deg + forwardAngleOffset;
            transform.rotation = Quaternion.Euler(0, 0, ang);
        }

        // Reached segment end?
        if ((transform.position - b).sqrMagnitude <= (pointEpsilon * pointEpsilon))
        {
            index = nextIndex;
        }
    }

    IEnumerator FadeAndRestart()
    {
        isFading = true;
        if (pauseAtEnds > 0f) yield return new WaitForSeconds(pauseAtEnds);

        yield return FadeTo(0f, fadeDuration);

        index = reverseDirection ? path.Count - 1 : 0;
        transform.position = path[index].position;

        yield return FadeTo(1f, fadeDuration);
        isFading = false;
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startA = sr.color.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startA, targetAlpha, Mathf.Clamp01(t / duration));
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(targetAlpha);
    }

    void SetAlpha(float a)
    {
        var c = sr.color; c.a = a; sr.color = c;
    }
}

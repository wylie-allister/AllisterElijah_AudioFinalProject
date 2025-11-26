using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Object pool for spark particle bursts (big/small).
/// Supports scaled plays (size/speed multipliers) per collision intensity.
public class SparkFXPool : MonoBehaviour
{
    public static SparkFXPool Instance { get; private set; }

    [Header("Prefabs")]
    public ParticleSystem bigSparksPrefab;
    public ParticleSystem smallSparksPrefab;

    [Header("Prewarm")]
    public int prewarmBig = 8;
    public int prewarmSmall = 12;

    private readonly Queue<ParticleSystem> bigPool = new Queue<ParticleSystem>();
    private readonly Queue<ParticleSystem> smallPool = new Queue<ParticleSystem>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (bigSparksPrefab) Prewarm(bigSparksPrefab, bigPool, prewarmBig);
        if (smallSparksPrefab) Prewarm(smallSparksPrefab, smallPool, prewarmSmall);
    }

    void Prewarm(ParticleSystem prefab, Queue<ParticleSystem> pool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var ps = Instantiate(prefab, transform);
            ps.gameObject.SetActive(false);
            pool.Enqueue(ps);
        }
    }

    ParticleSystem Get(Queue<ParticleSystem> pool, ParticleSystem prefab)
    {
        ParticleSystem ps = null;
        if (pool.Count > 0) ps = pool.Dequeue();
        else ps = Instantiate(prefab, transform);

        ps.gameObject.SetActive(true);
        return ps;
    }

    IEnumerator ReturnWhenDone(ParticleSystem ps, Queue<ParticleSystem> pool)
    {
        var main = ps.main;
        float maxWait = main.duration + main.startLifetime.constantMax + 0.5f;
        float t = 0f;
        while (t < maxWait && ps.IsAlive(true))
        {
            t += Time.deltaTime;
            yield return null;
        }
        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }

    Quaternion AlignToNormal(Vector2 normal, float randomYaw)
    {
        Vector3 up = new Vector3(normal.x, normal.y, 0f);
        if (up.sqrMagnitude < 1e-6f) up = Vector3.up;
        var rot = Quaternion.FromToRotation(Vector3.up, up);
        if (randomYaw != 0f) rot *= Quaternion.Euler(0, 0, Random.Range(-randomYaw, randomYaw));
        return rot;
    }

    // --- Base plays (unchanged) ---
    public void PlayBig(Vector2 worldPos, Vector2 surfaceNormal, float randomYaw = 12f)
    {
        if (!bigSparksPrefab) return;
        var ps = Get(bigPool, bigSparksPrefab);
        ps.transform.SetPositionAndRotation(worldPos, AlignToNormal(surfaceNormal, randomYaw));
        ps.Play();
        StartCoroutine(ReturnWhenDone(ps, bigPool));
    }
    public void PlaySmall(Vector2 worldPos, Vector2 surfaceNormal, float randomYaw = 8f)
    {
        if (!smallSparksPrefab) return;
        var ps = Get(smallPool, smallSparksPrefab);
        ps.transform.SetPositionAndRotation(worldPos, AlignToNormal(surfaceNormal, randomYaw));
        ps.Play();
        StartCoroutine(ReturnWhenDone(ps, smallPool));
    }

    // --- Scaled plays (size/speed multipliers) ---
    public void PlayBigScaled(Vector2 worldPos, Vector2 surfaceNormal, float sizeMul, float speedMul, float randomYaw = 12f)
    {
        if (!bigSparksPrefab) return;
        var ps = Get(bigPool, bigSparksPrefab);
        ps.transform.SetPositionAndRotation(worldPos, AlignToNormal(surfaceNormal, randomYaw));

        var main = ps.main;
        float oldSizeMul = main.startSizeMultiplier;
        float oldSpeedMul = main.startSpeedMultiplier;

        main.startSizeMultiplier = oldSizeMul * Mathf.Max(0.1f, sizeMul);
        main.startSpeedMultiplier = oldSpeedMul * Mathf.Max(0.1f, speedMul);

        ps.Play();
        StartCoroutine(RestoreAndReturn(ps, bigPool, oldSizeMul, oldSpeedMul));
    }

    public void PlaySmallScaled(Vector2 worldPos, Vector2 surfaceNormal, float sizeMul, float speedMul, float randomYaw = 8f)
    {
        if (!smallSparksPrefab) return;
        var ps = Get(smallPool, smallSparksPrefab);
        ps.transform.SetPositionAndRotation(worldPos, AlignToNormal(surfaceNormal, randomYaw));

        var main = ps.main;
        float oldSizeMul = main.startSizeMultiplier;
        float oldSpeedMul = main.startSpeedMultiplier;

        main.startSizeMultiplier = oldSizeMul * Mathf.Max(0.1f, sizeMul);
        main.startSpeedMultiplier = oldSpeedMul * Mathf.Max(0.1f, speedMul);

        ps.Play();
        StartCoroutine(RestoreAndReturn(ps, smallPool, oldSizeMul, oldSpeedMul));
    }

    IEnumerator RestoreAndReturn(ParticleSystem ps, Queue<ParticleSystem> pool, float oldSizeMul, float oldSpeedMul)
    {
        // Wait for finish
        var main = ps.main;
        float maxWait = main.duration + main.startLifetime.constantMax + 0.5f;
        float t = 0f;
        while (t < maxWait && ps.IsAlive(true))
        {
            t += Time.deltaTime;
            yield return null;
        }
        // restore multipliers for next reuse
        main.startSizeMultiplier = oldSizeMul;
        main.startSpeedMultiplier = oldSpeedMul;

        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }
}


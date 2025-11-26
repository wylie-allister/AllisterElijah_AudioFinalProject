using System.Collections;
using UnityEngine;
/* ... (same as previous annotated EngineAudioController code) ... */
[RequireComponent(typeof(Rigidbody2D))]
public class EngineAudioController : MonoBehaviour
{
    public AudioCue engineBaseCue, engineBoostCue;
    public AudioSource engineBaseSource, engineBoostSource;
    public float minPitch=0.85f, maxPitch=1.35f, minVolume=0.15f, maxVolume=0.8f, speedForMax=6f;
    public KeyCode boostKey = KeyCode.LeftShift; public float boostFade=0.2f;
    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!engineBaseSource) engineBaseSource = gameObject.AddComponent<AudioSource>();
        if (!engineBoostSource) engineBoostSource = gameObject.AddComponent<AudioSource>();
        engineBaseSource.loop = engineBoostSource.loop = true;
        engineBaseSource.spatialBlend = engineBoostSource.spatialBlend = 0f;
        if (engineBaseCue) AudioManager.Instance?.PlayLoopOn(engineBaseCue, engineBaseSource, 0f);
        if (engineBoostCue) AudioManager.Instance?.PlayLoopOn(engineBoostCue, engineBoostSource, 0f);
        engineBoostSource.volume = 0f;
    }
    void Update()
    {
        float speed = rb ? rb.velocity.magnitude : 0f;
        float t = Mathf.Clamp01(speedForMax>0f? speed/speedForMax: 0f);
        engineBaseSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        engineBaseSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
        bool boosting = Input.GetKey(boostKey);
        StopAllCoroutines(); StartCoroutine(Fade(engineBoostSource, boosting?1f:0f, boostFade));
    }
    IEnumerator Fade(AudioSource src, float target, float dur)
    {
        float start=src.volume, t=0f; while (t<dur){ t+=Time.deltaTime; src.volume=Mathf.Lerp(start,target,t/dur); yield return null; } src.volume=target;
    }
}
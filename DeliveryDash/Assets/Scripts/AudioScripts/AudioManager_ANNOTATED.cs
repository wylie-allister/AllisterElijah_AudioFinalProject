using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/* ... (same as previous annotated AudioManager code) ... */
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] int poolSize = 16;
    [SerializeField] bool dontDestroyOnLoad = true;
    private readonly Queue<AudioSource> pool = new Queue<AudioSource>();
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("AudioSource_" + i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.dopplerLevel = 0f;
            go.SetActive(false);
            pool.Enqueue(src);
        }
    }
    AudioSource GetSource()
    {
        AudioSource src;
        if (pool.Count > 0) src = pool.Dequeue();
        else
        {
            var go = new GameObject("AudioSource_Dynamic");
            go.transform.SetParent(transform);
            src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
        }
        src.gameObject.SetActive(true);
        return src;
    }
    void ReturnSource(AudioSource src)
    {
        if (!src) return;
        src.Stop();
        src.clip = null;
        src.outputAudioMixerGroup = null;
        src.loop = false;
        src.transform.SetParent(transform);
        src.gameObject.SetActive(false);
        pool.Enqueue(src);
    }
    public void Play2D(AudioCue cue)
    {
        if (!cue || !cue.clip) return;
        var src = GetSource();
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = cue.output;
        src.pitch = cue.GetRandomizedPitch();
        src.volume = cue.volume;
        src.loop = cue.loop;
        if (cue.loop) { src.clip = cue.clip; src.Play(); }
        else { src.PlayOneShot(cue.clip, cue.volume); StartCoroutine(ReturnAfter(src, cue.clip.length / Mathf.Max(0.01f, src.pitch))); }
    }
    public void PlayAt(AudioCue cue, Vector3 position, float spatialBlend = 1f, float minDist = 5f, float maxDist = 25f)
    {
        if (!cue || !cue.clip) return;
        var src = GetSource();
        src.transform.position = position;
        src.spatialBlend = Mathf.Clamp01(spatialBlend);
        src.minDistance = minDist;
        src.maxDistance = Mathf.Max(minDist + 0.01f, maxDist);
        src.outputAudioMixerGroup = cue.output;
        src.pitch = cue.GetRandomizedPitch();
        src.volume = cue.volume;
        src.loop = cue.loop;
        if (cue.loop) { src.clip = cue.clip; src.Play(); }
        else { src.PlayOneShot(cue.clip, cue.volume); StartCoroutine(ReturnAfter(src, cue.clip.length / Mathf.Max(0.01f, src.pitch))); }
    }
    public AudioSource PlayLoopOn(AudioCue cue, AudioSource target, float spatialBlend = 0f)
    {
        if (!cue || !cue.clip || target == null) return null;
        target.clip = cue.clip;
        target.outputAudioMixerGroup = cue.output;
        target.loop = true;
        target.spatialBlend = spatialBlend;
        target.pitch = cue.basePitch;
        target.volume = cue.volume;
        target.Play();
        return target;
    }
    System.Collections.IEnumerator ReturnAfter(AudioSource src, float seconds)
    {
        yield return new WaitForSeconds(seconds + 0.05f);
        ReturnSource(src);
    }
}
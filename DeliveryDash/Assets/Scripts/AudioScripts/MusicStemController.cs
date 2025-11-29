using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
/* ... (same as previous annotated MusicStemController code) ... */
public class MusicStemController : MonoBehaviour
{
    public AudioCue drumsLow, drumsHigh, melodicA, melodicB;
    public AudioMixerGroup musicBus;
    public float fadeSeconds = 1f;
    private AudioSource aDrumsLow, aDrumsHigh, aMelA, aMelB;
    void Awake()
    {
        aDrumsLow = MakeSource("DrumsLow");
        aDrumsHigh = MakeSource("DrumsHigh");
        aMelA = MakeSource("MelodicA");
        aMelB = MakeSource("MelodicB");
    }
    AudioSource MakeSource(string name)
    {
        var go = new GameObject(name); go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false; src.loop = true; src.spatialBlend = 0f;
        src.outputAudioMixerGroup = musicBus; return src;
    }
    public void StartAll()
    {
        StartStem(drumsLow, aDrumsLow); StartStem(drumsHigh, aDrumsHigh);
        StartStem(melodicA, aMelA); StartStem(melodicB, aMelB);
    }
    void StartStem(AudioCue cue, AudioSource src)
    {
        if (!cue || !cue.clip || src == null) return;
        src.clip = cue.clip; src.pitch = cue.basePitch; src.volume = 0f;
        src.outputAudioMixerGroup = cue.output ? cue.output : musicBus; src.Play();
    }
    public void SetStemLevel(string name, float targetVol01)
    {
        AudioSource src = name=="DrumsLow"?aDrumsLow: name=="DrumsHigh"?aDrumsHigh: name=="MelodicA"?aMelA: name=="MelodicB"?aMelB:null;
        if (src) StartCoroutine(FadeTo(src, Mathf.Clamp01(targetVol01)));
    }
    IEnumerator FadeTo(AudioSource src, float target, float? seconds=null)
    {
        float dur = seconds ?? fadeSeconds; float t=0f; float start = src.volume;
        while (t<dur){ t+=Time.deltaTime; src.volume = Mathf.Lerp(start, target, Mathf.Clamp01(t/dur)); yield return null; }
        src.volume = target;
    }
}
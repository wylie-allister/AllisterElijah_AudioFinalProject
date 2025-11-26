using UnityEngine;
using UnityEngine.Audio;
/* ... (same as previous annotated AudioCue code) ... */
[CreateAssetMenu(fileName = "NewAudioCue", menuName = "Audio/Audio Cue")]
public class AudioCue : ScriptableObject
{
    [Header("Clip & Playback")]
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
    [Header("Pitch Controls")]
    public float basePitch = 1f;
    public float randomSemitoneRange = 0f;
    [Header("Mixer Routing")]
    public AudioMixerGroup output;
    public float GetRandomizedPitch()
    {
        if (randomSemitoneRange <= 0f) return basePitch;
        float semi = Random.Range(-randomSemitoneRange, randomSemitoneRange);
        float mul = Mathf.Pow(2f, semi / 12f);
        return basePitch * mul;
    }
}
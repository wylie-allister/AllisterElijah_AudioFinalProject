using UnityEngine;
/* ... (same as previous annotated NPCCarAudioController code) ... */
[RequireComponent(typeof(AudioSource))]
public class NPCCarAudioController : MonoBehaviour
{
    public AudioCue engineCue; public Transform player; public Camera mainCam;
    public float maxPanAtWorldUnits=8f, minVolume=0.2f, maxVolume=0.6f, maxAudibleDistance=25f;
    public AudioLowPassFilter lowPass; public float offscreenCutoff=1800f, onscreenCutoff=22000f;
    AudioSource src;
    void Awake(){ src = GetComponent<AudioSource>(); src.loop=true; src.spatialBlend=0f; if (engineCue) AudioManager.Instance?.PlayLoopOn(engineCue, src, 0f); }
    void Update()
    {
        if (!player) return; if (!mainCam) mainCam = Camera.main;
        float dx = transform.position.x - player.position.x; src.panStereo = Mathf.Clamp(dx/Mathf.Max(0.01f,maxPanAtWorldUnits), -1f, 1f);
        float dist = Vector2.Distance(transform.position, player.position);
        float t = 1f - Mathf.Clamp01(dist/Mathf.Max(0.01f, maxAudibleDistance));
        src.volume = Mathf.Lerp(minVolume, maxVolume, t);
        if (lowPass && mainCam){ bool visible = IsVisibleInViewport(); lowPass.cutoffFrequency = visible? onscreenCutoff: offscreenCutoff; }
    }
    bool IsVisibleInViewport(){ Vector3 vp = mainCam.WorldToViewportPoint(transform.position); return vp.z>0 && vp.x>=0 && vp.x<=1 && vp.y>=0 && vp.y<=1; }
}
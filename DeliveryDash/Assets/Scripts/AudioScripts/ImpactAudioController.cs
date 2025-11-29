using UnityEngine;
/* ... (same as previous annotated ImpactAudioController code) ... */
[RequireComponent(typeof(Rigidbody2D))]
public class ImpactAudioController : MonoBehaviour
{
    public AudioCue crashCarCue, crashBoundaryCue;
    public AudioLowPassFilter lowPass;
    public float lightImpactThreshold=2f, lightImpactCutoff=1200f, normalCutoff=22000f;
    public LayerMask trafficMask, boundaryMask;
    void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 pos = (col.contactCount>0)? (Vector3)col.GetContact(0).point : transform.position;
        int other = col.collider.gameObject.layer;
        bool isTraffic = (trafficMask.value & (1<<other))!=0;
        bool isBoundary = (boundaryMask.value & (1<<other))!=0;
        if (isTraffic && crashCarCue) AudioManager.Instance?.PlayAt(crashCarCue, pos, 0.2f, 3f, 20f);
        if (isBoundary && crashBoundaryCue) AudioManager.Instance?.PlayAt(crashBoundaryCue, pos, 0.2f, 3f, 20f);
        if (lowPass){ float rel=col.relativeVelocity.magnitude; lowPass.cutoffFrequency = (rel<=lightImpactThreshold)? lightImpactCutoff: normalCutoff; }
    }
}
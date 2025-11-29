using UnityEngine;
/* ... (same as previous annotated UISoundPlayer code) ... */
public class UISoundPlayer : MonoBehaviour
{
    public AudioCue clickCue, hoverCue;
    public void PlayClick(){ if (clickCue) AudioManager.Instance?.Play2D(clickCue); }
    public void PlayHover(){ if (hoverCue) AudioManager.Instance?.Play2D(hoverCue); }
}
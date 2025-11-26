using UnityEngine;

public class SpillDebug : MonoBehaviour
{
    public SpillMeter spill;

    void Start()
    {
        if (!spill) spill = FindObjectOfType<SpillMeter>();
        if (spill) spill.Increase(25f);   // should bump the UI to ~25%
        else Debug.LogWarning("SpillDebug: no SpillMeter found in the scene.");
    }
}

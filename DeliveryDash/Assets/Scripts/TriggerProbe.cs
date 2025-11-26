using UnityEngine;
/// Attach to a trigger to log when something enters/stays/exits (for debugging). 
public class TriggerProbe : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D o){ Debug.Log($"[TriggerProbe] ENTER {name} by {o.name} tag={o.tag}"); }
    void OnTriggerStay2D(Collider2D o) { Debug.Log($"[TriggerProbe] STAY  {name} by {o.name} tag={o.tag}"); }
    void OnTriggerExit2D(Collider2D o) { Debug.Log($"[TriggerProbe] EXIT  {name} by {o.name} tag={o.tag}"); }
}

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// Holds an ordered list of waypoints for traffic to follow.
/// Put this on an empty "Path_*" object; add child empties P0..Pn as waypoints.
public class TrafficPath : MonoBehaviour
{
    public Transform[] points;

    [Tooltip("If true, draws path gizmos in Scene view.")]
    public bool drawGizmos = true;

    [Tooltip("Gizmo color for this path.")]
    public Color gizmoColor = new Color(1f, 0.85f, 0.2f, 1f);

#if UNITY_EDITOR
    [ContextMenu("Auto-collect children as points (sorted by name)")]
    void AutoCollect()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++)
            points[i] = transform.GetChild(i);
        // Sort by name so P0,P1,P2… works
        System.Array.Sort(points, (a,b)=> string.CompareOrdinal(a.name, b.name));
        EditorUtility.SetDirty(this);
    }
#endif

    public int Count => points != null ? points.Length : 0;
    public Transform this[int i] => points[i];

    void OnDrawGizmos()
    {
        if (!drawGizmos || points == null || points.Length < 2) return;
        Gizmos.color = gizmoColor;

        for (int i = 0; i < points.Length; i++)
        {
            var p = points[i];
            if (!p) continue;
            Gizmos.DrawSphere(p.position, 0.06f);
            if (i < points.Length - 1 && points[i+1])
                Gizmos.DrawLine(p.position, points[i+1].position);
        }
    }
}

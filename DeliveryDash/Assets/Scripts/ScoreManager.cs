using UnityEngine;
using TMPro;

/// Centralized point tracking.
public class ScoreManager : MonoBehaviour
{
    public int points = 0;
    [Header("HUD")]
    public TMP_Text pointsText;    // Canvas/HUD/PointsText

    public void Add(int amount)
    {
        points += amount;
        Refresh();
    }

    public void Subtract(int amount)
    {
        points -= amount;
        if (points < 0) points = 0;
        Refresh();
    }

    void Refresh()
    {
        if (pointsText) pointsText.text = $"Points: {points}";
    }
}

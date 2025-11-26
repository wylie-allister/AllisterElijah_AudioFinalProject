using UnityEngine;

/// <summary>
/// Central place to register successful deliveries.
/// Bumps the delivery count, updates HUD, awards points, and shows a temporary message.
/// </summary>
public class DeliveryManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("HUDController on Canvas/HUD – used to update delivery count & show messages.")]
    [SerializeField] private HUDController hud;

    [Tooltip("ScoreManager on Systems – used to award points for deliveries.")]
    [SerializeField] private ScoreManager score;

    [Header("State (read-only at runtime)")]
    [SerializeField] private int deliveries = 0;

    /// <summary>
    /// Call this from your trigger script (e.g., HomeZone) when the player presses Enter inside a home trigger.
    /// </summary>
    public void RegisterDelivery()
    {
        deliveries++;
        if (hud) hud.SetDeliveries(deliveries);

        // Points: +50 per delivery
        if (score) score.Add(50);

        // Show a 5s confirmation message (auto-clears even if spammed)
        if (hud) hud.ShowMessage("Delivery made", 5f);
    }

    /// <summary>Optional helper to read the current delivery count.</summary>
    public int CurrentDeliveries => deliveries;

    // If you prefer wiring in Inspector:
    // - Drag Canvas/HUD (with HUDController) to 'hud'
    // - Drag Systems/ScoreManager to 'score'
}

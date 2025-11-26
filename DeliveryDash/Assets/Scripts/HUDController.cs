using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Lightweight HUD updater:
/// - Delivery counter
/// - Timer text
/// - Short collision/warning text (auto clears)
/// - General message text (auto clears)
/// Uses single coroutines so repeated calls reset the timer cleanly.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Text References (TMP)")]
    [SerializeField] private TMP_Text counterText;     // "Deliveries: 0"
    [SerializeField] private TMP_Text timerText;       // "Time: 01:00"
    [SerializeField] private TMP_Text collisionText;   // short warnings; clears fast
    [SerializeField] private TMP_Text messageText;     // e.g., "Delivery made"; clears after N seconds

    // Internal coroutine handles so new messages cancel the prior one
    private Coroutine messageRoutine;
    private Coroutine collisionRoutine;

    #region Public API (called by other systems)

    /// <summary>Set the visible delivery count, e.g., after a successful delivery.</summary>
    public void SetDeliveries(int count)
    {
        if (counterText) counterText.text = $"Deliveries: {count}";
    }

    /// <summary>Set the countdown display from a seconds value, e.g., GameManager updates this each tick.</summary>
    public void SetTimer(float seconds)
    {
        if (!timerText) return;
        seconds = Mathf.Max(0f, seconds);
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"Time: {mins:00}:{secs:00}";
    }

    /// <summary>Show a short collision/warning text that auto-clears quickly.</summary>
    public void ShowCollision(string text, float seconds = 1.2f)
    {
        if (collisionRoutine != null) StopCoroutine(collisionRoutine);
        collisionRoutine = StartCoroutine(ShowCollisionRoutine(text, seconds));
    }

    /// <summary>Show a general message (e.g., delivery made) that auto-clears. New messages reset the timer.</summary>
    public void ShowMessage(string text, float seconds = 5f)
    {
        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(ShowMessageRoutine(text, seconds));
    }

    #endregion

    #region Routines

    private IEnumerator ShowCollisionRoutine(string text, float seconds)
    {
        if (collisionText) collisionText.text = text;
        yield return new WaitForSeconds(seconds);
        if (collisionText) collisionText.text = string.Empty;
        collisionRoutine = null;
    }

    private IEnumerator ShowMessageRoutine(string text, float seconds)
    {
        if (messageText) messageText.text = text;
        yield return new WaitForSeconds(seconds);
        if (messageText) messageText.text = string.Empty;
        messageRoutine = null;
    }

    #endregion

    // Wire these in the Inspector:
    // - counterText  ← Canvas/HUD/CounterText
    // - timerText    ← Canvas/HUD/TimerText
    // - collisionText← Canvas/HUD/CollisionText
    // - messageText  ← Canvas/HUD/MessageText
}

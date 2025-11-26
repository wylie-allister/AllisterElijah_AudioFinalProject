using System;

/// <summary>
/// LIGHTWEIGHT EVENT HUB so systems can communicate without hard references.
/// - HUDController subscribes to update texts.
/// - GameManager pushes timer updates.
/// - DeliveryManager announces delivery changes.
/// - SpillMeter announces collision prompts.
/// </summary>
public static class GameEvents
{
    public static event Action<string> OnCollisionPrompt;
    public static void RaiseCollision(string msg) => OnCollisionPrompt?.Invoke(msg);

    public static event Action<int> OnDeliveriesChanged;
    public static void RaiseDeliveries(int count) => OnDeliveriesChanged?.Invoke(count);

    public static event Action<float> OnTimerChanged;
    public static void RaiseTimer(float seconds) => OnTimerChanged?.Invoke(seconds);

    public static event Action<string> OnMessage;
    public static void RaiseMessage(string msg) => OnMessage?.Invoke(msg);
}

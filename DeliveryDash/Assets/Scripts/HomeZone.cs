using UnityEngine;

/// <summary>
/// HomeZone
/// - Place on each Home (requires BoxCollider2D with Is Trigger = ON).
/// - When the Player (tagged 'Player') enters the trigger, we set a flag.
/// - While inside, pressing Enter (or Keypad Enter) calls DeliveryManager.RegisterDelivery().
/// - Writes Debug.Logs when entering/exiting the zone and when delivery is triggered.
/// </summary>
public class HomeZone : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag Systems/DeliveryManager here.")]
    [SerializeField] private DeliveryManager deliveryManager;

    [Header("Input")]
    [Tooltip("Primary key to confirm delivery.")]
    [SerializeField] private KeyCode deliverKey = KeyCode.Return;

    [Tooltip("Also accept keypad Enter.")]
    [SerializeField] private bool acceptKeypadEnter = true;

    private bool playerInside = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log($"[HomeZone:{name}] Player ENTERED delivery zone.");
            // Optional: give a brief prompt via HUD
            FindObjectOfType<HUDController>()?.ShowCollision("Press Enter to deliver", 1.5f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log($"[HomeZone:{name}] Player EXITED delivery zone.");
        }
    }

    void Update()
    {
        if (!playerInside || deliveryManager == null) return;

        bool pressed = Input.GetKeyDown(deliverKey) ||
                       (acceptKeypadEnter && Input.GetKeyDown(KeyCode.KeypadEnter));

        if (pressed)
        {
            Debug.Log($"[HomeZone:{name}] Delivery CONFIRMED (Enter pressed).");
            deliveryManager.RegisterDelivery(); // +50 points, HUD update, 5s message handled by DeliveryManager
        }
    }
}

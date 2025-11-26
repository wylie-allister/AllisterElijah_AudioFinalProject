using UnityEngine;
using UnityEngine.UI;

/// Tracks spill value, auto-recovers over time, and updates a Slider + fill Image.
/// Public methods: AddSpill(float), Add(float), Increase(float) all do the same.
public class SpillMeter : MonoBehaviour
{
    [Header("UI")]
    public Slider spillSlider;          // assign your vertical Slider ("SpillSlider")
    public Image spillFillImage;        // assign the Fill Image (for color)

    [Header("Spill Logic")]
    public float maxSpill = 100f;
    public float recoverPerSecond = 8f;   // how fast it calms down
    public float clampMin = 0f;

    [Header("Coloring")]
    public Gradient colorBySpill;       // optional; else fixed color
    public Color fallbackColor = Color.yellow;

    [Header("Debug")]
    [SerializeField] private float spillValue = 0f;

    public float Value => spillValue;

    void Start()
    {
        if (spillSlider)
        {
            spillSlider.minValue = clampMin;
            spillSlider.maxValue = maxSpill;
            spillSlider.value = spillValue;
        }
        ApplyColor();
    }

    void Update()
    {
        // passive recovery
        if (spillValue > clampMin)
        {
            spillValue = Mathf.Max(clampMin, spillValue - recoverPerSecond * Time.deltaTime);
            PushToUI();
        }
    }

    void PushToUI()
    {
        if (spillSlider) spillSlider.value = spillValue;
        ApplyColor();
    }

    void ApplyColor()
    {
        if (!spillFillImage) return;
        if (colorBySpill != null)
        {
            float t = Mathf.InverseLerp(clampMin, maxSpill, spillValue);
            spillFillImage.color = colorBySpill.Evaluate(t);
        }
        else spillFillImage.color = fallbackColor;
    }

    // -------- API (any of these work; collisions script uses reflection) --------
    public void AddSpill(float amount) => Add(amount);
    public void Add(float amount)      => Increase(amount);
    public void Increase(float amount)
    {
        if (amount <= 0f) return;
        spillValue = Mathf.Min(maxSpill, spillValue + amount);
        PushToUI();
    }

    // Optional helpers if you want external scripts to set/clear
    public void Set(float v)
    {
        spillValue = Mathf.Clamp(v, clampMin, maxSpill);
        PushToUI();
    }
    public void Clear() => Set(0f);
}

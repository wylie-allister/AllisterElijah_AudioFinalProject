using UnityEngine;
using UnityEngine.SceneManagement;

/// Controls game timer, pause menu, and game-over flow.
/// - Counts down from Start Seconds -> 0
/// - Updates HUD timer text
/// - Pause menu shows/hides and freezes time
/// - Game Over panel shows at 0 and freezes time
public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("Seconds to start each run with.")]
    public float startSeconds = 60f;

    [Tooltip("Current timer (read-only at runtime).")]
    [SerializeField] private float currentSeconds;

    [Header("UI References")]
    [Tooltip("Canvas/HUD object with HUDController.")]
    public HUDController hud;

    [Tooltip("Pause Menu panel (set inactive by default).")]
    public GameObject pauseMenuPanel;

    [Tooltip("Game Over panel (set inactive by default).")]
    public GameObject gameOverPanel;

    [Header("State")]
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isGameOver = false;

    void Start()
    {
        // Initialize time and UI panels
        currentSeconds = Mathf.Max(0f, startSeconds);
        if (hud != null) hud.SetTimer(currentSeconds);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Make sure the game actually runs time
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
    }

    void Update()
    {
        // Do not tick the timer when paused or at game over
        if (isPaused || isGameOver) return;

        // Count down with scaled time so pausing (timeScale=0) stops the timer
        currentSeconds -= Time.deltaTime;
        if (currentSeconds < 0f) currentSeconds = 0f;

        // Push to HUD
        if (hud != null) hud.SetTimer(currentSeconds);

        // Reached zero? Trigger game over once
        if (currentSeconds <= 0.0001f && !isGameOver)
        {
            EnterGameOver();
        }
    }

    // === Public UI Hooks (wire these to buttons) ===

    /// Called by the in-game Menu button.
    public void OnPressMenu()
    {
        if (isGameOver) return; // ignore menu at game over
        SetPaused(true);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
    }

    /// Resume from pause.
    public void OnResume()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        SetPaused(false);
    }

    /// Restart the current scene.
    public void OnRestart()
    {
        // Always unpause time before reload
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    /// Quit the application (no-op in Editor).
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // === Internals ===

    private void EnterGameOver()
    {
        isGameOver = true;
        SetPaused(true); // freeze game
        if (gameOverPanel) gameOverPanel.SetActive(true);
        // Optional: hud.ShowMessage("Time's up!", 3f);
    }

    private void SetPaused(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }

    // In case you want to add/subtract time from elsewhere:
    public void AddTime(float seconds)
    {
        currentSeconds = Mathf.Clamp(currentSeconds + seconds, 0f, 9999f);
        if (hud != null) hud.SetTimer(currentSeconds);
    }
}

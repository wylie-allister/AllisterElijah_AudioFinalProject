using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pausePanel;           // your Pause/Menu panel
    public MenuAudioController menuAudio;   // drag the component here

    bool isPaused;

    void Update()
    {
        // Toggle pause on Escape (or your key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) OpenPause();
            else ClosePause();
        }
    }

    public void OpenPause()
    {
        isPaused = true;
        Time.timeScale = 0f;                   // pause gameplay logic
        pausePanel.SetActive(true);
        if (menuAudio) menuAudio.OnMenuOpened(); // fade Ambience down (unscaled time)
    }

    public void ClosePause()
    {
        isPaused = false;
        Time.timeScale = 1f;                   // resume gameplay logic
        pausePanel.SetActive(false);
        if (menuAudio) menuAudio.OnMenuClosed(); // fade Ambience back up
    }

    // Buttons
    public void OnResumeButton() => ClosePause();

    public void OnRestartButton()
    {
        Time.timeScale = 1f; // ensure normal time before reload
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicIntensityDriver : MonoBehaviour
{

    [Header("Assign in Inspector")]
    public MusicStemController music;
    public float totalTime = 60f;     // match your game timer
    public float timeLeft = 60f;      // drive this from your actual timer

    [Header("Thresholds (seconds)")]
    public float addDrumsHighAt = 30f;
    public float addMelodicBAt = 20f;
    public float fullBlastAt = 10f;
    public bool isGO = false;


    void Start()
    {
        // Start base layers on
        if (music)
        {
            music.SetStemLevel("DrumsLow", 0.9f);
            music.SetStemLevel("MelodicA", 0.8f);
            // others start at 0 by default
        }
    }

    void Update()
    {
        // Replace with your real timer update:
        timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);

        if (!music) return;

        if (timeLeft <= 1f)
        {
            isGO = true;
        }

        if (isGO == false)
        {
            // Bring in DrumsHigh as time drops below threshold
            if (timeLeft < addDrumsHighAt)
                music.SetStemLevel("DrumsHigh", 0.7f);

            // Bring in MelodicB later
            if (timeLeft < addMelodicBAt)
                music.SetStemLevel("MelodicB", 0.7f);

            // Final push: everything strong in last seconds
            if (timeLeft < fullBlastAt)
            {
                music.SetStemLevel("DrumsHigh", 1.0f);
                music.SetStemLevel("MelodicB", 1.0f);
                // Optionally push base layers too:
                music.SetStemLevel("DrumsLow", 0.9f);
                music.SetStemLevel("MelodicA", 0.8f);
            }
        }
        else
        {
            StopMusic();
        }


    }

    public void StopMusic()
    {
        
        music.SetStemLevel("DrumsLow", 0f);
        music.SetStemLevel("DrumsHigh", 0f);
        music.SetStemLevel("MelodicA", 1f);
        music.SetStemLevel("MelodicB", 0f);
    }
}


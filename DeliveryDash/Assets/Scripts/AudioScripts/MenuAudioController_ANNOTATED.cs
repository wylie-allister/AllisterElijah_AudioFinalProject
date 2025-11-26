using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
/* ... (same as previous annotated MenuAudioController code) ... */
public class MenuAudioController : MonoBehaviour
{
    public AudioMixer mixer; public string ambienceParam="AmbienceVol_dB";
    public float menuDb=-40f, gameDb=0f, fadeTime=0.5f; Coroutine current;
    public void OnMenuOpened()=>FadeTo(menuDb); public void OnMenuClosed()=>FadeTo(gameDb);
    void FadeTo(float targetDb){ if (current!=null) StopCoroutine(current); current=StartCoroutine(FadeMixerParam(targetDb)); }
    IEnumerator FadeMixerParam(float targetDb){ mixer.GetFloat(ambienceParam, out float startDb); float t=0f;
        while (t<fadeTime){ t+=Time.unscaledDeltaTime; mixer.SetFloat(ambienceParam, Mathf.Lerp(startDb,targetDb,t/fadeTime)); yield return null; }
        mixer.SetFloat(ambienceParam, targetDb); current=null;
    }
}
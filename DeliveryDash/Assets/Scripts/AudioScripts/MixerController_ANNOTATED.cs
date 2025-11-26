using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/* ... (enhanced MixerController with master/music/sfx/ambience/ui sliders + PlayerPrefs) ... */
public class MixerController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider masterSlider, musicSlider, sfxSlider, ambienceSlider, uiSlider;
    public string masterParam="MasterVol_dB", musicParam="MusicVol_dB", sfxParam="SFXVol_dB", ambienceParam="AmbienceVol_dB", uiParam="UIVol_dB";
    public float minDb=-40f, maxDb=0f;
    public bool saveToPrefs=true; public string prefsPrefix="Audio_";
    void Start()
    {
        if (masterSlider) masterSlider.onValueChanged.AddListener(v=>SetParam01(masterParam,v));
        if (musicSlider) musicSlider.onValueChanged.AddListener(v=>SetParam01(musicParam,v));
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(v=>SetParam01(sfxParam,v));
        if (ambienceSlider) ambienceSlider.onValueChanged.AddListener(v=>SetParam01(ambienceParam,v));
        if (uiSlider) uiSlider.onValueChanged.AddListener(v=>SetParam01(uiParam,v));
        LoadOrSync();
    }
    void LoadOrSync()
    {
        TrySync(masterSlider, masterParam); TrySync(musicSlider, musicParam);
        TrySync(sfxSlider, sfxParam); TrySync(ambienceSlider, ambienceParam); TrySync(uiSlider, uiParam);
    }
    void TrySync(Slider s, string param)
    {
        if (!s) return;
        if (saveToPrefs && PlayerPrefs.HasKey(prefsPrefix+param)) s.value = PlayerPrefs.GetFloat(prefsPrefix+param, s.value);
        else if (mixer.GetFloat(param, out float db)) s.value = Mathf.InverseLerp(minDb, maxDb, db);
        SetParam01(param, s.value);
    }
    void SetParam01(string param, float v01)
    {
        if (string.IsNullOrEmpty(param) || mixer==null) return;
        float db = Mathf.Lerp(minDb, maxDb, Mathf.Clamp01(v01)); mixer.SetFloat(param, db);
        if (saveToPrefs) PlayerPrefs.SetFloat(prefsPrefix+param, v01);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBootstrap : MonoBehaviour

{
    public MusicStemController music;
    void Start() { if (music) music.StartAll(); }
}

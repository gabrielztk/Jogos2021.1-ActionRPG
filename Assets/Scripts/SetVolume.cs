using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour {

    public AudioMixer mixer;
    public string mixer_group;

    public void SetLevel (float sliderValue)
    {
        mixer.SetFloat(mixer_group, Mathf.Log10(sliderValue) * 20);
    }
}
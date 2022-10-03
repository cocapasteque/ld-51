using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider musicSlider;
    public Slider uiSlider;
    public Slider sfxSlider;
    
    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat("Music", musicSlider.value);
        mixer.SetFloat("UI", uiSlider.value);
        mixer.SetFloat("Sfx", sfxSlider.value);
    }
}

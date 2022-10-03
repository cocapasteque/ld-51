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

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music", 12);
        uiSlider.value = PlayerPrefs.GetFloat("UI", -2);
        sfxSlider.value = PlayerPrefs.GetFloat("Sfx", -2);
    }
    
    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat("Music", musicSlider.value);
        mixer.SetFloat("UI", uiSlider.value);
        mixer.SetFloat("Sfx", sfxSlider.value);
        
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("UI", uiSlider.value);
        PlayerPrefs.SetFloat("Sfx", sfxSlider.value);
    }
}

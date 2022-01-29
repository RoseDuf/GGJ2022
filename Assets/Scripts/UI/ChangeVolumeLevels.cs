using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolumeLevels : MonoBehaviour
{
    private Slider _slider;
    private float _masterVolume;
    private float _musicVolume;
    private float _soundVolume;

    private void Start()
    {
        _slider = this.GetComponent<Slider>();
    }

    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = _slider.value;

        if (whatValue == "Master")
        {
            _masterVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("MasterVolume", _masterVolume);
        }
        if (whatValue == "Music")
        {
            _musicVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", _musicVolume);
        }
        if (whatValue == "Sound")
        {
            _soundVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("SoundVolume", _soundVolume);
        }
    }
}

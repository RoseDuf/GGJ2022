using Game;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolumeLevels : MonoBehaviour
{
    public enum SoundType {Master, Music, Sfx}

    [SerializeField] private SoundType _type;
    [SerializeField] private Slider _slider;
    private float _masterVolume;
    private float _musicVolume;
    private float _soundVolume;

    private bool _wasInitialized = false;

    private void Start()
    {
        if (_slider == null)
            _slider = GetComponent<Slider>();
        
        switch (_type)
        {
            case SoundType.Master:
                _slider.value = SoundSystem.Instance.GetMasterVolume();
                break;
            case SoundType.Music:
                _slider.value = SoundSystem.Instance.GetMusicVolume();
                break;
            case SoundType.Sfx:
                _slider.value = SoundSystem.Instance.GetSfxVolume();
                break;
        }

        _wasInitialized = true;
    }

    public void SetSpecificVolume(string whatValue)
    {
        if (!_wasInitialized)
            return;
        
        float sliderValue = _slider.value;

        if (whatValue == "Master")
        {
            _masterVolume = _slider.value;
            SoundSystem.Instance.SetMasterVolume(_masterVolume);
        }
        if (whatValue == "Music")
        {
            _musicVolume = _slider.value;
            SoundSystem.Instance.SetMusicVolume(_musicVolume);
        }
        if (whatValue == "Sound")
        {
            _soundVolume = _slider.value;
            SoundSystem.Instance.SetSfxVolume(_soundVolume);
        }
    }
}

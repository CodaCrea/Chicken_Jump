using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSource;
    public AudioClip[] sounds;
    public AudioMixerGroup[] mixerSounds;
    public Slider sliderMusic, sliderSfx;
    public Toggle toggleMusic, toggleSfx;
    [HideInInspector]
    public int numberSoundLevel = 0, numberSoundLose = 1, numberSoundEnd = 2;
    [HideInInspector]
    public int numberSfxClick = 0, numberSfxJump = 1, numberSfxTakeShield = 2, numberSfxBoost = 3, numberSfxReduction = 4, numberMusic = 5, numberSfx = 6;

    private float _sliderValue, _sliderValueClamp, _sliderDB;
    private bool _isSoundJump = false;

    private void Start()
    {
        LoadAudioSettings();
        if (numberSoundLevel < audioSource.Length)
        {
            audioSource[numberSoundLevel].Play();
        }
    }

    private void OnApplicationQuit()
    {
        RemoveSaves();
    }

    public AudioSource PlaySounds(AudioClip audioEffect, AudioMixerGroup soundMixer, Vector3 audioPosition)
    {
        GameObject gameObjectSound = new("AudioTemp");
        gameObjectSound.transform.position = audioPosition;
        AudioSource audioSource = gameObjectSound.AddComponent<AudioSource>();
        audioSource.clip = audioEffect;
        audioSource.outputAudioMixerGroup = soundMixer;
        audioSource.Play();
        Destroy(gameObjectSound, audioEffect.length);
        return audioSource;
    }

    public void ToggleMusic()
    {
        ToggleSound(toggleMusic, sliderMusic, mixerSounds[numberMusic], "Musics");
    }

    public void ToggleSfx()
    {
        ToggleSound(toggleSfx, sliderSfx, mixerSounds[numberSfx], "SFX");
        PlaySounds(sounds[numberSfxJump], mixerSounds[numberSfxJump], transform.position);
    }

    public void SliderMusic()
    {
        SliderSound(toggleMusic, sliderMusic, mixerSounds[numberMusic], "Musics");
    }

    public void SliderSfx()
    {
        SliderSound(toggleSfx, sliderSfx, mixerSounds[numberSfx], "SFX");

        if (!_isSoundJump)
        {
            _isSoundJump = true;
            PlaySounds(sounds[numberSfxJump], mixerSounds[numberSfxJump], transform.position);
            StartCoroutine(TimerSoundEffect());
        }
    }

    private IEnumerator TimerSoundEffect()
    {
        float seconds = 0.3f;

        yield return new WaitForSeconds(seconds);
        _isSoundJump = false;
    }

    // Mute du volume en utilisant "AudioMixerGroup" pour séparer la musique et le son
    private void ToggleSound(Toggle toggle, Slider slider, AudioMixerGroup audioMixer, string name)
    {
        _sliderValue = slider.value;
        _sliderValueClamp = Mathf.Clamp01(_sliderValue);
        _sliderDB = Mathf.Log10(_sliderValueClamp) * 20;

        if (_sliderValueClamp == 0)
        {
            _sliderDB = -80.0f;
        }

        if (toggle.isOn)
        {
            audioMixer.audioMixer.SetFloat(name, -80.0f);
        }
        else
        {
            audioMixer.audioMixer.SetFloat(name, _sliderDB);
        }
    }

    //  Gestion du volume en utilisant "AudioMixerGroup" pour séparer la musique et le son
    private void SliderSound(Toggle toggle, Slider slider, AudioMixerGroup audioMixer, string name)
    {
        _sliderValue = slider.value;
        _sliderValueClamp = Mathf.Clamp01(_sliderValue);
        _sliderDB = Mathf.Log10(_sliderValueClamp) * 20;

        if (_sliderValueClamp == 0)
        {
            _sliderDB = -80.0f;
        }

        if (!toggle.isOn)
        {
            audioMixer.audioMixer.SetFloat(name, _sliderDB);
        }
    }

    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("SliderMusicValue", sliderMusic.value);
        PlayerPrefs.SetFloat("SliderSfxValue", sliderSfx.value);
        PlayerPrefs.SetInt("ToggleMusicState", toggleMusic.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ToggleSfxState", toggleSfx.isOn ? 1 : 0);
        Debug.Log("Sauvegarde");
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("SliderMusicValue"))
        {
            float sliderMusicValue = PlayerPrefs.GetFloat("SliderMusicValue");
            sliderMusic.value = sliderMusicValue;
            Debug.Log("Charge Music");
        }

        if (PlayerPrefs.HasKey("SliderSfxValue"))
        {
            float sliderSfxValue = PlayerPrefs.GetFloat("SliderSfxValue");
            sliderSfx.value = sliderSfxValue;
            Debug.Log("Charge Sfx");
        }

        if (PlayerPrefs.HasKey("ToggleMusicState"))
        {
            int toggleMusicState = PlayerPrefs.GetInt("ToggleMusicState");
            toggleMusic.isOn = toggleMusicState == 1;
            Debug.Log("Charge Toggle Music");
        }

        if (PlayerPrefs.HasKey("ToggleSfxState"))
        {
            int toggleSfxState = PlayerPrefs.GetInt("ToggleSfxState");
            toggleSfx.isOn = toggleSfxState == 1;
            Debug.Log("Charge Toggle Sfx");
        }
    }

    private void RemoveSaves()
    {
        PlayerPrefs.DeleteKey("SliderMusicValue");
        PlayerPrefs.DeleteKey("SliderSfxValue");
        PlayerPrefs.DeleteKey("ToggleMusicState");
        PlayerPrefs.DeleteKey("ToggleSfxState");
    }
}

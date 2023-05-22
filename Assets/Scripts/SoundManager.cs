using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst;

    public const string SOUND_ON_PPS = "soundOn";
    public const string SOUND_VOL_PPS = "soundVol";

    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private Sounds _soundsCollection;

    [Header("Runtime")]
    [SerializeField] private float _soundVol;
    [SerializeField] private bool _soundOn;

    public float SoundVol { get { return _soundVol; } private set { _soundVol = Mathf.Clamp01(value); PlayerPrefs.SetFloat(SOUND_VOL_PPS, value); } }
    public bool SoundOn { get { return _soundOn; } private set { _soundOn = value;  PlayerPrefs.SetInt(SOUND_ON_PPS, (value ? 1 : 0)); } }

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);

        Initialize();
        SetAudioSourcesVolumes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSoundVolume(float volume)
    {
        SoundVol = volume;
        if (SoundOn)
        {
            _musicSource.volume = volume;
            _soundSource.volume = volume;
        }        
    }

    public void ToggleMusic()
    {
        SoundOn = !SoundOn;
        SetAudioSourcesVolumes();
        
    }

    private void SetAudioSourcesVolumes()
    {
        if (SoundOn == false)
        {
            _musicSource.volume = 0;
            _soundSource.volume = 0;
        }
        else
        {
            _musicSource.volume = SoundVol;
            _soundSource.volume = SoundVol;
        }
    }

    public void PlayDefaultButtonSound()
    {
        _soundSource.clip = _soundsCollection.DefaultButtonSound;
        _soundSource.Play();
    }

    private void Initialize()
    {
        _soundOn = PlayerPrefs.GetInt(SOUND_ON_PPS, 1) == 1;
        _soundVol = PlayerPrefs.GetFloat(SOUND_VOL_PPS, 1);
    }
}

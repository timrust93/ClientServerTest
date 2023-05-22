using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst;

    public const string SOUND_ON_PPS = "soundOn";
    public const string SOUND_VOL_PPS = "soundVol";

    [Header("Runtime")]
    [SerializeField] private float _soundVol;
    [SerializeField] private bool _soundOn;

    public float SoundVol { get { return _soundVol; } set { _soundVol = Mathf.Clamp01(value); PlayerPrefs.SetFloat(SOUND_VOL_PPS, value); } }
    public bool SoundOn { get { return _soundOn; } set { _soundOn = value;  PlayerPrefs.SetInt(SOUND_ON_PPS, (value ? 1 : 0)); } }

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        _soundOn = PlayerPrefs.GetInt(SOUND_ON_PPS, 1) == 1;
        _soundVol = PlayerPrefs.GetInt(SOUND_VOL_PPS, 1);
    }
}

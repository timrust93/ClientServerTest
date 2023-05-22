using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundsSo", fileName = "SoundsSO")]
public class Sounds : ScriptableObject
{
    [SerializeField] private AudioClip _defaultButtonSound;
    [SerializeField] private AudioClip _defaultMusic;

    public AudioClip DefaultButtonSound { get => _defaultButtonSound; private set => _defaultButtonSound = value; }
    public AudioClip DefaultMusic { get => _defaultMusic; private set => _defaultMusic = value; }
}

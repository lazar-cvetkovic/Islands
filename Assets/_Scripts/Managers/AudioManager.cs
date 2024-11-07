using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : SingletonPersistent<AudioManager>
{
    [SerializeField] AudioData[] _soundEffects;
    [SerializeField] AudioData[] _musicTracks;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource _musicSource;

    public float SfxVolume => _sfxSource.volume;
    public float MusicVolume => _musicSource.volume;

    public void PlaySFX(SoundType soundType)
    {
        AudioData sound = Array.Find(_soundEffects, s => s.Name == soundType);

        if (sound == null)
        {
            Debug.LogWarning("Sound not found for sound type: " + soundType);
            return;
        }

        if (_sfxSource == null)
        {
            Debug.LogWarning("AudioSource not assigned to AudioManager.");
            return;
        }

        _sfxSource.PlayOneShot(sound.Clip);
    }

    public void PlaySFX(AudioClip clip) => _sfxSource.PlayOneShot(clip);

    public void PlayMusic(SoundType soundType)
    {
        AudioData sound = Array.Find(_musicTracks, s => s.Name == soundType);
        if (sound == null)
        {
            Debug.LogWarning("Sound not found for sound type: " + soundType);
            return;
        }

        _musicSource.clip = sound.Clip;
        _musicSource.Play();
    }
    public void StopSFX() => _sfxSource.Stop();

    public void ToggleSFX() => _sfxSource.mute = !_sfxSource.mute;

    public void ToggleMusic() => _musicSource.mute = !_musicSource.mute;

    public void ChangeSFXVolume(float volume) => _sfxSource.volume = volume;

    public void ChangeMusicVolume(float volume) => _musicSource.volume = volume;
}

public enum SoundType
{
    MainMusic = 0,
    Hover = 1,
    Click = 2,
    LooseHeart = 3,
    Win = 4,
    LooseFinal = 5
}
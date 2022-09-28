using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return _instance;
        }
    }

    [NonReorderable] public SoundData[] sounds;
    public AudioMixer mixer;
    string currentSongPlaying = "";
    string previousSongPlaying = "";
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        isFirstEnterRoomTime = true;
        foreach (SoundData currentSoundData in sounds)
        {
            currentSoundData.source = gameObject.AddComponent<AudioSource>();
            currentSoundData.source.outputAudioMixerGroup = currentSoundData.output;
            currentSoundData.source.clip = currentSoundData.clip;
            currentSoundData.source.volume = currentSoundData.volume;
            currentSoundData.source.pitch = currentSoundData.pitch;
            currentSoundData.source.loop = currentSoundData.loop;
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            mixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFX"))
        {
            mixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        }
    }

    public SoundData GetSoundByName(string p_name)
    {
        SoundData sound = Array.Find(sounds, sound => sound.name == p_name);
        return sound;
    }

    IEnumerator Co_AudioFade()
    {
        SoundData sound;
        string soundName = previousSongPlaying;
        sound = GetSoundByName(soundName);
    
        if (soundName != "")
        {
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(sound.source.DOFade(0, 1.25f));
            fadeOutSequence.Play();
            yield return fadeOutSequence.WaitForCompletion();
        }

        SoundData currentSound;
        string currentSoundName = currentSongPlaying;
        currentSound = GetSoundByName(currentSoundName);
        currentSound.source.Play();
        if (currentSoundName != "")
        {
            Sequence fadeInSequence = DOTween.Sequence();
            fadeInSequence.Append(currentSound.source.DOFade(1, 1.25f));
            fadeInSequence.Play();
            Debug.Log("NEW");
        }

    }

    public void TransitionAudio(string p_oldAudioClip, string p_newAudioClip)
    {
        foreach (SoundData currentSoundData in sounds)
        {
            currentSoundData.source.Stop();
        }
        StartCoroutine(Co_AudioFade(p_oldAudioClip, p_newAudioClip));
    }

    public IEnumerator Co_AudioFade(string p_oldAudioClip, string p_newAudioClip)
    {
        SoundData sound;
        string soundName = p_oldAudioClip;
        sound = GetSoundByName(soundName);
        if (soundName != "")
        {
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(sound.source.DOFade(0, 1.25f));
            fadeOutSequence.Play();
            yield return fadeOutSequence.WaitForCompletion();
        }

        SoundData currentSound;
        string currentSoundName = p_newAudioClip;
        currentSound = GetSoundByName(currentSoundName);
        currentSound.source.Play();
        if (currentSoundName != "")
        {
            Sequence fadeInSequence = DOTween.Sequence();
            fadeInSequence.Append(currentSound.source.DOFade(1, 1.25f));
            fadeInSequence.Play();
        }
    }
}

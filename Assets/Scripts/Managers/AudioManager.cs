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

    [NonReorderable] public SoundData[] soundEffects;
    [NonReorderable] public SoundData[] backgroundMusics;
    public AudioMixer mixer;
    string currentSongPlaying = "";
    string previousSongPlaying = "";
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
      
        foreach (SoundData currentSoundData in soundEffects)
        {
            currentSoundData.source = gameObject.AddComponent<AudioSource>();
            currentSoundData.source.outputAudioMixerGroup = currentSoundData.output;
            currentSoundData.source.clip = currentSoundData.clip;
            currentSoundData.source.volume = currentSoundData.volume;
            currentSoundData.source.pitch = currentSoundData.pitch;
            currentSoundData.source.loop = currentSoundData.loop;
        }

        foreach (SoundData currentSoundData in backgroundMusics)
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

    public SoundData GetSoundByName(string p_name, bool p_isSoundEffect)
    {
        SoundData[] currentSoundEffects;
        if (p_isSoundEffect)
        {
            currentSoundEffects = soundEffects;
        }
        else
        {
            currentSoundEffects = backgroundMusics;
        }
        SoundData sound = Array.Find(currentSoundEffects, sound => sound.name == p_name);
        return sound;
    }
    public void InstantPlayAudio(string p_oldAudioClip, string p_newAudioClip, bool p_isSoundEffect = true)
    {
        //foreach (SoundData currentSoundData in sounds)
        //{
        //    currentSoundData.source.Stop();
        //}
        //StartCoroutine(Co_AudioFade(p_oldAudioClip, p_newAudioClip));
    }

    public void ForceStopAudio(string p_newAudioClip, bool p_isSoundEffect = true)
    {
        SoundData sound;
        sound = GetSoundByName(p_newAudioClip, p_isSoundEffect);
        if (sound != null)
        {
            Debug.Log("STOPPING " + p_newAudioClip);
            sound.source.Stop();
        }
   

    }

    public void SmoothStopAudio(string p_newAudioClip, bool p_isSoundEffect = true)
    {
        SoundData sound;
        sound = GetSoundByName(p_newAudioClip, p_isSoundEffect);
        StartCoroutine(Co_AudioFade(p_newAudioClip, p_isSoundEffect));

    }
    public void AdditivePlayAudio(string p_newAudioClip, bool p_isSoundEffect = true)
    {
        SoundData sound;
        sound = GetSoundByName(p_newAudioClip, p_isSoundEffect);
        //Debug.Log("PLAYING NEW SONG: " + p_newAudioClip);
        Debug.Log("OUT PLAYING NEW SONG: " + p_newAudioClip);
        if (sound != null)
        {
            Debug.Log("PLAYING NEW SONG: " + p_newAudioClip);
            sound.source.Play();
        }
   
  
    }
    public void SmoothPlayAudio(string p_oldAudioClip, string p_newAudioClip, bool p_isSoundEffect = true)
    {
        SoundData[] currentSoundEffects;
        if (p_isSoundEffect)
        {
            currentSoundEffects = soundEffects;
        }
        else
        {
            currentSoundEffects = backgroundMusics;
        }
        //foreach (SoundData currentSoundData in currentSoundEffects)
        //{
        //    currentSoundData.source.Stop();
        //}
        StartCoroutine(Co_AudioFade(p_oldAudioClip, p_newAudioClip, p_isSoundEffect));
    }

    public IEnumerator Co_AudioFade(string p_oldAudioClip, string p_newAudioClip, bool p_isSoundEffect = true)
    {
        SoundData sound;
        string soundName = p_oldAudioClip;
        sound = GetSoundByName(soundName, p_isSoundEffect);
        Debug.Log("try STOP SONG: " + soundName);
        if (!string.IsNullOrEmpty(soundName))
        {
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(sound.source.DOFade(0, 1.25f));
            fadeOutSequence.Play();
            yield return fadeOutSequence.WaitForCompletion();
            Debug.Log("STOP SONG: " + soundName);
            if (sound!=null)
            {
                sound.source.Stop();
            }

        }
       
        SoundData currentSound;
        string currentSoundName = p_newAudioClip;
        currentSound = GetSoundByName(currentSoundName, p_isSoundEffect);
        Debug.Log("PLAYING NEW SONG: " + currentSound);
        if (currentSound != null)
        {
            currentSound.source.Play();
        }

        if (!string.IsNullOrEmpty(currentSoundName))
        {
            Debug.Log("PLAYING NEW SONG: " + currentSoundName);
            Sequence fadeInSequence = DOTween.Sequence();
          
       
            if (currentSound != null)
            {
                fadeInSequence.Append(currentSound.source.DOFade(1, 1.25f));
                fadeInSequence.Play();
            }
           
        }
    }
    public IEnumerator Co_AudioFade(string p_oldAudioClip, bool p_isSoundEffect = true)
    {
        SoundData sound;
        string soundName = p_oldAudioClip;
        sound = GetSoundByName(soundName, p_isSoundEffect);
        Debug.Log("try STOP SONG: " + soundName);
        if (!string.IsNullOrEmpty(soundName))
        {
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(sound.source.DOFade(0, 1.25f));
            fadeOutSequence.Play();
            yield return fadeOutSequence.WaitForCompletion();
            sound.source.Stop();
        }
     
    }
}

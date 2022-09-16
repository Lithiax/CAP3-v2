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

    private bool isFirstTime = true;
   // private Passageway currentAudioPassageway;
    [NonReorderable] public SoundData[] sounds;
    public AudioMixer mixer;
    string currentSongPlaying = "";
    string previousSongPlaying = "";
    bool isFirstEnterRoomTime = true;
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        isFirstEnterRoomTime = true;
        foreach (SoundData s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.output;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        //PlayerManager.onRoomEnteredEvent.AddListener(PlayOnRoomEnterPassageway);
        //TimeManager.onDayChangingEvent.AddListener(OnDayChangingEvent);
    }

    //private void OnDestroy()
    //{
    //    PlayerManager.onRoomEnteredEvent.RemoveListener(PlayOnRoomEnterPassageway);
    //    TimeManager.onDayChangingEvent.RemoveListener(OnDayChangingEvent);
    //}
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


    //public void OnDayChangingEvent()
    //{
    //    PlayOnRoomEnterPassageway(PlayerManager.instance.startRoomPassageway);
    //}

    public SoundData GetSoundByName(string name)
    {
        SoundData sound = Array.Find(sounds, sound => sound.name == name);
        return sound;
    }

    public void PlayOnRoomEnterPassageway(Passageway p_passageway)
    {
        if (!isFirstTime)
        {
            if (p_passageway.room.roomDescription != currentSongPlaying) 
                isFirstTime = true;
        }
        if (isFirstTime)
        {
            isFirstTime = false;
            previousSongPlaying = currentSongPlaying;
            currentSongPlaying = p_passageway.room.roomDescription;
            StartCoroutine(Co_AudioFade());

            //PlayByName(p_passageway.room.roomDescription);
        }
    }

    public void PlayOnRoomEnterString(string p_passageway)
    {
        if (!isFirstTime)
        {
            if (p_passageway != currentSongPlaying)
                isFirstTime = true;
        }
        if (isFirstTime)
        {
            isFirstTime = false;
            previousSongPlaying = currentSongPlaying;
            currentSongPlaying = p_passageway;
            StartCoroutine(Co_AudioFade());
        }
    }

    IEnumerator Co_AudioFade()
    {
        SoundData sound;
        string soundName = previousSongPlaying;
        sound = GetSoundByName(soundName);
        //Debug.Log("PLAYYYINNG " + soundName);
        if (soundName != "")
        {
            //Debug.Log("REPLACE");
            Sequence wee = DOTween.Sequence();
            wee.Append(sound.source.DOFade(0, 1.25f));
            wee.Play();
            yield return wee.WaitForCompletion();
        }

        SoundData currentSound;
        string currentSoundName = currentSongPlaying;
        currentSound = GetSoundByName(currentSoundName);
        //Debug.Log("NEWWWW PLAYYYINNG " + currentSoundName);
        currentSound.source.Play();
        if (currentSoundName != "")
        {
            Sequence wee2 = DOTween.Sequence();
            wee2.Append(currentSound.source.DOFade(1, 1.25f));
            wee2.Play();
            Debug.Log("NEW");
        }

        // yield return wee2.WaitForCompletion();
    }

    public void StartCoFade(string song1, string song2)
    {
        foreach (SoundData s in sounds)
        {
            s.source.Stop();
        }
        StartCoroutine(Co_AudioFade2(song1, song2));
    }

    public IEnumerator Co_AudioFade2(string song1, string song2)
    {
        SoundData sound;
        string soundName = song1;
        sound = GetSoundByName(soundName);
        if (soundName != "")
        {
            Sequence wee = DOTween.Sequence();
            wee.Append(sound.source.DOFade(0, 1.25f));
            wee.Play();
            yield return wee.WaitForCompletion();
        }

        SoundData currentSound;
        string currentSoundName = song2;
        currentSound = GetSoundByName(currentSoundName);
        currentSound.source.Play();
        if (currentSoundName != "")
        {
            Sequence wee2 = DOTween.Sequence();
            wee2.Append(currentSound.source.DOFade(1, 1.25f));
            wee2.Play();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

[Serializable]
public class audioFiles
{

    public string name;
    public AudioClip audioFile;
    public AudioMixerGroup mixer;
}

public class AudioManager : MonoBehaviour
{

    #region instance

    public static AudioManager Instance { get; private set; }

    #endregion

    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] private List<audioFiles> files = new List<audioFiles>();

    [SerializeField] private Dictionary<string, audioFiles> _items = new Dictionary<string, audioFiles>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        for (int i = 0; i < files.Count; i++)
        {

            _items.Add(files[i].name, files[i]);
        }

        StopAll();
    }

    public void SetAudio(string audio)
    {
        PlayAudio(audio);
    }

    public bool PlayAudio(string audio, float volume = 1)
    {

        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].clip == null || !audioSources[i].isPlaying)
            {
                audioSources[i].clip = _items[audio].audioFile;
                audioSources[i].outputAudioMixerGroup = _items[audio].mixer;
                audioSources[i].volume = volume;
                audioSources[i].pitch = 1;
                audioSources[i].Play();
                return true;
            }
        }
        return false;
    }

    public void PauseSong(string audio)
    {

        for (int i = 0; i < audioSources.Count; i++)
        {

            if (audioSources[i].clip != null && audioSources[i].isPlaying)
            {

                audioSources[i].clip = _items[audio].audioFile;
                audioSources[i].Pause();
            }
        }
    }

    public void StopSong(string audio)
    {

        for (int i = 0; i < audioSources.Count; i++)
        {

            if (audioSources[i].clip != null || audioSources[i].isPlaying)
            {

                audioSources[i].clip = _items[audio].audioFile;
                audioSources[i].Stop();
            }
        }
    }

    public bool PlaySong(string audio, float volume = 1, float multiple = 1)
    {

        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].clip == null || !audioSources[i].isPlaying)
            {
                audioSources[i].clip = _items[audio].audioFile;
                audioSources[i].outputAudioMixerGroup = _items[audio].mixer;
                audioSources[i].volume = volume;
                audioSources[i].pitch = 1;
                audioSources[i].loop = true;
                audioSources[i].Play();
                return true;
            }
        }
        return false;
    }


    public void StopAll()
    {

        for (int i = 0; i < audioSources.Count; i++)
        {

            audioSources[i].Stop();
        }
    }

    public bool IsPlaying(string name)
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].clip == _items[name].audioFile && audioSources[i].isPlaying)
            {
                return true;
            }
        }
        return false;
    }
}

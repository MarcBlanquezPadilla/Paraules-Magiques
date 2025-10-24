using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    bool musicMuted;
    bool sfxMuted;

    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer sfxMixer;

    [SerializeField] GameObject toggleMusic;
    [SerializeField] GameObject toggleSfx;

    private void Start()
    {
        float musicVolume;
        bool success = musicMixer.GetFloat("Volume", out musicVolume);
        if (success)
        {
            bool mutedMusic = musicVolume == -80f;
            toggleMusic.GetComponent<ToggleBehavior>().Toggle(mutedMusic);
        }
             

        float sfxVolume;
        success = musicMixer.GetFloat("Volume", out sfxVolume);
        if (success)
        {
            bool mutedSfx = sfxVolume == -80f;
            toggleMusic.GetComponent<ToggleBehavior>().Toggle(mutedSfx);
        }
    }

    public void MuteMusic(bool m)
    {
        musicMuted = m;
        if (m) musicMixer.SetFloat("Volume", -80f);
        else musicMixer.SetFloat("Volume", 0);
    }

    public void MuteSfx(bool m)
    {
        sfxMuted = m;
        if (m) sfxMixer.SetFloat("Volume", -80f);
        else sfxMixer.SetFloat("Volume", 0);
    }
}

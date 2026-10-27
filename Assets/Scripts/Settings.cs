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
            if (toggleMusic.TryGetComponent<ToggleBehavior>(out ToggleBehavior toggle))
            {
                toggle.Toggle(mutedMusic);
            }
        }
             

        float sfxVolume;
        success = sfxMixer.GetFloat("Volume", out sfxVolume);
        if (success)
        {
            bool mutedSfx = sfxVolume == -80f;
            if (toggleSfx.TryGetComponent<ToggleBehavior>(out ToggleBehavior toggle))
            {
                toggle.Toggle(mutedSfx);
            }
        }

        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            if (PlayerPrefs.GetInt("MusicMuted") == 0)
                MuteMusic(false);
            else
                MuteMusic(true);
        }

        if (PlayerPrefs.HasKey("SFXMuted"))
        {
            if (PlayerPrefs.GetInt("SFXMuted") == 0)
                MuteSfx(false);
            else
                MuteSfx(true);
        }
    }

    public void MuteMusic(bool m)
    {
        musicMuted = m;
        if (m) musicMixer.SetFloat("Volume", -80f);
        else musicMixer.SetFloat("Volume", 0);

        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (toggleMusic.TryGetComponent<ToggleBehavior>(out ToggleBehavior toggle))
        {
            toggle.Toggle(musicMuted);
        }
    }

    public void MuteSfx(bool m)
    {
        sfxMuted = m;
        if (m) sfxMixer.SetFloat("Volume", -80f);
        else sfxMixer.SetFloat("Volume", 0);

        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (toggleSfx.TryGetComponent<ToggleBehavior>(out ToggleBehavior toggle))
        {
            toggle.Toggle(sfxMuted);
        }
    }
}

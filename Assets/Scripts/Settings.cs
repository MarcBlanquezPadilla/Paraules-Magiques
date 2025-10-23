using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    #region Instance

    private static Settings _instance;
    public static Settings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Settings>();
            }
            return _instance;
        }
    }

    #endregion

    bool musicMuted;
    bool sfxMuted;

    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer sfxMixer;

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

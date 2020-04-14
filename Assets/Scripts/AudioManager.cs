using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private Dictionary<string, AudioSource> audioDictionary;

    public AudioSource MainTheme;

    private void Awake()
    {
        audioDictionary = new Dictionary<string, AudioSource>();

        foreach (AudioSource audio in GetComponentsInChildren<AudioSource>())
        {
            audioDictionary.Add(audio.name, audio);
        }

        var mTheme = GameObject.Find("MainTheme");
        if(mTheme)
            MainTheme = mTheme.GetComponent<AudioSource>();
    }

    public void Play(string name)
    {
        if(audioDictionary.ContainsKey(name))
            audioDictionary[name].Play();
    }

    public void PlayOnTopOfMainTheme(string name, float mainThemeVolume)
    {
        if(MainTheme)
            MainTheme.GetComponent<SetAudioLevel>().ChangeDamper(mainThemeVolume);
     
        if (audioDictionary.ContainsKey(name))
        {
            if (audioDictionary[name].isPlaying)
            {
                audioDictionary[name].Stop();
            }
            else
            {
                audioDictionary[name].Play();
            }
        }

    }
    
    /// <summary>
    /// I didn't understand why the music didn't want to launch. The way I found to make it works is by setactive false to true.
    /// </summary>
    public void ChangeMainTheme(AudioClip clip)
    {
        MainTheme.clip = clip;
        MainTheme.gameObject.SetActive(false);
        MainTheme.gameObject.SetActive(true);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioLevel : MonoBehaviour {

    AudioSource _audio;
    float ownDamper;

    public bool isMusic = false;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        ownDamper = _audio.volume;
    }

    private void Update()
    {
        if (isMusic)
            _audio.volume = Settings.VolumeMusic * ownDamper;
        else
            _audio.volume = Settings.VolumeSfx * ownDamper;
    }

    public void ChangeDamper(float damper)
    {
        ownDamper = damper;
    }

}

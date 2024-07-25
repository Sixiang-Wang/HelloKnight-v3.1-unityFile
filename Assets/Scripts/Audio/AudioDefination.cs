using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;
    public bool endOnDisable;
    private void OnEnable()
    {
        if (playOnEnable)
            PlayAudioClip();
    }

    private void OnDisable()
    {
        if (endOnDisable)
        {
            playAudioEvent.EndEvent();
        }
    }

    public void PlayAudioClip()
    {
        playAudioEvent.RaiseEvent(audioClip);
    }
}

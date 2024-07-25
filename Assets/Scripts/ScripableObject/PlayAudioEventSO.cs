using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/PlayAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRaised;
    public UnityAction OffEvent;

    public void RaiseEvent(AudioClip audioclip)
    {
        OnEventRaised?.Invoke(audioclip);
    }

    public void EndEvent()
    {
        OffEvent?.Invoke();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("广播")]
    public FloatEventSO syncVolumeEvent;

    [Header("事件监听")]
    public FloatEventSO volumeEvent;
    public VoidEventSO pauseEvent;

    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO PlayerHurtEvent;
    public PlayAudioEventSO EnemyHurtEvent;
    public PlayAudioEventSO PlayerMoveLoopEvent;
    public PlayAudioEventSO PlayerMoveSingleEvent;
    public PlayAudioEventSO EndGameAudioEvent;


    [Header("组件")]
    public AudioMixer mixer;
    public AudioSource BGMSource;
    public AudioSource FXSource;
    public AudioSource PlayerHurtSource;
    public AudioSource EnemyHurtSource;
    public AudioSource PlayerMoveLoopSource;
    public AudioSource PlayerMoveSingleSource;
    public AudioSource EndGameAudeoSource;

    private void OnEnable()
    {
        volumeEvent.OnEventRaised += OnVolumeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;

        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        PlayerHurtEvent.OnEventRaised += OnPlayerHurtEvent;
        EnemyHurtEvent.OnEventRaised += OnEnemyHurtEvent;
        PlayerMoveLoopEvent.OnEventRaised += OnPlayerMoveLoopEvent;
        PlayerMoveLoopEvent.OffEvent += EndPlayerMoveLoopEvent;
        PlayerMoveSingleEvent.OnEventRaised += OnPlayerMoveSingleEvent;

        EndGameAudioEvent.OnEventRaised += OnEndGameAudioEvent;
    }

    private void OnDisable()
    {
        volumeEvent.OnEventRaised -= OnVolumeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;

        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        PlayerHurtEvent.OnEventRaised -= OnPlayerHurtEvent;
        EnemyHurtEvent.OnEventRaised -= OnEnemyHurtEvent;
        PlayerMoveLoopEvent.OnEventRaised -= OnPlayerMoveLoopEvent;
        PlayerMoveLoopEvent.OffEvent -= EndPlayerMoveLoopEvent;
        PlayerMoveSingleEvent.OnEventRaised += OnPlayerMoveSingleEvent;

        EndGameAudioEvent.OnEventRaised -= OnEndGameAudioEvent;
    }

    

    private void OnPauseEvent()
    {
        float amount;
        mixer.GetFloat("MasterVolume", out amount);
        syncVolumeEvent.RaiseEvent(amount);
    }

    private void OnVolumeEvent(float amount)
    {
        mixer.SetFloat("MasterVolume", amount*100 - 80);
    }

    private void OnPlayerMoveSingleEvent(AudioClip clip)
    {
        PlayerMoveSingleSource.clip = clip;
        PlayerMoveSingleSource.Play();
    }

    private void EndPlayerMoveLoopEvent()
    {
        PlayerMoveLoopSource.Stop();
    }

    private void OnPlayerMoveLoopEvent(AudioClip clip)
    {
        PlayerMoveLoopSource.clip = clip;
        PlayerMoveLoopSource.Play();
    }

    private void OnEnemyHurtEvent(AudioClip clip)
    {
        EnemyHurtSource.clip = clip;
        EnemyHurtSource.Play();
    }

    private void OnPlayerHurtEvent(AudioClip clip)
    {
        PlayerHurtSource.clip = clip;
        PlayerHurtSource.Play();
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }

    private void OnEndGameAudioEvent(AudioClip arg0)
    {
        EndGameAudeoSource.clip = arg0;
        EndGameAudeoSource.Play();
    }
}

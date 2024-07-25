using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Audio;

public class EndGame : MonoBehaviour
{
    [Header("监听")]
    public VoidEventSO endGameEvent;

    [Header("广播")]
    public VoidEventSO backToMenuEvent;

    [Header("组件")]
    public GameObject endGameVideo;
    public AudioMixer mixer;

    [Header("视频参数")]
    public float BGMVolumeBefore;
    public float OtherVolumeBefore;
    public float audeoTime;
    public float videoTime;
    public float videoCounter;
    public bool videoPlaying;


    private void OnEnable()
    {
        endGameEvent.OnEventRaised += EndGameEvent;

    }

    private void OnDisable()
    {
        endGameEvent.OnEventRaised -= EndGameEvent;

    }

    private void Update()
    {
        if (videoCounter > 0)
        {
            videoCounter -= Time.deltaTime;
        }
        else if(-1<videoCounter&&videoCounter<=0)
        {
            videoCounter = -10;
            videoPlaying = false;
            StartCoroutine(BackToMenuEnumerator());
        }

    }

    private void EndGameEvent()
    {
        StartCoroutine(EndGameEnumerator());
    }

    private IEnumerator EndGameEnumerator()
    {
        mixer.GetFloat("BGMVolume", out BGMVolumeBefore);
        mixer.SetFloat("BGMVolume", -80);
        mixer.GetFloat("OtherVolume", out OtherVolumeBefore);
        yield return new WaitForSeconds(audeoTime);
        mixer.SetFloat("OtherVolume", -80);
        videoPlaying = true;
        videoCounter = videoTime;
        endGameVideo.SetActive(true);
    }
    private IEnumerator BackToMenuEnumerator()
    {
        backToMenuEvent.RaiseEvent();
        yield return new WaitForSeconds(2);
        endGameVideo.SetActive(false);
        mixer.SetFloat("BGMVolume", BGMVolumeBefore);
        mixer.SetFloat("OtherVolume", OtherVolumeBefore);
    }
}

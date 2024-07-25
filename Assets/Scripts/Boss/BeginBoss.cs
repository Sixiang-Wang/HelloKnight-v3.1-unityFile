using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BeginBoss : MonoBehaviour
{
    public GameObject doors;
    public GameObject boss;

    public PlayAudioEventSO playBossAudio;
    public AudioClip mantisLords;
    public AudioClip endBossAudio;

    public bool bossEnd;
    [Header("ÊÂ¼þ¼àÌý")]
    public VoidEventSO playerInBossRoomEvent;
    public VoidEventSO bossDieEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        playerInBossRoomEvent.OnEventRaised += beginBoss;
        bossDieEvent.OnEventRaised += endBoss;
    }

    private void OnDisable()
    {
        playerInBossRoomEvent.OnEventRaised -= beginBoss;
        bossDieEvent.OnEventRaised -= endBoss;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void beginBoss()
    {
        if (!bossEnd)
        {
            StartCoroutine(IbeginBoss());
        }
    }

    private void endBoss()
    {
        StartCoroutine(IendBoss());
    }

    private IEnumerator IbeginBoss()
    {
        playBossAudio.RaiseEvent(mantisLords);
        doors.SetActive(true);
        yield return new WaitForSeconds(2);
        boss.SetActive(true);
    }

    private IEnumerator IendBoss()
    {
        bossEnd = true;
        doors.SetActive(false);
        yield return new WaitForSeconds(1);
        playBossAudio.RaiseEvent(endBossAudio);
    }
}

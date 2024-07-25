using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]
    public int maxHeath;
    public int currentHeath;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;


    [Header("受伤无敌")]
    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool invulnerable;
    public float isHurtDuration;
    public float isHurtCounter;
    public bool soundInvulnerable;
    public float soundInvulnerableDuration;
    public float soundInvulnerableCounter;

    [Header("玩家死亡")]
    public bool isPlayer;
    public bool deadAnimEndOn;
    public bool deadAnimEndOff;
    public GameObject zoteHead;

    [Header("声音")]
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClipHurt;
    public AudioClip DeadAudio;

    public bool haveInvulnerableAudio;
    public AudioClip invulnerableAudio;

    public UnityEvent<Character> OnHealthChange;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Transform> OnDie;

    public bool isHurt;
    public bool isDead;
    private float tempDyingTime;
    private void Start()
    {
        currentHeath = maxHeath;
    }

    private void NewGame()
    {
        isDead = false;
        currentPower = maxPower;
        currentHeath = maxHeath;
        OnHealthChange?.Invoke(this);
    }

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (isHurt)
        {
            isHurtCounter -= Time.deltaTime;
            if (isHurtCounter <= 0)
            {
                isHurt = false;
            }
        }

        if (soundInvulnerable && haveInvulnerableAudio)
        {
            soundInvulnerableCounter -= Time.deltaTime;
            if(soundInvulnerableCounter <= 0)
            {
                soundInvulnerable = false;
            }
        }

        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }

        if (!deadAnimEndOn)
            deadAnimEndOff = true;
        if (deadAnimEndOn&&deadAnimEndOff)
        {
            deadAnimEndOff = false;
            if (isPlayer)
            {
                GameObject zoteHead = Instantiate(this.zoteHead, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
                zoteHead.GetComponent<Rigidbody2D>().velocity = new Vector2(5,5*-transform.localScale.x);
            }
        }
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (!isDead)
            {
                playAudioEvent.RaiseEvent(DeadAudio);
            }
            if (currentHeath > 0)
            {
                isDead = true;
                currentHeath = 0;
                OnHealthChange?.Invoke(this);
                OnDie.Invoke(transform);
            }
        }
    }


    public void TakeDamage(Attack attacker)
    {
        
        float healthOld = currentHeath;

        if (haveInvulnerableAudio && !soundInvulnerable)
        {
            TriggerSoundInvulnerable();
            playAudioEvent.RaiseEvent(invulnerableAudio);
        }
        

        if (invulnerable)
        {
            return;
        }

        if (isDead == false)
        {
            playAudioEvent.RaiseEvent(audioClipHurt);
        }

        if (currentHeath - attacker.damage > 0)
        {
            TriggerHurtDur();
            isHurt = true;
            currentHeath -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
            
        }
        else if(currentHeath>0)
        {
            invulnerable = true;
            invulnerableCounter = 9999;
            //isHurt = true;
            currentHeath = 0;
            //OnTakeDamage?.Invoke(attacker.transform);
            isHurt = false;
            OnDie?.Invoke(attacker.transform);
            isDead = true;
            //gg
        }
        else
        {
            OnDie?.Invoke(attacker.transform);
            isDead = true;
        }
        //isHurt = false;

        if (Mathf.Abs(healthOld-currentHeath) > 0.1 )
        {

            OnHealthChange?.Invoke(this);
        }
        
    }

    //受伤无敌
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    private void TriggerHurtDur()
    {
        isHurt = true;
        isHurtCounter = isHurtDuration;

    }

    private void TriggerSoundInvulnerable()
    {
        soundInvulnerable = true;
        soundInvulnerableCounter = soundInvulnerableDuration;
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID)) 
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "health"] = this.maxHeath;
            data.floatSavedData[GetDataID().ID + "power"] =  this.maxPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "health", this.maxHeath);
            data.floatSavedData.Add(GetDataID().ID + "power", this.maxPower); 
        }
        currentHeath = maxHeath;
        currentPower = maxPower;
        OnHealthChange.Invoke(this);
    }

    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            this.currentHeath = (int)data.floatSavedData[GetDataID().ID+"health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];


            //更新血条
            OnHealthChange?.Invoke(this);
        }
    }
}

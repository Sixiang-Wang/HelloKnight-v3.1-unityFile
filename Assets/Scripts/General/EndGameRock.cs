using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameRock : MonoBehaviour, IInteractable, ISaveable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;
    public GameObject lightObj;


    [Header("¹ã²¥")]
    public VoidEventSO endGameEvent;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;

        lightObj.SetActive(isDone);

        ISaveable saveable = this;
        saveable.RegisterSaveData();



    }
    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
        if (isDone)
        {
            lightObj.SetActive(true);
            this.gameObject.tag = "Used";
        }
        else
        {
            lightObj.SetActive(false);
            this.gameObject.tag = "Interactable";
        }
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            lightObj.SetActive(true);
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isDone = true;
        spriteRenderer.sprite = openSprite;
        GetComponent<AudioDefination>().PlayAudioClip();
        this.gameObject.tag = "Used";
        endGameEvent.RaiseEvent();

    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "isDone"))
        {
            data.boolSavedData[GetDataID().ID + "isDone"] = this.isDone;
        }
        else
        {
            data.boolSavedData.Add(GetDataID().ID + "isDone", this.isDone);
        }
    }

    public void LoadData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "isDone"))
        {
            this.isDone = data.boolSavedData[GetDataID().ID + "isDone"];
        }
    }
}


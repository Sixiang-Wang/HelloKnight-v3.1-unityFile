using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable,ISaveable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    [Header("¹ã²¥")]
    public VoidEventSO getAbilityEvent;
    public VoidEventSO saveDataEvent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;

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
            this.gameObject.tag = "Used";
        }
        else
        {
            this.gameObject.tag = "Interactable";
        }
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        getAbilityEvent.RaiseEvent();
        isDone = true;
        spriteRenderer.sprite = openSprite;
        GetComponent<AudioDefination>().PlayAudioClip();
        this.gameObject.tag = "Used";
        saveDataEvent.RaiseEvent();
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

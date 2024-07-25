using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("¹ã²¥")]
    public VoidEventSO saveDataEvent;

    [Header("²ÎÊý")]
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public GameObject lightObj;
    public bool isDone;
    public bool endAnim;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        spriteRenderer.color = Color.clear;
        
    }

    private void OnEnable()
    {
        lightObj.SetActive(isDone);
    }

    private void Update()
    {
        if (endAnim)
        {
            spriteRenderer.color = Color.clear;
            isDone = false;
            lightObj.SetActive(false);
            isDone = false;
            this.gameObject.tag = "Interactable";
        }
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            this.gameObject.tag = "Untagged";
            endAnim = false;
            spriteRenderer.color = Color.white;
            isDone = true;
            lightObj.SetActive(true);
            anim.SetTrigger("Save");
            saveDataEvent.RaiseEvent();
        }
    }

}

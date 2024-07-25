using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Sign : MonoBehaviour
{
    private PlayerinputControl playerInput;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    public bool canPress;

    private IInteractable targetItem;

    [Header("¹ã²¥")]
    public VoidEventSO playerInBossRoomEvent;
    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerinputControl();
        playerInput.Enable();
    }


    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void Update()
    {

        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if(actionChange == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;

            switch (d.device)
            {
                case Keyboard:
                    anim.Play("E");
                    break;
                case DualShockGamepad:
                    break;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = collision.GetComponent<IInteractable>();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("closeDoor"))
        {
            Debug.Log("inBoss");
            playerInBossRoomEvent.RaiseEvent();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
        if (collision.CompareTag("Interactable")|| collision.CompareTag("Used"))
        {
            canPress = false;
        }
    }
}

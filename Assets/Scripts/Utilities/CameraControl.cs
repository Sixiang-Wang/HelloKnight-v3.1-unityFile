using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Unity.VisualScripting;

public class CameraControl : MonoBehaviour
{
    /*
    public Transform target;
    public Transform farBackground;
    public Transform middleBackGround;
    public Transform NearBackGround;
    private Vector2 lastPos;
    */
    [Header("监听")]
    public VoidEventSO afterSceneLoadedEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO camaraShakeEvent;

    

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        camaraShakeEvent.OnEventRaised += OnCameraSHakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }

    

    private void OnDisable()
    {
        camaraShakeEvent.OnEventRaised -= OnCameraSHakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    private void OnCameraSHakeEvent()
    {
        impulseSource.GenerateImpulse();
    }


    //场景切换后更改
    private void Start()
    {
        

        /*
        lastPos = transform.position;
        */
    }

    /*
    private void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector2 amountToMove = new Vector2(target.position.x - lastPos.x, target.position.y - lastPos.y);
        farBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f);
        middleBackGround.position += new Vector3(amountToMove.x * 0.5f, amountToMove.y*0.85f, 0f);
        NearBackGround.position += new Vector3(amountToMove.x * 0.3f, amountToMove.y * 0.5f, 0f);
        lastPos = transform.position;
    }
    */
    
    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        confiner2D.InvalidateCache();
    }
    
}

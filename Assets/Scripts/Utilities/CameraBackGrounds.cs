using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Unity.VisualScripting;

public class CameraBackGrounds : MonoBehaviour
{
    //
    public Transform target;
    public Transform farBackground;
    public Transform middleBackground;
    public Transform nearBackground;
    private Vector2 lastPos;
    //




    private void Awake()
    {
        /*
        var farObj = GameObject.FindGameObjectWithTag("BackgroundFar");
        var middleObj = GameObject.FindGameObjectWithTag("BackgroundMiddle");
        var nearObj = GameObject.FindGameObjectWithTag("BackgroundNear");
        farBackground = farObj.GetComponent<Transform>();
        middleBackground = middleObj.GetComponent<Transform>();
        nearBackground = nearObj.GetComponent<Transform>();
        */
    }


    //场景切换后更改
    private void Start()
    {

        //
        lastPos = transform.position;
        //
    }

    //
    private void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector2 amountToMove = new Vector2(target.position.x - lastPos.x, target.position.y - lastPos.y);
        farBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f);
        middleBackground.position += new Vector3(amountToMove.x * 0.5f, amountToMove.y * 0.85f, 0f);
        nearBackground.position += new Vector3(amountToMove.x * 0.3f, amountToMove.y * 0.5f, 0f);
        lastPos = transform.position;
    }
    //


}


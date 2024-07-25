using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [Header("ÎÞÏÞ±³¾°")]
    public GameObject mainCamera;
    public float mapWidth;
    public int mapNums;
    public float totalWidth;
    public Vector3 tempPosition;
    public float MainTrans,trans2,trans3;
    public bool notAuto;
    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if(!notAuto)
            mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        totalWidth = mapWidth * mapNums;

    }

    private void Update()
    {
        totalWidth = mapWidth * mapNums;
        MainTrans = mainCamera.transform.position.x;
        trans2 = transform.position.x + totalWidth / 2;
        trans3 = transform.position.x - totalWidth / 2;
        tempPosition = transform.position;
        if(mainCamera.transform.position.x > transform.position.x + totalWidth / 2)
        {
            tempPosition.x += totalWidth;
            transform.position = tempPosition;
        }
        else if(mainCamera.transform.position.x < transform.position.x - totalWidth / 2)
        {
            tempPosition.x -= totalWidth;
            transform.position = tempPosition;
        }
    }
}

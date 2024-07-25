using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PhysicsCheck physicsCheck;

    // Start is called before the first frame update
    void Start()
    {
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (physicsCheck.isGround)
        {

            Destroy(this.gameObject);
        }
    }
}

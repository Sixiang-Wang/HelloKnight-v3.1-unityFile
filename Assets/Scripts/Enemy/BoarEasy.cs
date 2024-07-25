using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarEasy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarEasyPatrolState();
    }
}

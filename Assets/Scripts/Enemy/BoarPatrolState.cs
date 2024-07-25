using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


//巡逻时候的状态
public class BoarPatrolState : BaseState
{

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        

        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
            currentEnemy.rb.velocity = new Vector2(0,currentEnemy.rb.velocity.y);
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
            currentEnemy.anim.SetBool("run", false);
        }
        else if(currentEnemy.wait!=true)
        {
            currentEnemy.anim.SetBool("walk", true);
        }
        //发现player切换到chase

    }


    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private bool isAttack;
    private float attackRateCounter = 0;

    

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;

        attack = enemy.GetComponent<Attack>();
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);
        //¼ÆÊ±Æ÷
        attackRateCounter -= Time.deltaTime;

        //ÅÐ¶Ï¹¥»÷¾àÀë
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange*0.85)
        {
            //¹¥»÷
            if (!currentEnemy.isHurt)
            {
                currentEnemy.rb.velocity = Vector2.zero;
            }
            isAttack = true;

            
            //¼ÆÊ±Æ÷
            
            if (attackRateCounter <= 0)
            {
                currentEnemy.anim.SetTrigger("attack");
                
                attackRateCounter = attack.attackRate;
                
                
            }
        }
        else
        {
            isAttack = false;
        }

        //ÈËÎï·­×ª
        moveDir = (target - currentEnemy.transform.position).normalized;

        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        //ÅÐ¶Ï¶¯»­ÊÇ·ñÊÇ¹¥»÷¶¯»­
        if (currentEnemy.isShootAnim)
        {
            currentEnemy.Shoot(new Vector3(target.x - currentEnemy.transform.position.x, target.y - currentEnemy.transform.position.y, 0).normalized);
        }
            
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("run", false);
    }
}

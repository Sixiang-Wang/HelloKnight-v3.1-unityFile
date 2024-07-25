using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private PhysicsCheck physicsCheck;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isCrouch", playerController.isCrouch);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerController.isSlide);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayerAttack()
    {
        anim.SetTrigger("attack");
    }
}

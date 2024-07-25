using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDown : MonoBehaviour
{
    private Attack attack;
    public GameObject player;
    private Rigidbody2D rb;
    private PlayerController pCtrl;

    private void Awake()
    {
        attack = GetComponent<Attack>();
        rb = player.GetComponent<Rigidbody2D>();
        pCtrl = player.GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.velocity = new Vector2(rb.velocity.x, pCtrl.jumpSpeed);
        pCtrl.jumpTime = pCtrl.jumpMaxTime-1;
        pCtrl.slideReady = true;
        collision.GetComponent<Character>()?.TakeDamage(attack);
    }

}


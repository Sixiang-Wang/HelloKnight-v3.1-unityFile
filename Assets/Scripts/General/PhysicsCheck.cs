using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("¼ì²â²ÎÊý")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 leftOffsetDash;
    public Vector2 rightOffsetDash;
    public float checkHeight;
    public float checkWidth;

    public float checkRaduis;
    public LayerMask groundLayer;

    [Header("×´Ì¬")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool touchLeftWallDash;
    public bool touchRightWallDash;
    public bool onWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);

        }

        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
        }
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Check();
    }


    private void Check()
    {
        
        //¼ì²âµØÃæ£¿
        if(onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y+0.1f), checkRaduis, groundLayer);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis, groundLayer);
        //Ç½ÌåÅÐ¶Ï
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis, groundLayer);
        touchLeftWallDash = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(leftOffsetDash.x, leftOffsetDash.y), new Vector2(checkWidth, checkHeight), 0, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis, groundLayer);
        touchRightWallDash = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(rightOffsetDash.x, rightOffsetDash.y), new Vector2(checkWidth, checkHeight), 0, groundLayer);

        if (isPlayer)
        {
            onWall = ((touchLeftWall && playerController.inputDirection.x < 0f) || (touchRightWall && playerController.inputDirection.x > 0f)) && rb.velocity.y<0f;
            if (!playerController.canSlideWall)
                onWall = false;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis);
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(leftOffsetDash.x, leftOffsetDash.y), new Vector2(checkWidth, checkHeight));
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(rightOffsetDash.x, rightOffsetDash.y), new Vector2(checkWidth, checkHeight));

    }
}

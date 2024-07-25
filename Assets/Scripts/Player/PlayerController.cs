using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Events;
using System.Runtime.Remoting.Messaging;


public class PlayerController : MonoBehaviour,ISaveable
{
    
    public PlayerinputControl inputControl;
    public Vector2 inputDirection;
    [Header("�����¼�")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;
    public VoidEventSO newGameEvent;
    public VoidEventSO getDoubleJumpEvent;
    public VoidEventSO getDashEvent;
    public VoidEventSO getWallSlideEvent;
    public VoidEventSO getInvulnerableDashEvent;

    [Header("�㲥")]
    public PlayAudioEventSO PlayerMoveLoopEvent;

    [Header("�ƶ�")]
    public float playerSpeed;
    public float runspeed;
    public float walkspeed;
    private bool startMove;
    private bool endMove;
    [Header("����")]
    public bool canSlideWall;
    public bool canDash;
    public bool canInvulnerableDash;
    [Header("��Ծ")]
    public float jumpForce;
    public float wallJumpForce;
    public bool wallJump;
    public float jumpSpeed;
    public float wallJumpSpeed;
    public float wallJumpTimer;
    public float wallJumpTimerCounter;
    public bool onWallBefore;
    public bool onWallBeforeForWallJump;
    public bool wallJumpAnimEnd;
    public int jumpMaxTime;
    public int jumpTime;
    public bool jumping;
    public bool jumpButton;
    public bool wallJumpButton;
    public float wallJumpDir;
    public bool jumpReady;
    public float OnGroundTime;
    public bool isFly;

    public bool isDrop;

    [Header("��ǽ")]
    public float wallSlideSpeedRate;
    [Header("���")]
    public float slideDistance;
    public float slideSpeed;
    public bool isSlide;
    public int slidePowerCost;
    public float slideDir;
    public bool slideReady;
    public GameObject dashEffect;
    [Header("�¶�")]
    public bool isCrouch;
    [Header("����")]
    public float hurtForce;
    public bool isHurt;
    [Header("��ƨ")]
    public bool isDead;
    [Header("����")]
    public bool isAttack;
    public bool attackUp;
    public bool attackDown;
    [Header("��")]
    public float fallMaxSpeed;
    public bool fallMaxOn;
    


    [Header("�������")]
    public PhysicsMaterial2D Normal;
    public PhysicsMaterial2D Wall;

    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;
    private Character character;
    private Animator anim;

    [Header("����")]
    public PlayAudioEventSO playAudioEvent;
    public AudioClip stepAudio;
    public AudioClip jumpAudio;
    public AudioClip wallJumpAudio;
    public AudioClip sildeAudio;
    public AudioClip landAudio;
    public AudioClip landHardAudio;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerinputControl();
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Walk.started += BeginWalk;
        inputControl.Gameplay.Walk.canceled += StopWalk;
        inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Dash;
        

        inputControl.Enable();
    }

    

    private void StopWalk(InputAction.CallbackContext context)
    {
        playerSpeed = runspeed;
    }

    private void BeginWalk(InputAction.CallbackContext context)
    {
        playerSpeed = walkspeed;
    }

    private void NewGame()
    {
        canDash = false;
        canSlideWall = false;
        jumpMaxTime = 1;
    }

    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveData();

        newGameEvent.OnEventRaised += NewGame;
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        getDoubleJumpEvent.OnEventRaised += GetDoubleJump;
        getDashEvent.OnEventRaised += GetDash;
        getWallSlideEvent.OnEventRaised += GetWallSlide;
        getInvulnerableDashEvent.OnEventRaised += GetInvulnerableDash;
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();

        inputControl.Disable();
        newGameEvent.OnEventRaised += NewGame;
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        getDoubleJumpEvent.OnEventRaised += GetDoubleJump;
        getDashEvent.OnEventRaised -= GetDash;
        getWallSlideEvent.OnEventRaised -= GetWallSlide;
        getInvulnerableDashEvent.OnEventRaised -= GetInvulnerableDash;
    }

    

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        anim.SetBool("isFly", isFly);
        anim.SetBool("attackUp", attackUp);
        anim.SetBool("attackDown", attackDown);

        CheckState();
        if(!onWallBefore)
            anim.SetBool("onWallBefore", onWallBefore);
        if (physicsCheck.onWall)
        {
            onWallBefore = true;
            onWallBeforeForWallJump = true;
        }
        if (physicsCheck.isGround||wallJumpAnimEnd)
        {
            onWallBefore = false;
        }
        if (physicsCheck.isGround)
        {
            onWallBeforeForWallJump = false;
        }
        if (wallJump)
        {
            wallJumpTimerCounter -= Time.deltaTime;
        }
        if (wallJumpTimerCounter <= 0)
            wallJump = false;
        if (!physicsCheck.isGround)
        {
            //��ֹrigidbody���е����õ�
            jumpButton = false;
        }
        if (!physicsCheck.onWall)
        {
            wallJumpButton = false;
        }
        if ((physicsCheck.isGround || physicsCheck.onWall) &&!jumpButton &&!wallJumpButton)
        {
            jumping = false;
            if(!jumpButton)
                jumpTime = jumpMaxTime;
        }
        if (!inputControl.Gameplay.Jump.IsPressed())
        {
            jumpReady = true;
        }

        if (physicsCheck.isGround && OnGroundTime<=0.1)
        {
            OnGroundTime += Time.deltaTime;
        }
        if (OnGroundTime >= 0.1)
        {
            jumpButton = false;
        }

        if (physicsCheck.isGround || physicsCheck.onWall)
        {
            slideReady = true;
            isDrop = false;
        }

        if(!jumping && !physicsCheck.isGround && !physicsCheck.onWall && !isDrop && !onWallBeforeForWallJump)
        {
            isDrop = true;
            jumpTime--;
        }


    }

    
    private void FixedUpdate()
    {
        if(!isHurt&&!isSlide)
            Move();
        if (startMove && ((inputDirection.x > -0.1f && inputDirection.x < 0.1f) || !physicsCheck.isGround || physicsCheck.touchLeftWall || physicsCheck.touchRightWall || isSlide))
        {
            startMove = false;
            PlayerMoveLoopEvent.OffEvent();
        }
        WallJump();
        FallSpeedLimit();

        if (Keyboard.current.yKey.isPressed)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2((float)(inputDirection.x * 3 *playerSpeed * Time.deltaTime), (float)(inputDirection.y * 3 *playerSpeed * Time.deltaTime));
        }
        if(Keyboard.current.yKey.wasReleasedThisFrame)
        {
            rb.gravityScale = 5;
        }
    }

    //���ȡ���ȶ�����
    private void OnLoadDataEvent()
    {
        isDead = false;
        character.isDead = false;  
    }

    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }

    //�����½��ٶȣ�Ҫ��Ȼ�����
    private void FallSpeedLimit()
    {
        if(rb.velocity.y <= -fallMaxSpeed)
        {
            rb.gravityScale = 0;
            fallMaxOn = true;

        }
        else if(rb.velocity.y >= -fallMaxSpeed + 1)
        {
            fallMaxOn = false;
            rb.gravityScale = 5;

        }
    }

    public void Move()
    {

        if (!wallJump)
        {
            if (!isCrouch)
            {
                rb.velocity = new Vector2((float)(inputDirection.x * playerSpeed * Time.deltaTime), rb.velocity.y);
                if((inputDirection.x > 0.1f || inputDirection.x < -0.1f) && physicsCheck.isGround && !physicsCheck.touchLeftWall && !physicsCheck.touchRightWall && !startMove)
                {
                    startMove = true;
                    PlayerMoveLoopEvent.RaiseEvent(stepAudio);
                }
            }
                
            else
                rb.velocity = new Vector2((float)(inputDirection.x * 0.5f * playerSpeed * Time.deltaTime), rb.velocity.y);
        }
            

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if(inputDirection.x < 0)
        {
            faceDir = -1;
        }
        //��ת
        transform.localScale = new Vector3(faceDir, 1, 1);

        //�¶�
        isCrouch = inputDirection.y < -0.1f && physicsCheck.isGround;
        attackUp = inputDirection.y > 0.1f;
        attackDown = inputDirection.y < -0.1f && !physicsCheck.isGround;


        if (isCrouch)
        {
            //��С��ײ���
            coll.offset = new Vector2(-0.05f, 0.45f);
            coll.size = new Vector2(0.8f, 0.9f);
        }
        else if(!isSlide)
        {
            //��ԭ��ײ���
            coll.offset = originalOffset;
            coll.size = originalSize;
        }

        if (isSlide)
        {
            coll.offset = new Vector2(-0.05f, 0.45f);
            coll.size = new Vector2(0.8f, 0.9f);
        }
        else if(!isCrouch)
        {
            coll.offset = originalOffset;
            coll.size = originalSize;
        }
    }

    private void WallJump()
    {
        
        if (jumpTime >= 0)
        {
            
            if (inputControl.Gameplay.Jump.IsPressed() && physicsCheck.onWall && jumpReady)
            {
                jumpReady = false;
                wallJump = true;
                wallJumpButton = true;
                wallJumpTimerCounter = wallJumpTimer;
            }
            if (inputControl.Gameplay.Jump.IsPressed() && wallJumpTimerCounter > 0 && wallJump)
            {
                rb.velocity = new Vector2(wallJumpDir * wallJumpSpeed, 3f*wallJumpSpeed);
            }
            if (wallJump && !inputControl.Gameplay.Jump.IsPressed())
            {
                jumpReady = true;
                wallJump = false;
                wallJumpTimerCounter = -1;
            }
            if ((physicsCheck.isGround || physicsCheck.onWall) && !wallJumpButton)
            {

                wallJump = false;
                wallJumpTimerCounter = -1;
            }
        }
        
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (jumpTime == 1)
            isFly = true;
        else
            isFly = false;

        if (jumpMaxTime == 1)
            isFly = false;
        anim.SetBool("isFly", isFly);
        anim.SetInteger("jumpTime", jumpTime);
        anim.SetBool("onWallBefore", onWallBefore);
        
        OnGroundTime = 0;
        jumpTime--;
        wallJumpButton = true;
        wallJumpDir = -inputDirection.x;
        if (!physicsCheck.onWall)
        {
            jumpButton = true;
            jumping = true;
        }
        if (jumpTime >= 0)
        {
            playAudioEvent.RaiseEvent(jumpAudio);
            if (!physicsCheck.onWall)
            {
                isSlide = false;
                StopAllCoroutines();
                jumpReady = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
        }
        

    }

    public void Land()
    {
        if (fallMaxOn)
        {
            playAudioEvent.RaiseEvent(landHardAudio);

        }
        else
        {
            playAudioEvent.RaiseEvent(landAudio);

        }
        fallMaxOn = false;
    }



    private void Dash(InputAction.CallbackContext context)
    {
        if (!canDash)
        {
            return;
        }
        if (!isSlide && character.currentPower >= slidePowerCost && slideReady)
        {
            dashEffect.SetActive(true);
            dashEffect.GetComponent<Animator>().SetTrigger("Dash");
            isSlide = true;
            slideDir = transform.localScale.x;
            var targetPos = new Vector3(transform.position.x + slideDistance * slideDir, transform.position.y);

            StartCoroutine(TriggerDash(targetPos));

            character.OnSlide(slidePowerCost);
        }

    }

    private IEnumerator TriggerDash(Vector3 target)
    {
        do
        {
            yield return null;

            if ((physicsCheck.touchLeftWallDash && transform.localScale.x < 0f) || (physicsCheck.touchRightWallDash && transform.localScale.x > 0f))
            {
                isSlide = false;
                break;
            }
            if (isHurt)
            {
                break;
            }
            if (!physicsCheck.isGround)
            {
                slideReady = false;
            }
            rb.MovePosition(new Vector2(transform.position.x + slideDir * slideSpeed, transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.2f);

        isSlide = false;
        rb.velocity = new Vector2(0f, 0f);
    }


    private void PlayerAttack(InputAction.CallbackContext context)
    {
        
        playerAnimation.PlayerAttack();
        isAttack = true;
        if (attackDown)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }



    #region ��UnityEvent��ִ�еĴ���
    public void GetHurt(Transform attacker)
    {
        isHurt = true; 
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x-attacker.position.x), 2).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }


    public void PlayerDead(Transform attacker)
    {
        
        isDead = true;
        if(physicsCheck.isGround)
            rb.velocity = new Vector2(0,rb.velocity.y);
        inputControl.Gameplay.Disable();
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? Normal : Wall;
        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeedRate);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (isDead || (isSlide && canInvulnerableDash))
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        if (wallJump&&rb.velocity.y<0)
        {
            wallJump = false;
        }
    }

    

    //��ȡ����
    private void GetDoubleJump()
    {
        jumpMaxTime = 2;
    }

    private void GetDash()
    {
        canDash = true;
    }

    private void GetWallSlide()
    {
        canSlideWall = true;
    }

    private void GetInvulnerableDash()
    {
        canInvulnerableDash = true;
    }


    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "canDash"))
        {
            data.boolSavedData[GetDataID().ID + "canDash"] = this.canDash;
            data.boolSavedData[GetDataID().ID + "canSlideWall"] = this.canSlideWall;
            data.boolSavedData[GetDataID().ID + "canInvulnerableDash"] = this.canInvulnerableDash;
            data.intSavedData[GetDataID().ID + "jumpMaxTime"] = this.jumpMaxTime;
            
        }
        else
        {
            data.boolSavedData.Add(GetDataID().ID + "canDash",this.canDash);
            data.boolSavedData.Add(GetDataID().ID + "canSlideWall", this.canSlideWall);
            data.boolSavedData.Add(GetDataID().ID + "canInvulnerableDash", this.canInvulnerableDash);
            data.intSavedData.Add(GetDataID().ID + "jumpMaxTime", this.jumpMaxTime);
        }
    }

    public void LoadData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "canDash"))
        {
            this.canDash = data.boolSavedData[GetDataID().ID + "canDash"];
            this.canSlideWall = data.boolSavedData[GetDataID().ID + "canSlideWall"];
            this.canInvulnerableDash = data.boolSavedData[GetDataID().ID + "canInvulnerableDash"];
            this.jumpMaxTime = data.intSavedData[GetDataID().ID + "jumpMaxTime"];
        }
    }
}

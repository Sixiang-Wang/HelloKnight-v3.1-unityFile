using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector]public Rigidbody2D rb;
    [HideInInspector] public  Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    [HideInInspector] public Transform attacker;
    private Character character;

    [Header("��������")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public Vector3 spawnPoint;


    [Header("�����˵���")]
    public float hurtForce;
    public bool isBoss;

    [Header("���")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    
    [Header("��ʱ��")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    public float lostTime;
    public float lostTimeCounter;

    [Header("״̬")]
    public bool isHurt;
    public bool isDead;

    [Header("Boss�㲥")]
    public VoidEventSO bossDieEvent;

    [Header("�ӵ���")]
    public GameObject bulletPre;
    public float bulletSpeed;
    private float shootAngle;
    [HideInInspector]public bool isShootAnim = false;

    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        //waitTimeCounter = waitTime;
        spawnPoint = transform.position;
    }

    private void OnEnable()
    {
        //״̬��
        currentState = patrolState;
        currentState.OnEnter(this);
        
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        
        //״̬��
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        //״̬��
        currentState.PhysicsUpdate();

        if (!wait && !isHurt && !isDead)
            Move();

        
    }

    private void OnDisable()
    {
        //״̬��
        currentState.OnExit();
        
    }

    public virtual void Move()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PreMove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover"))
            rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    //��ʱ��
    public void TimeCounter()
    {
        
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {

                wait = false;
                waitTimeCounter = waitTime;

            }
        }

        if (!FoundPlayer()&&lostTimeCounter>=0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if(FoundPlayer()){
            lostTimeCounter = lostTime;
        }

            
    }

    public virtual bool FoundPlayer()
    {
        if (character.isHurt)
        {
            return true;
        }
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
        
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill =>skillState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this); 
    }
    
    public virtual Vector3 GetNewPoint()
    {
        return transform.position;

    }
    #region �¼�ִ�з���
    public void OntakeDamage(Transform attackTrans)
    {
        isHurt = true;
        
        attacker = attackTrans;
        //ת��
        if (!isBoss)
        {
            if (attackTrans.position.x - transform.position.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            if (attackTrans.position.x - transform.position.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        

        //����
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        if (!isBoss)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        StartCoroutine(OnHurt(dir));
        
    }

    //Э��
    private IEnumerator OnHurt(Vector2 dir)
    {
        
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);

        isHurt = false;

    }

    public void OnDie()
    {
        if (isBoss)
        {
            Debug.Log("1");
            bossDieEvent.RaiseEvent();
        }
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    public void AnimShootBegin()
    {
        isShootAnim = true;
    }

    #endregion

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(-transform.localScale.x * checkDistance, 0, 0), 0.2f);  
    }

    public virtual void Shoot(Vector3 shootDir)
    {
        isShootAnim = false;
        GameObject bullet = Instantiate(bulletPre,transform.position+new Vector3(0,-0.8f,0),Quaternion.identity);
        
        if (shootDir.x >= 0)
        {
            if (shootDir.y >= 0)
            {
                shootAngle = (Mathf.Acos(shootDir.x) * 180) / 3.1415926f;
            }
            else
            {
                shootAngle = (Mathf.Asin(shootDir.y) * 180) / 3.1415926f + 360;
            }
        }
        else
        {
            if (shootDir.y >= 0)
            {
                shootAngle = (Mathf.Acos(shootDir.x) * 180) / 3.1415926f;
            }
            else
            {
                shootAngle = -(Mathf.Asin(shootDir.y) * 180) / 3.1415926f + 180;
            }
        }
        bullet.transform.Rotate(new Vector3(0, 0, shootAngle+180-45));
        
        
        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * bulletSpeed;
    }
}

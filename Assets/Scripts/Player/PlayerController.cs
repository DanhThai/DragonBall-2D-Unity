using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    public float jumpPower = 80;
    public int HP;
    [SerializeField]
    private float hurtTimeDuration = 0.2f;
    [SerializeField]
    private float DieTimeDuration = 0.5f;
    [SerializeField]
    Transform grCheckCollider;
    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    private GameManager gameManager;
    private float horizontalVal;
    private Vector2 direction;
    public State state {get; set;}
    public enum State
    {
        WALKING,
        FLYING,
        JUMPING
    }

    int facingDirection = 1;
    const float checkGroundRadius = 0.4f;
    bool isGround = false, isHurt = false, isDie;

    private int currentHP;
    private float hurtTimeStart, dieTimeStart;
    private float heightCurrent = 0;
    private Rigidbody2D rgbody;
    private Animator playerAnimator;
    private GameObject aliveGO, deadGO;

    void Start()
    {
        currentHP = HP;
        aliveGO = transform.Find("Alive").gameObject;
        deadGO = transform.Find("Dead").gameObject;
        playerAnimator = aliveGO.GetComponent<Animator>();
        rgbody = aliveGO.GetComponent<Rigidbody2D>();
        aliveGO.SetActive(true);
        deadGO.SetActive(false);
    }
    void Update()
    {

        horizontalVal = Input.GetAxisRaw("Horizontal");
        // jump
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        Hurted();
        CheckDie();

    }
    void GoundCheck(Vector2 dir)
    {
        isGround = false;
        // Check player to ground layer
        //Collider2D[] collider = Physics2D.OverlapCircleAll(grCheckCollider.position, checkGroundRadius, groundLayer);
        isGround = Physics2D.Raycast(grCheckCollider.position, Vector2.down, checkGroundRadius, groundLayer);
        if (horizontalVal == 0 && !isGround)
        {
            rgbody.gravityScale = 1;
            playerAnimator.SetBool("isJump", !isGround);
            var yVelocity= Mathf.Abs(rgbody.velocity.y) <= 1? -1 :rgbody.velocity.y;
            Debug.Log(yVelocity);
            playerAnimator.SetFloat("yVelocity", yVelocity);
            heightCurrent = 0;         
        }
    }
    void FixedUpdate()
    {
        direction = new Vector2(horizontalVal, rgbody.velocity.y);
        
        //Debug.Log("horizontalVal"+horizontalVal);
        if (state == State.FLYING && horizontalVal != 0)
        {
            Fly(direction);
        }
        else
        {
            playerAnimator.SetFloat("yVelocity", -1);
            Move(direction);
        }
        GoundCheck(direction);

    }
    void ChangeFace(Vector2 dir){
        if (dir.x == -1)
        {
            facingDirection = -1;
        }
        if (dir.x == 1)
        {
            facingDirection = 1;
        }
        // 0 is Idle, 1 is runing
        aliveGO.transform.localScale = new Vector3(facingDirection, 1F, 1F);
    }
    void Move(Vector2 dir)
    {
        // player run
        #region run
        ChangeFace(dir);
        // walking
        if (isGround == true)
        {
            playerAnimator.SetFloat("xVelocity", Mathf.Abs(dir.x));
            playerAnimator.SetBool("isJump", false);
            state = State.WALKING;
        }
        else
            state = State.FLYING;
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            playerAnimator.SetBool("isAttack", false);
            float xVal = dir.x * speed * 100 * Time.fixedDeltaTime;
            rgbody.velocity = new Vector2(xVal, dir.y);
        }
        #endregion

    }
    void Jump()
    {
        //animator.SetBool("isAttack", false);
        playerAnimator.SetBool("isJump", true);
        playerAnimator.SetFloat("yVelocity", 1);
        rgbody.AddForce(new Vector2(0f, jumpPower*3));
        rgbody.gravityScale = 0;
        heightCurrent += jumpPower*3 ;
        state = State.FLYING;
        
    }
    void Fly(Vector2 dir)
    {
        ChangeFace(dir);
        rgbody.gravityScale = 0;
        playerAnimator.SetFloat("yVelocity", 0);
        float moveDis = rgbody.velocity.x + horizontalVal*100f * Time.fixedDeltaTime;
        rgbody.velocity = new Vector2(moveDis, heightCurrent);
    }
    public int getFaceDirection()
    {
        return facingDirection;
    }
    private void Hurted()
    {
        if (isHurt && Time.time - hurtTimeStart > hurtTimeDuration)
        {
            isHurt = false;

            playerAnimator.SetBool("isHurt", isHurt);
        }
    }
    private void CheckDie()
    {
        if (isDie && Time.time - dieTimeStart > DieTimeDuration)
        {
            /* isDie = false;
             playerAnimator.SetBool("isDie", isDie);
             aliveGO.transform.localPosition = new Vector3(-10f, 1f,0f);
             currentHP = HP;*/
            Destroy(gameObject);
        }
    }
    public void Damage(int damage)
    {
        Debug.Log(damage);
        currentHP -= damage;
        if (currentHP > 0)
        {
            Hurt();
        }
        else
        {
            Die();
        }

    }
    private void Hurt()
    {
        isHurt = true;
        hurtTimeStart = Time.time;
        playerAnimator.SetBool("isHurt", isHurt);
    }
    private void Die()
    {
        isDie = true;
        dieTimeStart = Time.time;
        deadGO.transform.localPosition = aliveGO.transform.localPosition + new Vector3(0f, 0.66f, 0f);
        deadGO.SetActive(true);
        aliveGO.SetActive(false);
        // Check die for relive
        gameManager.Dead();
        //playerAnimator.SetBool("isDie", isDie);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(grCheckCollider.position, new Vector2(grCheckCollider.position.x, grCheckCollider.position.y - checkGroundRadius));
    }

}

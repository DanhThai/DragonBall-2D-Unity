using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{

    [SerializeField]
    private int maxHealth = 50,damage;
    [SerializeField]
    private float 
        attackTimeDuration = 0.5f,
        dieTimeDuration = 0.5f, 
        knockTimeDuration = 1f,        
        hitRadius,
        movSpeed= 0.003f;
    [SerializeField]
    private GameObject hitParticle;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    Transform _checkPlayer;
    [SerializeField]
    LayerMask playerLayer;

    private float attackTimeStart,knockTimeStart,dieTimeStart;
    
    private int currentHealth, knockDir, flip = 1;
    private bool isKnock = false,isAttack=false, isDie=false;

    private float reliveTime = 20f, attackTimer = 5f, maxXMove = 0.9f;
    private float positonXStart;

    private GameObject aliveGO, deadGO;
    private Animator aliveAnim;
    private Rigidbody2D deadRG;
    

    // Start is called before the first frame update
    void Start()
    {
        aliveGO = transform.Find("Alive").gameObject;
        deadGO = transform.Find("Dead").gameObject;
        aliveAnim = aliveGO.GetComponent<Animator>();
        deadRG = deadGO.GetComponent<Rigidbody2D>();
        aliveGO.SetActive(true);
        deadGO.SetActive(false);
        currentHealth = maxHealth;
        positonXStart = aliveGO.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        checkKnock();
        checkAlive();
        CheckAttackPlayer();
        if(!isKnock && !isDie)
            AttackPlayer();
    }
    void checkAlive()
    {
        if (aliveGO.activeSelf == false)
        {
            if (Time.time - dieTimeStart > dieTimeDuration)
            {
                deadGO.SetActive(false);
            }
            if (Time.time - dieTimeStart > reliveTime)
            {
                isDie = false;
                aliveGO.SetActive(true);
                currentHealth = maxHealth;
            }
        }
    }
    private void Move()
    {
        if(!isKnock && !isAttack && !isDie)
        {
            if (Mathf.Abs(aliveGO.transform.position.x - positonXStart) > maxXMove)
            {              
                flip = -flip;
                aliveGO.transform.localScale = new Vector3(flip, 1f, 1f);
            }
            aliveGO.transform.position += new Vector3(movSpeed * flip, 0, 0);
        }
       
    }
    public void Damage(int damage)
    {

        currentHealth -= damage;

        knockDir = playerController.getFaceDirection();
        // Tạo Hit Particle Object

        GameObject hit = Instantiate(hitParticle);
        hit.transform.position = aliveAnim.transform.position + new Vector3(knockDir * 0.1f, 0f, 0);
        hit.transform.localScale = new Vector3(knockDir, 1, 1);


        if (currentHealth < 0)
        {
            Debug.Log("die");
            Invoke("Die", 1f);
            //Die();
        }
        else
        {
            Debug.Log("knock");
            KnockBack();
        }
    }
    public void KnockBack()
    {
        aliveGO.transform.localScale = new Vector3(-knockDir, 1F, 1F);
        isKnock = true;
        knockTimeStart = Time.time;
        aliveAnim.SetBool("isKnock", isKnock);
    }
    public void checkKnock()
    {
        if (isKnock && Time.time - knockTimeStart > knockTimeDuration)
        {
            isKnock = false;
            aliveAnim.SetBool("isKnock", isKnock);
        }
    }
    public void Die()
    {
        isDie = true;
        dieTimeStart = Time.time;
        aliveGO.SetActive(false);
        deadGO.SetActive(true);
        deadGO.transform.localScale = new Vector3(knockDir, 1F, 1F);
        deadRG.velocity = new Vector2(3 * knockDir, 1);
    }
    private void CheckAttackPlayer()
    {
        if(isAttack && Time.time - attackTimeStart > attackTimeDuration)
        {
            isAttack = false;
            aliveAnim.SetBool("isAttack", isAttack);
        }
    }
    private void AttackPlayer()
    {
        Collider2D[] detectPlayers = Physics2D.OverlapCircleAll(_checkPlayer.position, hitRadius, playerLayer);
        if (detectPlayers.Length > 0 && Time.time - attackTimeStart >= attackTimer)
        {
            isAttack = true;          
            aliveAnim.SetBool("isAttack", isAttack);
            attackTimeStart = Time.time;
            foreach (Collider2D player in detectPlayers)
            {
              /*  if (player.transform.localScale.x==1)
                    aliveAnim.SetBool("attackRight", false);
                else
                    aliveAnim.SetBool("attackRight", true);*/
                aliveAnim.transform.localScale = new Vector3(-aliveAnim.transform.localScale.x, 1F, 1F);
                Debug.Log(player.name);
                player.transform.parent.SendMessage("Damage", damage);              
            }

        }
    }
    private void OnDrawGizmos()
    { 
        Gizmos.DrawWireSphere(_checkPlayer.position, hitRadius);
    }
}

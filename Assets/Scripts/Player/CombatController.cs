using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private float durationTimer, attackRadius;
    private int damage=10;
    private float lastTimer = 0f;
    private int attackNum = 0,attackLast;
    private bool combatEnable,isAttack = false;
    private Animator playerAnimator;
    [SerializeField]
    private Transform enemyPos;
    [SerializeField]
    private LayerMask monsterLayer;
    private GameObject aliveGO;
    private Collider2D monster;

    // Start is called before the first frame update
    void Start()
    {
        aliveGO = transform.Find("Alive").gameObject;
        playerAnimator = aliveGO.GetComponent<Animator>();
        attackLast = attackNum;
    }

    // Update is called once per frame
    private void Update()
    {
        checkCombatInput();
        checkAttacking();
    }
    private void checkCombatInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {    
            combatEnable = true;
            attackNum++;
            lastTimer = Time.time;
        }
    }
    public void checkAttacking()
    {
        if(combatEnable)
        {
            if (!isAttack || attackNum-attackLast>0)
            {              
                //Debug.Log("attack"+ lastTimer);
                isAttack = true;
                attackLast = attackNum;
                checkEnemy();               
            }
            else
            {
                if (Time.time - lastTimer >= durationTimer)
                {
                    //Debug.Log("finish"+ Time.time);
                    finishAttack();
                }
            }
        }      
    }
    public void checkEnemy()
    {
        Collider2D[] detectEnemies = Physics2D.OverlapCircleAll(enemyPos.position,attackRadius, monsterLayer);
        if (detectEnemies.Length>0)
        {
            playerAnimator.SetBool("isAttack", isAttack);

            foreach (Collider2D enemy in detectEnemies)
            {
                monster = enemy;
                Invoke( "attack",0.3f);
            }
        }
            
    }
    private void attack()
    {
        monster.transform.parent.SendMessage("Damage", damage);
        //transform.localPosition = enemy.transform.localPosition;
        Debug.Log(monster.name);
    }
    private void finishAttack()
    {
        isAttack=false;
        combatEnable = false;
        playerAnimator.SetBool("isAttack", isAttack);
        attackNum = 0;

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyPos.position, attackRadius);
    }
}

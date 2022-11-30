using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrowController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 50;
    [SerializeField]
    private float dieTimeDuration=0.5f, knockTimeDuration=1f, reliveTime=20f;
    [SerializeField]
    private GameObject hitParticle;
    [SerializeField]
    private PlayerController playerController;

    private float dieTimeStart, knockTimeStart;
    private int currentHealth, knockDir;
    private bool isKnock=false;
    private GameObject aliveGO,deadGO;
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
    }

    // Update is called once per frame
    void Update()
    {
        checkKnock();
        checkAlive();
    }
    void checkAlive()
    {
        if(aliveGO.activeSelf ==false)
        {
            if (Time.time - dieTimeStart > dieTimeDuration)
            {
                deadGO.SetActive(false);
            }
            if (Time.time - dieTimeStart > reliveTime)
            {
                aliveGO.SetActive(true);
                currentHealth = maxHealth;
            }
        }
    }
    public void Damage(int damage)
    {

        currentHealth -= damage;
       
        knockDir = playerController.getFaceDirection();
        // Tạo Hit Particle Object
        //Debug.Log(aliveAnim.transform.position);
        //Vector3 damageDir = aliveAnim.transform.position + new Vector3(knockDir * 0.3f, 0f, 0);
        //transform.localPosition = damageDir;
        GameObject hit = Instantiate(hitParticle);
               
        hit.transform.position = aliveAnim.transform.position + new Vector3(knockDir* 0.3f, 0.2f, 0);
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
            isKnock=false;
            aliveAnim.SetBool("isKnock", isKnock);
        }
    }
    public void Die()
    {
        
        dieTimeStart = Time.time;
        aliveGO.SetActive(false);
        deadGO.SetActive(true);
        deadGO.transform.localScale = new Vector3(knockDir, 1F, 1F);
        deadRG.velocity = new Vector2(3 * knockDir, 1);
    }
}

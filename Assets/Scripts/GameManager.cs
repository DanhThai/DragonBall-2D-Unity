using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private CameraFollow cameraFollow;
    [SerializeField]
    private float reviveTime;
    private float reviveTimeStart;
    private bool isRevive;
    
    private GameObject aliveGO;
    // Start is called before the first frame update
    void Start()
    {
        // cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAlive();
    }
    private void CheckAlive()
    {
        if(isRevive && Time.time - reviveTimeStart > reviveTime)
        {
            isRevive = false;
            var position =new  Vector3(0f, 1f, 0);
            GameObject playerTemp = Instantiate(player,position, Quaternion.Euler( new Vector3(0,0,0)));
            Debug.Log("Revive-----------------");
            aliveGO = playerTemp.transform.Find("Alive").gameObject;
            cameraFollow.target = aliveGO.transform;
        }
    }
    public void Dead()
    {
        isRevive = true;
        reviveTimeStart = Time.time;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    public Transform target;
    [SerializeField]
    Vector3 offset;
    [SerializeField]
    Vector3 minVal, maxVal;
    [SerializeField]
    float smooth = 10;
    private void FixedUpdate()
    {   
        if( target != null)
        {            
            Vector3 targetPositon = target.position + offset;
            Vector3 boundPosition = new Vector3(Mathf.Clamp(targetPositon.x, minVal.x, maxVal.x),
                                Mathf.Clamp(targetPositon.y, minVal.y, maxVal.y), -10);
            transform.position = Vector3.Lerp(transform.position, boundPosition, smooth * Time.fixedDeltaTime);
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
   IEnumerator finishAnimator()
   {
       yield return new WaitForSeconds(0.5f);
       Destroy(gameObject);
   }
    
}

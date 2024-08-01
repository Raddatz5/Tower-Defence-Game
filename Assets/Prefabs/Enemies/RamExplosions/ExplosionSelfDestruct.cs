using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSelfDestruct : MonoBehaviour
{   
    ParticleSystem particleSystemA;

    void Awake()
    {
         particleSystemA = GetComponent<ParticleSystem>();
    }
    void OnEnable() 
    {  
        particleSystemA.Play();
        StartCoroutine(SelfDestruct());
    }
    
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        gameObject.transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
}

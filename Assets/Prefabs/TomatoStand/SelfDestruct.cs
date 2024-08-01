using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float destroyTimer = 3f;
    
   ParticleSystem particleSystemA;

    void Awake()
    {
         particleSystemA = GetComponent<ParticleSystem>();
    }
    void OnEnable() 
    {  
        particleSystemA.Play();
        StartCoroutine(SelfDestructA());
    }
    
IEnumerator SelfDestructA()
    {   
        yield return new WaitForSeconds(destroyTimer);
        gameObject.transform.position = transform.parent.position;
        gameObject.SetActive(false);
    } 
void OnDisable()
    {
        StopCoroutine(SelfDestructA());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    GameObject target;
    float speed;
    ArrowManager arrowManager;
    void Start()
    {
        arrowManager = GetComponentInParent<ArrowManager>();
    }
    void OnEnable()
    {  if(arrowManager!=null)
        {
        speed = arrowManager.Speed;
        target = arrowManager.TempTarget;
        StartCoroutine(Follow());
        }
    }

    IEnumerator Follow()
    {   
        while(true)
        {   
            transform.LookAt(target.transform.position);
            transform.position += transform.forward * speed * Time.deltaTime;
            yield return null;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
  }

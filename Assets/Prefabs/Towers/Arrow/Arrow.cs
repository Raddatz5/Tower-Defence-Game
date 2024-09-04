using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]GameObject target;
    float speed;
    ArrowManager arrowManager;
    Vector3 initialPosition;
    void Awake()
    {
        arrowManager = GetComponentInParent<ArrowManager>();
    }
    void OnEnable()
    {  if(arrowManager!=null)
        {
        speed = arrowManager.Speed;
        target = arrowManager.TempTarget;
        transform.position = arrowManager.transform.position;
        }
      
    }

      void Update()
      {     
            initialPosition = transform.position;
            if(target != null)
            {
                transform.LookAt(target.transform.position + new Vector3(0,3f,0));
            }
            transform.position = initialPosition + transform.forward * speed * Time.deltaTime;        
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    public void WhatsMyTarget(GameObject targetRef)
    {
        target = targetRef;
    }   
  }

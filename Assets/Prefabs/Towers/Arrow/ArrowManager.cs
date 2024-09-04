using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{   [SerializeField] int poolSize = 5;
    GameObject [] pool; 
    [SerializeField] GameObject arrow;
    [SerializeField] float speed;
    public float Speed { get { return speed;}}
    GameObject tempTarget;
    public GameObject TempTarget { get { return tempTarget; } }
    Transform tempParent;
       
    
    void Awake()
    {
        PopulatePool();
        tempParent = FindAnyObjectByType<Parent>().transform;
    }

    private void PopulatePool()
    {
        pool = new GameObject[poolSize];

        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(arrow,transform);
            pool[i].SetActive(false);
        }
    }

    public void Fire(GameObject target)
    {   tempTarget = target;
        for (int i = 0; i < pool.Length; i++)
        {
            if(!pool[i].activeSelf)
            {   Arrow arrow;
                pool[i].SetActive(true);
                pool[i].transform.SetParent(tempParent,true);
                arrow = pool[i].GetComponent<Arrow>();
                arrow.WhatsMyTarget(target);
                break;
            }
        }
    }
}

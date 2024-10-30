using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BarrelManager : MonoBehaviour
{
    public GameObject barrel1;
    GameObject [] barrelPool1;
    public int poolSize = 5;
    public float speed = 2f;
    public float Speed { get { return speed;}}
    public Transform barrelMount;

    void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {   
        barrelPool1 = new GameObject[poolSize];
        for(int i = 0; i < poolSize;i++)
        {
            barrelPool1 [i] = Instantiate(barrel1, transform);
            barrelPool1 [i].SetActive(false);
        }
    }

    public IEnumerator BarrelLaunch()
    {  
    
        for(int i =0;i < barrelPool1.Length;i++)
        {   
            if(!barrelPool1[i].activeInHierarchy)
            {            
                barrelPool1[i].transform.position = barrelMount.position;
                barrelPool1[i].SetActive(true);
                break;
            }
             else{Debug.Log("One was active");}
        }  
        yield return null;
    }

}

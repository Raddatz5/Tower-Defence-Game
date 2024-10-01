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
    Animator animator;
    [SerializeField] GameObject reloadArrowBot;
    [SerializeField] GameObject reloadArrowTop;
    [SerializeField] Transform reloadParent;
    MeshRenderer reloadArrowMeshBot;
    MeshRenderer reloadArrowMeshTop;
       
    
    void Awake()
    {
        PopulatePool();
        tempParent = FindAnyObjectByType<Parent>().transform;
        reloadArrowMeshBot = reloadArrowBot.GetComponent<MeshRenderer>();
        reloadArrowMeshTop = reloadArrowTop.GetComponent<MeshRenderer>();
    }
    void OnEnable()
    {
        reloadArrowMeshTop.enabled = true;
        reloadArrowMeshBot.enabled = false;
        // reloadArrowBot.transform.SetParent(this.transform,false);
    }

    void Start()
    {
        animator = GetComponentInParent<Animator>();
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
    {   reloadArrowMeshBot.enabled = false;
        reloadArrowMeshTop.enabled = false;
        tempTarget = target;
        animator.SetTrigger("Fire");
        StartCoroutine(Reset());
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

    public void SetArrowReload()
{
    reloadArrowMeshBot.enabled = true;
    reloadArrowMeshTop.enabled = false;
    // reloadArrowBot.transform.SetParent(reloadParent, false);
}
       public void ReloadDone()
{   
    reloadArrowMeshBot.enabled = false;
    reloadArrowMeshTop.enabled = true;
    // reloadArrowBot.transform.SetParent(this.transform,true);
}
    IEnumerator Reset()
    {   
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("Fire");
    }
 void OnDisable() { StopAllCoroutines();
        
    }
}

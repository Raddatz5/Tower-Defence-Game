using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBasicAnimationController : MonoBehaviour
{   EnemyMover enemyMover;
    Animator animator;
    
    void Start()
    {
        enemyMover = GetComponent<EnemyMover>();
        animator = GetComponent<Animator>();
    }

    public void Move()
        {
            animator.SetBool("Move",true);
        }
    public void Stop()
        {   
            animator.SetBool("Move",false);
        }
    
    public void Attack()
    {   float rand = Random.Range(0f,1f);
        Debug.Log(rand);
        if (rand > 0.5f)
        {animator.SetTrigger("Attack1");}
        else {animator.SetTrigger("Attack2");}
        StartCoroutine(Reset());
        
    }
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
    }
 void OnDisable() { StopAllCoroutines();
        
    }
}

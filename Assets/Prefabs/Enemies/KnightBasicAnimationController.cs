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


    void Update()
    {   if(enemyMover.ImAttacking)
    {
        animator.SetTrigger("Attack1");
    }
        if(enemyMover.ImMoving)
        {
            animator.SetTrigger("Move");
        }
        else
        {
            animator.SetTrigger("Stop");
        }
    }
}

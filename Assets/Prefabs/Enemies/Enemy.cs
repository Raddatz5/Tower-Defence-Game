using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int goldAmount = 25;
    // [SerializeField] int goldPenalty = 25;
    [SerializeField] float ramDamage = 10f;
    public float RamDamage { get { return ramDamage;}}
    Gold gold;

    void Start()
    {
        gold = FindObjectOfType<Gold>();
    }

    public void RewardGold()
    {   if(gold == null){return;}
        gold.AddToGold(goldAmount);
    }

    //  public void StealGold()
    // {   if(gold == null){return;}
    //     gold.Withdraw(goldPenalty);
    // }


   
}

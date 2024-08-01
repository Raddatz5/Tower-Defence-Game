using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCost : MonoBehaviour
{
    [SerializeField] int goldCost = 25;
    public int HowMuch{get{return goldCost;}}
}

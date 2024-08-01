using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightManager : MonoBehaviour
{
    [SerializeField] GameObject knightPrefab;
    [SerializeField] [Range(0, 20)] int sizeOfPool;
    GameObject [] knightPool;

    void Awake()
    {   
        PopulatePool();
    }

    void PopulatePool()
    {   
        knightPool = new GameObject[sizeOfPool];

        for(int i = 0;i < sizeOfPool;i++)
        {
            knightPool[i] = Instantiate(knightPrefab, transform);
            knightPool[i].SetActive(false);
        }
    }
}

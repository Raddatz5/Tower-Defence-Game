using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    [SerializeField] int poolSize = 50;
   [SerializeField] GameObject ramExplosion1;
   [SerializeField] GameObject ramExplosion2;
   [SerializeField] GameObject ramExplosion3;
   [SerializeField] GameObject ramExplosion4;
   [SerializeField] GameObject ramExplosion5;
   [SerializeField] GameObject ramExplosion6;
   [SerializeField] GameObject shieldExplosion;
   [SerializeField] GameObject knightExplosion;
   List<GameObject> explosionIndex = new();
   GameObject[][] explosionPools;
    Vector2Int arrayPosition;

   void Awake()
   {   
    CreateExplosionIndex();
    PopulatePool();
   }

void CreateExplosionIndex()
{
    explosionIndex.Add(ramExplosion1);
    explosionIndex.Add(ramExplosion2);
    explosionIndex.Add(ramExplosion3);
    explosionIndex.Add(ramExplosion4);
    explosionIndex.Add(ramExplosion5);
    explosionIndex.Add(ramExplosion6);
    explosionIndex.Add(shieldExplosion);
    explosionIndex.Add(knightExplosion);
    
}
   void PopulatePool()
   {
         explosionPools = new GameObject[explosionIndex.Count][];
                
        for(int j = 0; j < explosionIndex.Count; j++)
        {   
            explosionPools[j] = new GameObject[poolSize];
            for (int i = 0; i<poolSize; i++)
            {
                explosionPools[j][i]= Instantiate(explosionIndex[j],transform);
                explosionPools[j][i].SetActive(false);
                arrayPosition = new Vector2Int(j,i);
            }
        }
   }

   public void SpawnExplosion(int enemyBigNumber, Vector3 destroyedObjectPosition)
    {     
        for(int i = 0; i < poolSize; i++)
            {   
                if(!explosionPools[enemyBigNumber][i].activeSelf)
                {   
                    explosionPools[enemyBigNumber][i].transform.position = destroyedObjectPosition;
                    explosionPools[enemyBigNumber][i].SetActive(true);
                    break; 
                }  
            }

     }

    
}

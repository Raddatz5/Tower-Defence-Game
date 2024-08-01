using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject spawnObject;

    Vector3 positionAtDisable;

    public void IGotDisabled()
    {
        //record the parent objcect position
        positionAtDisable = parentObject.transform.position;

        // pull a certain amount of objects from the pool

        // tell the objects where their startposition will be
    }


}

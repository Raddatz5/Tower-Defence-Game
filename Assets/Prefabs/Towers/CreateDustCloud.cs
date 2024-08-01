using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDustCloud : MonoBehaviour
{   SlamMover slamMover;


    void Start()
    {
        slamMover = GetComponentInParent<SlamMover>();
    }

     public void DustCloudChild()
    {
        slamMover.StartDustCLoud();
    }
    public void ChangeBoolChild()
    {
        slamMover.ChangeBool();
    }
}

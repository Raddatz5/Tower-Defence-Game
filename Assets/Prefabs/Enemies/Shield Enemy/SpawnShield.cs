using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnShield : MonoBehaviour
{
    [SerializeField] GameObject shield;
    ShieldRotate shieldRotate;
    GameObject myShield = null;


    void OnEnable()
    {   
        Vector3 offset = new Vector3(0, 2.8f, 3.6f);
        myShield = Instantiate(shield,transform.position + offset,quaternion.identity);
        shieldRotate = myShield.GetComponent<ShieldRotate>();
        shieldRotate.AssignRotation(transform);
    }
    void OnDisable()
    {
        GameObject.Destroy(myShield);
    }
}

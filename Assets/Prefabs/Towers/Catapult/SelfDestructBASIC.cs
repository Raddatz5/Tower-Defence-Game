using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructBASIC : MonoBehaviour
{   
    [SerializeField] float timer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Boom());
    }
IEnumerator Boom()
{
    yield return new WaitForSeconds(timer);
    Destroy(gameObject);
}
   
}

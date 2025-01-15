using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{

    float ballSpeed;
    float fuseTime;
    Rigidbody rb;
    bool move = true;
    Vector3 direction;
    ButtonManager buttonManager;
    CannonBarrell cb;
    [SerializeField] float ballRadius = 0.5f;
    [SerializeField] GameObject explosionSmall;
    [SerializeField] GameObject explosionBig;
    [SerializeField] GameObject explosionBiggest;
    Transform refenceParent;
    MeshRenderer meshRenderer;
    SphereCollider sphereCollider;
    
 
// TO DO: Adjust height based on ballradius
    void Awake()
    {   
        rb = GetComponent<Rigidbody>();
        cb = GetComponentInParent<CannonBarrell>();
        buttonManager = FindAnyObjectByType<ButtonManager>();
        refenceParent = gameObject.transform.parent;
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
      
        // rb.velocity = initialDirection*ballSpeed;
    }
    void OnEnable()
    {   meshRenderer.enabled = true;
        explosionSmall.SetActive(false);
        explosionBig.SetActive(false);
        explosionBiggest.SetActive(false);

        StartCoroutine(MoveTimer());
        direction = transform.forward;
        ballSpeed = cb.BallSpeed;
        fuseTime = cb.FuseTime;
        ballRadius = cb.BallRadius;
        SetRadius(ballRadius);
    }

    void FixedUpdate() 
    {   
        if (rb != null && move) 
        { 
            // rb.velocity = rb.velocity.normalized * ballSpeed;
            rb.MovePosition(rb.position + direction.normalized*ballSpeed*Time.fixedDeltaTime);
        } 
    }
    IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(fuseTime);
        SpawnExplosion();
        meshRenderer.enabled = false;
        move = false;
        yield return new WaitForSeconds(3f);
        ReparentAndDisable();
    }

    private void SpawnExplosion()
    {   
        explosionSmall.transform.position = transform.position;
        explosionSmall.SetActive (true);
        explosionSmall.GetComponent<ParticleSystem>().Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("CannonWall"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void ReparentAndDisable()
    {   
        if(transform.parent != refenceParent)
        {
        gameObject.transform.SetParent(refenceParent, false);
        }
        gameObject.SetActive(false);
    }
    public void SetRadius(float newRadius)
    {
        // Update the radius
        ballRadius = newRadius;

        // Adjust the scale of the GameObject to reflect the new radius for the MeshRenderer
        transform.localScale = Vector3.one * ballRadius * 2f; // Multiply by 2 because scale is diameter

    }
}

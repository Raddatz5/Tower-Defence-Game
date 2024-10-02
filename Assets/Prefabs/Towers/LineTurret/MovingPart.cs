using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPart : MonoBehaviour
{   
    Upgrade upgrade;
    LineTurret lineTurret;
    Vector3 startPoint1;
    Vector3 endPoint1;
    Vector3 point1;
    Vector3 point2;
    float speed;
    [SerializeField] float timePause = 1.5f;
    float distance;
    Rigidbody rb;

    void Awake()
    {
        upgrade = GetComponentInParent<Upgrade>();
        lineTurret = GetComponentInParent<LineTurret>();
        rb = GetComponentInParent<Rigidbody>();
    }

    void OnEnable()
    {
        point1 = lineTurret.Tower1AttachPoint.position;
        point2 = lineTurret.Tower2AttachPoint.position;
        distance = Vector3.Distance(point1, point2);
        speed = lineTurret.Speed;
        transform.LookAt(point2);
        //move towards next point
        StartCoroutine(MoveTowardsNext());

    }   

    IEnumerator MoveTowardsNext()
    {   rb.isKinematic = false;
        float t = 0;
        float elapsedTime = 0;

        while(t<1)
        {
            t += elapsedTime*speed/distance*0.1f;
            elapsedTime += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(point1, point2, t));
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(ChangeDirection());
    }

    IEnumerator ChangeDirection()
    {   rb.isKinematic = true;
        yield return new WaitForSeconds(timePause);
        //tuple to swap values
        (point2, point1) = (point1, point2);
        StartCoroutine(MoveTowardsNext());
    }
}

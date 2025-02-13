using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float moveSpeed = 40f;
    float scrollSpeed = 15f;
    float rotationSpeed = 50f;
    float minY = 50f;
    float maxY = 85f;
    float minRotation = -45f;
    float maxRotation = 45f;
     float minX = -40f;
    float maxX = 40f;
    float minZ = -30f;
     float maxZ = 30f;
     float rotationX;
     bool stopMoving = false;

void Start()
{
    minX = transform.position.x + minX;
    maxX = transform.position.x + maxX;
    minZ = transform.position.z + minZ;
    maxZ = transform.position.z + maxZ;
    minRotation = transform.rotation.eulerAngles.y + minRotation;
    maxRotation = transform.rotation.eulerAngles.y + maxRotation;
    rotationX = transform.rotation.eulerAngles.x;
}

    void Update()
    {   
        if (!stopMoving)
        {
        // Check if rotating
        bool isRotating = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));

        if (!isRotating)
        {
            // Movement with W, A, S, D
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Move forward and backward (W, S)
           Vector3 forwardMovement = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * vertical * moveSpeed * Time.deltaTime;

            // Strafe left and right (A, D)
            Vector3 strafeMovement = transform.right * horizontal * moveSpeed * Time.deltaTime;

            Vector3 move = forwardMovement + strafeMovement;
            Vector3 newPositionA = transform.position + move;

            // Clamping the X and Z position
            newPositionA.x = Mathf.Clamp(newPositionA.x, minX, maxX);
            newPositionA.z = Mathf.Clamp(newPositionA.z, minZ, maxZ);

            transform.position = newPositionA;
                        
         }
         // Rotation with Shift + A / Shift + D
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            float rotate = 0;
            if (Input.GetKey(KeyCode.A))
            {
                rotate = -rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                rotate = rotationSpeed * Time.deltaTime;
            }
           float newRotation = transform.eulerAngles.y + rotate;
           newRotation = (newRotation > 180) ? newRotation - 360 : newRotation; // Normalize angle to [-180, 180]
           newRotation = Mathf.Clamp(newRotation, minRotation, maxRotation);
           
           transform.eulerAngles = new Vector3(rotationX, newRotation, 0);
        }
    }
        
        // Up and down movement with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 scrollMove = new Vector3(0, -scroll * scrollSpeed, 0);
        Vector3 newPosition = transform.position + scrollMove;
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;

        
    }

public void FreezeCamera(bool trigger)
{
    stopMoving = trigger;
}

}

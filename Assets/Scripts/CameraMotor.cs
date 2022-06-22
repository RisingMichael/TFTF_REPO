using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{

    public Transform lookAt;
    public float boundX = 0.15f;
    public float boundY = 0.05f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called after update and fixed update
    void LateUpdate()
    {
        // Move camera after Player ==> in LateUpdate
        Vector3 delta = Vector3.zero;


        // Check if inside bounds on x-Axis
        float deltaX = lookAt.position.x - transform.position.x;
        if(deltaX > boundX || deltaX < -boundX)
        {
            if(transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }


        // Check if inside bounds on y-Axis
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        // Move Camera
        transform.position += new Vector3(delta.x, delta.y, 0);
    }
}

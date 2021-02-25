using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLine : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float shiftSpeed = 15f;

    public float offLimit = 7.5f;



    void Update()
    {
        float currentSpeed = normalSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = shiftSpeed;

        if (Input.GetKey(KeyCode.A))
        {
            if (transform.position.x >= -offLimit)
                MoveRight(-currentSpeed);
        }
        if(Input.GetKey(KeyCode.D))
        {
            if (transform.position.x <= offLimit)
                MoveRight(currentSpeed);
        }




    }
    void MoveRight(float speed)
    {
      
            float newPosition = transform.position.x + speed * Time.deltaTime;
            transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
        

    }

}

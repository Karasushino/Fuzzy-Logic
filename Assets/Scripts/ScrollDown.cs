using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDown : MonoBehaviour
{
    public float top = 50f;
    public float bottom = -6f;
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        //Scroll down
        if (transform.position.z >= bottom)
            MoveDown();
        else
            ResetToTop();



    }

    void MoveDown()
    {
        float newValue = transform.position.z - speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, transform.position.y, newValue);
    }

    void ResetToTop()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, top);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class FSMBoat : MonoBehaviour
{
    public Transform lineObject;

    public float boatMaxTurn = 30f;
    public float maxRotationDistance = 5f;
    public float maxDistance = 9f;
    public float speed = 120f;
    public float maxSpeed = 15f;


    //Privates 
    float distanceToLine;
    Rigidbody body;

    [Header("UI Elements")]
    public Slider[] slider;
    public Text[] text;

enum State
    {
        Left,
        Right,
        Middle
    };

    State state = State.Middle;

    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        //Default force to apply;
    }

    //Update x frame
    private void Update()
    {
        //Calculate the distance from the boat to the Line.
        distanceToLine = transform.position.x - lineObject.position.x;
       
        //Decide the State.
        //Move right State
        if (distanceToLine < -1f)
            state = State.Right;
        //Move left State
        else if (distanceToLine > 1f)
            state = State.Left;
        else
            state = State.Middle;

        text[0].text = state.ToString();
        float displayDistance = distanceToLine / maxDistance;
        displayDistance = Mathf.Clamp(displayDistance, -1, 1);
        text[1].text = "Distance: " + Round(displayDistance, 3);
        slider[0].value = displayDistance;
    }

    //Phyisics update at 50hz.
    void FixedUpdate()
    {
       
        //Let the state machine decide the behaviour of the boat.
        PerformState(state);

        //Rotate the boat to have a more realistic rotation.
        RotateBasedOnDistance(distanceToLine);

    }

    //FSM behaviour.
    void PerformState(State state)
    {
        float forceToApply = speed;
        //Traveling fast Modifier State
        if (Mathf.Abs(body.velocity.x) < maxSpeed * 1.25f)
        {
            forceToApply *= 1.25f;
            text[3].text = "Yes";
        }
        else //Extra speed UI.
            text[3].text = "No";


        switch (state)
        {
            case State.Left:
                body.AddForce(new Vector3(-forceToApply, 0f, 0f));
                break;
            case State.Right:
                body.AddForce(new Vector3(forceToApply, 0f, 0f));
                break;
            case State.Middle:
                //Literally do nothing let physics deal with drag to stop the boat.
                break;
            default:
                Debug.Log("Something went wrong");
                break;
        }

        //UI Velocity display.
        float displayVelocity = body.velocity.x / (maxSpeed * 1.25f);
        displayVelocity = Mathf.Clamp(displayVelocity, -1f, 1f);
        text[2].text = "Velocity: " + Round(displayVelocity, 3);
        slider[1].value = displayVelocity;

       

    }

    void RotateBasedOnDistance(float distance)
    {
        //Normalize the value to set as multiplier of rotation;
        //7 just arbitrary maximum distance that the boat is ever going to be at.
        float normalized = distance / maxRotationDistance;
        float newRotation = 90f + boatMaxTurn * -normalized;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);
    }

    static double Round(double value, int digits)
    {
        double mult = Mathf.Pow(10.0f, (float)digits);
        return Math.Round(value * mult) / mult;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;
using UnityEngine.UI;
using System;

public class FuzzyBoat : MonoBehaviour
{

    IFuzzyEngine engine;

    //Input 
    LinguisticVariable velocity;
    LinguisticVariable distance;

    //Output
    LinguisticVariable Steering;


    //Public var
    public float boatMaxTurn = 30f;
    public float maxRotationDistance = 5f;
    public float maxDistance = 9f;
    public float forceScalar = 70f;
    public float maxSpeed = 15f;

    //Boat Rigid body
    Rigidbody body;

    //Line object reference
    public Transform lineObject;

    //References to UI elements
    [Space(5)]
    [Header("UI Elements")]
    public Slider[] slider;
    public Text[] text;

    void Start()
    {
        //Monobehaviour Start
        body = this.gameObject.GetComponent<Rigidbody>();
        
        engine = new FuzzyEngineFactory().Create(FuzzyEngineType.CoG);


        //Input variables
        distance = new LinguisticVariable("distance");
        velocity = new LinguisticVariable("velocity");

        //Output Variable
        Steering = new LinguisticVariable("Steering");

        //Input Fuzzy Sets

        //Distance membership functions
        var leftDistance = distance.MembershipFunctions.AddTriangle("leftDistance", -5, -1, -0.1);
        var noneDistance = distance.MembershipFunctions.AddTriangle("noneDistance", -0.15, 0, 0.15);
        var rightDistance = distance.MembershipFunctions.AddTriangle("rightDistance", 0.1, 1, 5);

        //Speed membership functions
        var slow = velocity.MembershipFunctions.AddTriangle("slow", -2, -1, 1);
        var average = velocity.MembershipFunctions.AddTriangle("average", 0, 0.5, 1);
        var fast = velocity.MembershipFunctions.AddTriangle("fast", -1, 1, 2);

        //Output Fuzzy set
        //Steering membership functions
        var strongLeftSteering = Steering.MembershipFunctions.AddTriangle("strongLeftSteering", -2, -1, -0.8);
        var leftSteering = Steering.MembershipFunctions.AddTriangle("leftSteering", -0.8, -0.5, -0.1);
        var noneSteering = Steering.MembershipFunctions.AddTriangle("noneSteering", -0.1, 0, 0.1);
        var rightSteering = Steering.MembershipFunctions.AddTriangle("rightSteering", 0.1, 0.5, 0.8);
        var strongRightSteering = Steering.MembershipFunctions.AddTriangle("strongRightSteering", 0.8, 1, 2);


        //FIS Rules
        var rule1 = Rule.If(distance.Is(rightDistance)).If(velocity.Is(fast)).Then(Steering.Is(strongLeftSteering));
        var rule2 = Rule.If(distance.Is(rightDistance)).If(velocity.Is(slow)).Then(Steering.Is(leftSteering));
        var rule3 = Rule.If(distance.Is(noneDistance)).If(velocity.Is(fast)).Then(Steering.Is(noneSteering));
        var rule4 = Rule.If(distance.Is(noneDistance)).If(velocity.Is(slow)).Then(Steering.Is(noneSteering));
        var rule5 = Rule.If(distance.Is(leftDistance)).If(velocity.Is(fast)).Then(Steering.Is(strongRightSteering));
        var rule6 = Rule.If(distance.Is(leftDistance)).If(velocity.Is(slow)).Then(Steering.Is(rightSteering));
        var rule7 = Rule.If(distance.Is(noneDistance)).If(velocity.Is(average)).Then(Steering.Is(noneSteering));
        var rule8 = Rule.If(distance.Is(leftDistance)).If(velocity.Is(average)).Then(Steering.Is(rightSteering));
        var rule9 = Rule.If(distance.Is(rightDistance)).If(velocity.Is(average)).Then(Steering.Is(leftSteering));


        //Add rules to the FIS Engine
        engine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8,rule9);


    }

    void FixedUpdate()
    {
        //Calculate the distance from the boat to the Line.
        double distanceToLine = transform.position.x - lineObject.position.x;

        //Normalize it to be a value between -1 and 1. (The max Distance is the max distance that the boat can be from the line.)
        //The max distance acts as a sensitivity value for the fuzzy logic output.
        double normalizedDistance = distanceToLine / maxDistance;
        normalizedDistance = Mathf.Clamp((float)normalizedDistance, -1f, 1f); //Clamp it just in case

        //Function where the boat will tilt based on the distance from the line. (The further the more it tilts)
        RotateBasedOnDistance((float)distanceToLine);

        double normalizeVelocity = body.velocity.x / maxSpeed;
        normalizeVelocity = Mathf.Clamp((float)normalizeVelocity, -1f, 1f);

        //Defuzzify.
        double result = engine.Defuzzify(new { distance = normalizedDistance, velocity = normalizeVelocity });
        result = Mathf.Clamp((float)result, -1f, 1f);


        //Apply the force to the boat.
        float forceToApply = (float)result * forceScalar;
        body.AddForce(new Vector3(forceToApply, 0f, 0f));

        //UI Values
        slider[0].value = (float)normalizedDistance;
        slider[1].value = (float)normalizeVelocity;
        slider[2].value = (float)result;

        text[0].text = "Distance: " + Round(normalizedDistance,3);
        text[1].text = "Velocity: " + Round(normalizeVelocity,3);
        text[2].text = "Steering: " + Round(result,3);

    }

    void RotateBasedOnDistance(float distance)
    {
        //Normalize the value to set as multiplier of rotation;
        //7 just arbitrary maximum distance that the boat is ever going to be at.
        float normalized = distance / maxRotationDistance;
        float newRotation = 90f + boatMaxTurn * -normalized;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);
    }

    void RotateBasedOnResult(float result)
    {

        float newRotation = 90f + boatMaxTurn * result;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);
    }

    static double Round(double value, int digits)
    {
        double mult = Mathf.Pow(10.0f, (float)digits);
        return Math.Round(value * mult) / mult;

    }

}


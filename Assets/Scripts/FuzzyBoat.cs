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

    LinguisticVariable velocity;
    LinguisticVariable distance;

    //Output
    LinguisticVariable stearing;

    public Transform lineObject;

    public float boatMaxTurn = 30f;
    public float maxRotationDistance = 5f;
    public float maxDistance = 9f;
    public float forceScalar = 70f;
    public float maxSpeed = 15f;

    Rigidbody body;

    [Space(5)]
    [Header("UI Elements")]
    public Slider[] slider;
    public Text[] text;

    void Start()
    {
        engine = new FuzzyEngineFactory().Default();


        //Input variables
        distance = new LinguisticVariable("distance");
        var leftDistance = distance.MembershipFunctions.AddTriangle("leftDistance", -5, -1, -0.1);
        var noneDistance = distance.MembershipFunctions.AddTriangle("noneDistance", -0.15, 0, 0.15);
        var rightDistance = distance.MembershipFunctions.AddTriangle("rightDistance", 0.1, 1, 5);

        velocity = new LinguisticVariable("velocity");
        var slow = velocity.MembershipFunctions.AddTriangle("slow", -2, -1, 1);
        var fast = velocity.MembershipFunctions.AddTriangle("fast", -1, 1, 2);

        //Output
        stearing = new LinguisticVariable("stearing");
        var strongLeftStearing = stearing.MembershipFunctions.AddTriangle("strongLeftStearing", -2, -1, -0.8);
        var leftStearing = stearing.MembershipFunctions.AddTriangle("leftStearing", -0.8, -0.5, -0.1);
        var noneStearing = stearing.MembershipFunctions.AddTriangle("noneStearing", -0.1, 0, 0.1);
        var rightStearing = stearing.MembershipFunctions.AddTriangle("rightStearing", 0.1, 0.5, 0.8);

        var strongRightStearing = stearing.MembershipFunctions.AddTriangle("strongRightStearing", 0.8, 1, 2);


        //var rule1 = Rule.If(distance.Is(rightDistance)).Then(stearing.Is(leftStearing));
        //var rule2 = Rule.If(distance.Is(noneDistance)).Then(stearing.Is(noneStearing));
        //var rule3 = Rule.If(distance.Is(leftDistance)).Then(stearing.Is(rightStearing));

        //engine.Rules.Add(rule1, rule2, rule3);

        var rule1 = Rule.If(distance.Is(rightDistance)).If(velocity.Is(fast)).Then(stearing.Is(strongLeftStearing));
        var rule2 = Rule.If(distance.Is(rightDistance)).If(velocity.Is(slow)).Then(stearing.Is(leftStearing));
        var rule3 = Rule.If(distance.Is(noneDistance)).If(velocity.Is(fast)).Then(stearing.Is(noneStearing));
        var rule4 = Rule.If(distance.Is(noneDistance)).If(velocity.Is(slow)).Then(stearing.Is(noneStearing));
        var rule5 = Rule.If(distance.Is(leftDistance)).If(velocity.Is(fast)).Then(stearing.Is(strongRightStearing));
        var rule6 = Rule.If(distance.Is(leftDistance)).If(velocity.Is(slow)).Then(stearing.Is(rightStearing));

        engine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

        body = gameObject.GetComponent<Rigidbody>();

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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

public class FuzzyBoat : MonoBehaviour
{

    IFuzzyEngine engine;
    LinguisticVariable distance;
    LinguisticVariable direction;

    public Transform lineObject;

    public float boatMaxTurn = 15f;
    public float maxRotationDistance = 7f;
    public float maxDistance = 10f;
    public float forceScalar = 30f;

    void Start()
    {
        engine = new FuzzyEngineFactory().Default();


        // Here we need to setup the Fuzzy Inference System
        distance = new LinguisticVariable("distance");
        var leftDistance = distance.MembershipFunctions.AddTriangle("leftDistance", -5, -1, -0.15);
        var noneDistance = distance.MembershipFunctions.AddTriangle("noneDistance", -0.2, 0, 0.2);
        var rightDistance = distance.MembershipFunctions.AddTriangle("rightDistance", 0.15, 1, 5);

        direction = new LinguisticVariable("direction");
        var leftDirection = direction.MembershipFunctions.AddTriangle("leftDirection", -2, -1, -0.2);
        var noneDirection = direction.MembershipFunctions.AddTriangle("noneDirection", -0.2, 0, 0.2);
        var rightDirection = direction.MembershipFunctions.AddTriangle("rightDirection", 0.2, 1, 2);


        var rule1 = Rule.If(distance.Is(rightDistance)).Then(direction.Is(leftDirection));
        var rule2 = Rule.If(distance.Is(leftDistance)).Then(direction.Is(rightDirection));
        var rule3 = Rule.If(distance.Is(noneDistance)).Then(direction.Is(noneDirection));

        engine.Rules.Add(rule1, rule2, rule3);

    }

    void FixedUpdate()
    {
        //Calculate the distance from the boat to the Line.
        double distanceToLine = transform.position.x - lineObject.position.x;

        //Normalize it to be a value between -1 and 1. (The max Distance is the max distance that the boat can be from the line.)
        //The max distance acts as a sensitivity value for the fuzzy logic output.
        double normalizedDistance = distanceToLine / maxDistance;

        //Function where the boat will tilt based on the distance from the line. (The further the more it tilts)
        RotateBasedOnDistance((float)distanceToLine);

        double result = engine.Defuzzify(new { distance = normalizedDistance });


        //RotateBasedOnResult((float)result);
        Debug.Log("Result: " + result);

        float forceToApply = (float)result * forceScalar;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(forceToApply, 0f, 0f));
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

}


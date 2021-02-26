using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

public class FuzzyBox : MonoBehaviour
{

    bool selected = false;
    IFuzzyEngine engine;
    LinguisticVariable distance;

    LinguisticVariable direction;

    public Transform lineObject;

    public float verticalForceScalar = 1f;

    public float boatMaxTurn = 15f;
    public float maxDistance = 7f;

    void Start()
    {
        engine = new FuzzyEngineFactory().Default();


        // Here we need to setup the Fuzzy Inference System
        distance = new LinguisticVariable("distance");
        var rightDistance = distance.MembershipFunctions.AddTriangle("rightDistance", -15, 0, 0);
        var noneDistance = distance.MembershipFunctions.AddTriangle("noneDistance", -1, 0, 1);
        var leftDistance = distance.MembershipFunctions.AddTriangle("leftDistance", 0, 0, 15);

        direction = new LinguisticVariable("direction");
        var rightDirection = direction.MembershipFunctions.AddTriangle("rightDirection", -100, 0, 0);
        var noneDirection = direction.MembershipFunctions.AddTriangle("noneDirection", -15, 0, 15);
        var leftDirection = direction.MembershipFunctions.AddTriangle("leftDirection", 0, 0, 100);


        var rule1 = Rule.If(distance.Is(rightDistance)).Then(direction.Is(leftDirection));
        var rule2 = Rule.If(distance.Is(leftDistance)).Then(direction.Is(rightDirection));
        var rule3 = Rule.If(distance.Is(noneDistance)).Then(direction.Is(noneDirection));

        engine.Rules.Add(rule1, rule2, rule3);

    }

    void FixedUpdate()
    {

        // Convert position of box to value between 0 and 100
        double distanceToLine = transform.position.x - lineObject.position.x;
        RotateBasedOnDistance((float)distanceToLine);

        double result = engine.Defuzzify(new { distance = distanceToLine, });

        Debug.Log("Result: "+ result);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3((float)(result), 0f, 0f));

    }

    void RotateBasedOnDistance(float distance)
    {
        //Normalize the value to set as multiplier of rotation;
        //7 just arbitrary maximum distance that the boat is ever going to be at.
        //float clampedDistance = Mathf.Clamp(distance, -maxDistance, maxDistance);
        float normalized = distance / maxDistance;

        float newRotation = 90f + boatMaxTurn * -normalized;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);


    }



}

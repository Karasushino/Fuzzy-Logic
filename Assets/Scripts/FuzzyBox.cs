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

    void Start()
    {
        engine = new FuzzyEngineFactory().Default();


        // Here we need to setup the Fuzzy Inference System
        distance = new LinguisticVariable("distance");
        var rightDistance = distance.MembershipFunctions.AddTriangle("rightDistance", -100, 0, -10);
        var noneDistance = distance.MembershipFunctions.AddTriangle("noneDistance", -10, -0, 10);
        var leftDistance = distance.MembershipFunctions.AddTriangle("leftDistance", 10, 0, 100);

        direction = new LinguisticVariable("direction");
        var rightDirection = direction.MembershipFunctions.AddTriangle("rightDirection", -100, 0, -10);
        var noneDirection = direction.MembershipFunctions.AddTriangle("noneDirection", -10, 0, 10);
        var leftDirection = direction.MembershipFunctions.AddTriangle("leftDirection", 10, 0, 100);


        var rule1 = Rule.If(distance.Is(rightDistance)).Then(direction.Is(leftDirection));
        var rule2 = Rule.If(distance.Is(leftDistance)).Then(direction.Is(rightDirection));
        var rule3 = Rule.If(distance.Is(noneDistance)).Then(direction.Is(noneDirection));

        engine.Rules.Add(rule1, rule2, rule3);

    }

    void FixedUpdate()
    {
        if (!selected)
        {
            // Convert position of box to value between 0 and 100
            double distanceToLine = transform.position.x - lineObject.position.x;

            double result = engine.Defuzzify(new { distance = distanceToLine });

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(new Vector3((float)(result), 0f, 0f));
            Debug.Log("Result: " + (float)result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var hit = new RaycastHit();
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.transform.name == "FuzzyBox") Debug.Log("You have clicked the FuzzyBox");
        //        selected = true;
        //    }
        //}

        //if (Input.GetMouseButton(0) && selected)
        //{
        //    float distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        //    Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
        //    transform.position = new Vector3(curPosition.x, Mathf.Max(0.5f, curPosition.y), transform.position.z);
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    selected = false;
        //}
    }



}

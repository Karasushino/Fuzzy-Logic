using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWidth : MonoBehaviour
{
    LineRenderer line;
    public float width = 2f;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        line.widthMultiplier = width;
    }
}

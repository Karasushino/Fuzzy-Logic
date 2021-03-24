using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    // Start is called before the first frame update
    bool FSM = true;
    bool Fuzzy = true;
    bool Canvas = true;
    bool Particles = true;

    public GameObject FSMObject;
    public GameObject FuzzyObject;
    public GameObject CanvasObject;

    public GameObject[] ParticlesObject;

   public void ToggleFSM()
    {
        FSM = !FSM;
        FSMObject.SetActive(FSM);
    }
    public void ToggleFuzzy()
    {
        Fuzzy = !Fuzzy;
        FuzzyObject.SetActive(Fuzzy);
    }
    
    public void ToggleCanvas()
    {
        Canvas = !Canvas;
        CanvasObject.SetActive(Canvas);
    }

    public void ToggleParticles()
    {
        Particles = !Particles;

        foreach(GameObject obj in ParticlesObject)
        {
            obj.SetActive(Particles);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_colour : MonoBehaviour
{
    public Material material1;
    public Material material2;
    public GameObject cube;

    public void LeaverON()
    {
        cube.GetComponent<Renderer>().material = material1;
    }
   
    public void LeaverOFF()
    {
        cube.GetComponent<Renderer>().material = material2;
    }
  
}

using UnityEngine;

public class Vegetable : Ingredient
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class Grain : Ingredient
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.71f, 0.4f, 0.16f)); //Change colour depending on enum
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

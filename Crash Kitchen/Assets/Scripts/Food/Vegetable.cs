using UnityEngine;

public class Vegetable : Ingredient
{
    public bool isOutline;
    public int cleanCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isOutline)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor",new Color(0.4f, 0.49f, 0.17f, 1));
            cleanCounter = 0;
            name = "Dirty " + foodName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            cleanCounter++;
            if(cleanCounter == 3)
            {
                gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
                name = foodName;
            }
        }
    }
}

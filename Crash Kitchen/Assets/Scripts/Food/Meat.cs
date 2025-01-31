using UnityEngine;

public class Meat : Ingredient
{
    public float cookTime;
    public bool isCooked;
    public bool isCooking;
    public float cookPercent;

    void Start()
    {
        foodName = this.name;
    }

    void Update()
    {
        QualityChange();
    }

    public void StartCooking()
    {
        isCooking = true;
    }

    public void StopCooking()
    {
        isCooking = false;
    }
    public void Cook()
    {
        if (cookPercent < 100 & isCooking)
        {
            cookPercent += 10 * Time.deltaTime;
        }
    }

    public void QualityChange()
    {
        if (cookPercent > 50 & cookPercent < 100)
        {
            Debug.Log("This is happening");
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        }
        else if (cookPercent >= 100)
        {
            Debug.Log("Bruh moment");
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cooker")
        {
            StartCooking();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cooker")
        {
            Cook();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Cooker")
        {
            StopCooking();
        }
    }
}

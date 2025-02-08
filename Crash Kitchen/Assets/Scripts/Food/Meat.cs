using UnityEngine;

public class Meat : Ingredient
{   
    [Header("Cook Properties")]
    public float cookPercent;
    public int cookRate;
    public bool isCooked;
    public bool isCooking;

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
            cookPercent += cookRate * Time.deltaTime;
        }
    }

    public void QualityChange()
    {
        if (cookPercent > 50 & cookPercent < 100)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        }
        else if (cookPercent >= 100)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
            isCooked = true;
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

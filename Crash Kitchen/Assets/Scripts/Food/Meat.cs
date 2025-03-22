using UnityEngine;

public class Meat : Ingredient
{   
    [Header("Cook Properties")]
    public float cookPercent;
    public int cookRate;
    public bool isCooked;
    public bool isCooking;
    public MeatType meatType;

    public enum MeatType
    {
        Steak,
        Patty,
        Hotdog
        
    }
    void Start()
    {
        foodName = meatType.ToString();
        if(!isCooked)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1f, 0.75f, 0.79f));
        }
        else
        {
            foodName = "Cooked " + meatType.ToString();
            this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
        }
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
            switch(meatType)
            {
                case MeatType.Steak:
                    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().materials[1].SetColor("_BaseColor", Color.red);
                    break;
                case MeatType.Patty:
                    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                    break;
                case MeatType.Hotdog:
                    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().materials[1].SetColor("_BaseColor", Color.red);
                    break;
            }
        }
        else if (cookPercent >= 100 && !isCooked)
        {
            switch(meatType)
            {
                case MeatType.Steak:
                    this.gameObject.transform.GetChild(0).GetComponent<Renderer>().materials[1].SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
                    break;
                case MeatType.Patty:
                this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
                    break;
                case MeatType.Hotdog:
                this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
                    break;
            }
            isCooked = true;
            name = "Cooked " + meatType.ToString(); // Important for DishCreator
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

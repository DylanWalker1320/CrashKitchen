using UnityEngine;

public class Meat : Ingredient
{   
    [Header("Cook Properties")]
    public float cookPercent;
    public int cookRate;
    public bool isCooked;
    public bool isCooking;
    public MeatType meatType;
    public Renderer renderer;
    public enum MeatType
    {
        Steak,
        Patty,
        Hotdog
        
    }
    void Start()
    {

        renderer = GetComponentInChildren<MeshRenderer>();
        if (!renderer)
        {
            Debug.LogError("Renderer not found on " + this.name);
        }

        foodName = this.name;
        if(!isCooked)
        {
            renderer.material.SetColor("_BaseColor", new Color(1f, 0.75f, 0.79f));
        }
        else
        {
            renderer.material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
        }
    }

    void Update()
    {
        if (!renderer)
        {
            Debug.LogError("[Update]: Renderer not found on " + this.name);
        }
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
        if (!renderer)
        {
            return;
        }

        if (cookPercent > 50 & cookPercent < 100)
        {
            switch(meatType)
            {
                case MeatType.Steak:
                    renderer.materials[1].SetColor("_BaseColor", Color.red);
                    break;
                case MeatType.Patty:
                    renderer.material.SetColor("_BaseColor", Color.red);
                    break;
                case MeatType.Hotdog:                 
                    renderer.material.SetColor("_BaseColor", Color.red);
                    break;
            }
        }
        else if (cookPercent >= 100 && !isCooked)
        {
            if(MeatType.Steak == meatType)
            {
                renderer.materials[1].SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
            }
            else
            {
                renderer.material.SetColor("_BaseColor", new Color(0.4f, 0.2f, 0.1f));
            }
            isCooked = true;
            name = "Cooked " + meatType.ToString(); // Important for DishCreator
            Instantiate(Resources.Load<GameObject>("Prefabs/SPARKLE"), transform.position, new Quaternion(270f, 0f, 0f, 0f));
        }
    }

    public void OnTriggerEnter(Collider other)
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

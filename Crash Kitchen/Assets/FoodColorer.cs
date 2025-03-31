using UnityEngine;

public class FoodColorer : MonoBehaviour
{

    // Have serialized field for check marks for the different food tags
    [SerializeField] private bool isBun = false;
    [SerializeField] private bool isHotdogBun = false;
    [SerializeField] private bool isLettuce = false;
    [SerializeField] private bool isPatty = false;
    [SerializeField] private bool isGlizzy = false;
    [SerializeField] private bool isSteak = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
{
    foreach (Transform child in transform)
    {
        Color foodColor;
        
        // Determine color based on food type
        if (isBun) foodColor = new Color(0.71f, 0.4f, 0.16f);
        else if (isHotdogBun) foodColor = new Color(0.71f, 0.4f, 0.16f);
        else if (isLettuce) foodColor = new Color(0.4f, 0.49f, 0.17f, 1);
        else if (isPatty) foodColor = new Color(1f, 0.75f, 0.79f);     
        else if (isGlizzy) foodColor = new Color(1f, 0.75f, 0.79f);    
        else if (isSteak) foodColor = new Color(1f, 0.75f, 0.79f);     
        else foodColor = Color.white;
        
        child.GetComponent<Renderer>().material.SetColor("_BaseColor", foodColor);
    }
}

    // Update is called once per frame
    void Update()
    {
        
    }
}

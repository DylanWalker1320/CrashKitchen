using UnityEngine;

public class Bun : Grain
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(0.71f, 0.4f, 0.16f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.GetType().IsSubclassOf(typeof(Ingredient)))
        {
            gameObject.GetComponent<FixedJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
        }
    }
}

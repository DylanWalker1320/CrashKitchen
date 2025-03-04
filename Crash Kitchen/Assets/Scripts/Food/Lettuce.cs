using UnityEngine;

public class Lettuce : Vegetable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
    }

    private void OnCollisionEnter(Collision other)
    {
        // if(other.GetType().IsSubclassOf(typeof(Ingredient)))
        // {
        //     gameObject.GetComponent<FixedJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
        // }
    }
}

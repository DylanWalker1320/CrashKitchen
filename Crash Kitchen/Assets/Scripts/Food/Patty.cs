using UnityEngine;

public class Patty : Meat
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        // if(other.GetType().IsSubclassOf(typeof(Ingredient)))
        // {
        //     gameObject.GetComponent<FixedJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
        // }
    }
}

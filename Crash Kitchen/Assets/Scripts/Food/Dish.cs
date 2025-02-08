using UnityEngine;
using System.Collections;

[System.Serializable]
public class Dish : Food
{
    // public Ingredient[] ingredients;
    public float quality;
    public bool isBagged;

    private void Start()
    {
        if (transform.childCount > 0)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.yellow);
        }
    }
    private void Update()
    {
    }
}

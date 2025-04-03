using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using Unity.Netcode;

public class DishCreator : MonoBehaviour
{
    public DishType dishType;
    public List<string> ingredients;
    public GameObject[] hiddenIngredients; // NOTE: Use ingredient prefabs, lock them into the same position as the outlined masked ingredients, and turn them off. Add them into this array in the inspector.
    [SerializeField] RecipeDatabase recipes;
    [SerializeField] bool recipeFound;

    public enum DishType 
    {
        MegaGlizzy, 
        HealthyBurger, 
        DeluxeSteak
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        name = dishType.ToString();
        Debug.Log("Dish Name: " + name);
        recipes = FindAnyObjectByType<RecipeDatabase>();
    }

    void LookForInitialIngredients()
    {
        // Looks for the recipe's ingredients in the recipe database
        for(int i = 0; i < recipes.myRecipeList.recipes.Length; i++)
        {
            if(name == recipes.myRecipeList.recipes[i].name)
            {
                ingredients = recipes.myRecipeList.recipes[i].ingredients;
                recipeFound = true;
                break;
            }
        }
    }

    bool LookForIngredient(string ingredientToCheck)
    {
        // Validates collided ingredient is within the recipe
        foreach(string ingredient in ingredients)
        {
            if(ingredient == ingredientToCheck)
            {
                return true;
            }
        }
        return false;
    }

    void CheckForCompletion()
    {
        // Checks for completion and instantiates the dish
        foreach(GameObject ingredient in hiddenIngredients)
        {
            if(ingredient.activeSelf == false)
            {
                return;
            }
        }
        GameObject dish = Resources.Load<GameObject>("Prefabs/" + dishType.ToString());
        Instantiate(dish, gameObject.transform.position, Quaternion.identity);
        GameManager.instance.currentOrderDone = true;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Looks for initial ingredients if not found, doesn't work in start
        if(!recipeFound)
        {
            LookForInitialIngredients();
        }   
    }

    void OnTriggerEnter(Collider other)
    {
        // Checks for collided ingredient
        if(LookForIngredient(other.gameObject.transform.parent.name))
        {
            // Iterates through each hidden Ingredient ("Turned ON" ingredients currently turned off), if they're off and the collided ingredient is found, it turns the hidden ingredient on
            foreach(GameObject ingredient in hiddenIngredients)
            {
                if(ingredient.name == "Turned " + other.gameObject.transform.parent.name && ingredient.activeSelf == false)
                {
                    ingredient.SetActive(true);
                    break;
                }
            }

            // If the other collider has a network object, destroy it and remove it from the ingredients list
            if(other.gameObject.GetComponent<NetworkObject>() != null)
            {
                other.gameObject.GetComponent<NetworkObject>().Despawn(true);
                // Move the object to a far vector
                other.gameObject.transform.position = new Vector3(1000, 1000, 1000);
            }
            else
            {
                Destroy(other.gameObject.transform.parent.gameObject);
            }

            ingredients.Remove(other.gameObject.transform.parent.name);
            CheckForCompletion();
        }
    }

    //     Debug.Log(other.gameObject.GetComponent<MeshRenderer>().materials[0]);
    //     if(LookForIngredient(other.gameObject.transform.parent.name))
    //     {
    //         for(int i = 0; i < ingredients.Count; i++)
    //         {
    //             if(gameObject.transform.GetChild(i).name == other.gameObject.transform.parent.name)
    //             {
    //                 Debug.Log("Level 1");
    //                 if(gameObject.transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().materials[0] != other.gameObject.GetComponent<MeshRenderer>().materials[0])
    //                 {
    //                     foreach(Material material in gameObject.transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().materials)
    //                     {
    //                         Destroy(material);
    //                     }
    //                     gameObject.transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().materials[0] = Resources.Load<Material>("Assets/" + other.gameObject.transform.parent.name + ".obj").GetComponent<MeshRenderer>().materials[0];
    //                 }
    //             }
    //         }
    //         ingredients.Remove(other.gameObject.name);
    //         Destroy(other.gameObject.transform.parent.gameObject);
    //     }
    // }
}

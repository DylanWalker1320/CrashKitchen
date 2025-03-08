using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class DishCreator : MonoBehaviour
{
    public string dishName;
    public List<string> ingredients;
    public GameObject[] hiddenIngredients;
    [SerializeField] RecipeDatabase recipes;
    [SerializeField] bool recipeFound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recipes = FindAnyObjectByType<RecipeDatabase>();
    }


    void LookForInitialIngredients()
    {
        Debug.Log(recipes.myRecipeList.recipes.Length);
        for(int i = 0; i < recipes.myRecipeList.recipes.Length; i++)
        {
            if(dishName == recipes.myRecipeList.recipes[i].name)
            {
                ingredients = recipes.myRecipeList.recipes[i].ingredients;
                recipeFound = true;
                break;
            }
        }
    }

    bool LookForIngredient(string ingredientToCheck)
    {
        foreach(string ingredient in ingredients)
        {
            if(ingredient == ingredientToCheck)
            {
                Debug.Log("Found Ingredient");
                return true;
            }
        }
        return false;
    }

    void CheckForCompletion()
    {
        foreach(GameObject ingredient in hiddenIngredients)
        {
            if(ingredient.activeSelf == false)
            {
                return;
            }
        }
        GameObject dish = Resources.Load<GameObject>("Prefabs/" + dishName);
        Instantiate(dish, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(!recipeFound)
        {
            LookForInitialIngredients();
        }   
    }

    void OnTriggerEnter(Collider other)
    {
        if(LookForIngredient(other.gameObject.transform.parent.name))
        {
            foreach(GameObject ingredient in hiddenIngredients)
            {
                if(ingredient.name == "Turned " + other.gameObject.transform.parent.name && ingredient.activeSelf == false)
                {
                    ingredient.SetActive(true);
                    break;
                }
            }
            Destroy(other.gameObject.transform.parent.gameObject);
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

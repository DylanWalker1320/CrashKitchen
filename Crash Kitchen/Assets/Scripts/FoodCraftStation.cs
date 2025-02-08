using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodCraftStation : MonoBehaviour
{
    [SerializeField] List<Ingredient> presentIngredients = new List<Ingredient>();
    [SerializeField] RecipeDatabase recipes;
    [SerializeField] bool checkRecipes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recipes = FindAnyObjectByType<RecipeDatabase>();
        checkRecipes = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
    void FixedUpdate()
    {
        CheckForRecipe();
    }

    void CheckForRecipe()
    {
        if(presentIngredients.Count > 1 && checkRecipes)
        {
            checkRecipes = false;
            for(int i = 0; i < recipes.myRecipeList.recipes.Length; i++)
            {
                foreach (Ingredient name in presentIngredients)
                {
                    if(!recipes.myRecipeList.recipes[i].ingredients.Contains(name.name))
                    {
                        Debug.Log("UH OH");
                        break;
                    }
                    else
                    {
                        if(presentIngredients[presentIngredients.Count - 1].name == name.name && 
                            presentIngredients.Count == recipes.myRecipeList.recipes[i].ingredients.Count)
                        {
                            foreach(Ingredient ingredient in presentIngredients)
                            {
                                Destroy(ingredient.gameObject);
                            }
                            GameObject dish = Resources.Load<GameObject>("Prefabs/" + recipes.myRecipeList.recipes[i].name);
                            Debug.Log(dish);
                            if(dish != null)
                            {
                                Debug.Log("SHEESH");
                                Instantiate(dish, gameObject.transform.position, Quaternion.identity);
                                break;
                            }
                        }
                    }
                }
            }
            // iterate through present ingredients
            // if this ingredient is in this recipe, go next
            // else move to next recipe
        }
        else
        {
            checkRecipes = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Ingredient>() != null)
        {
            presentIngredients.Add(other.gameObject.GetComponent<Ingredient>());
            checkRecipes = true;
        }
        else
        {
            Debug.Log("FAKEEEE");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.GetType().IsSubclassOf(typeof(Ingredient)))
        {
            presentIngredients.Remove(other.gameObject.GetComponent<Ingredient>());
        }
    }
    
}

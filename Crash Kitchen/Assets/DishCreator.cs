using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using Unity.Netcode;

public class DishCreator : NetworkBehaviour
{
    public DishType dishType;
    public List<string> ingredients;
    public GameObject[] hiddenIngredients; 
    [SerializeField] RecipeDatabase recipes;
    [SerializeField] bool recipeFound;

    public enum DishType 
    {
        MegaGlizzy, 
        HealthyBurger, 
        DeluxeSteak
    }
    
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
        
        // If we're on the server, handle completion directly
        if (IsServer)
        {
            CompleteDishServerSide();
        }
        else
        {
            // Otherwise, request the server to complete it
            CompleteDishServerRpc();
        }
    }

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
        // Only proceed if this is a valid ingredient
        if(LookForIngredient(other.gameObject.transform.parent.name))
        {
            // Activate the corresponding hidden ingredient locally
            foreach(GameObject ingredient in hiddenIngredients)
            {
                if(ingredient.name == "Turned " + other.gameObject.transform.parent.name && ingredient.activeSelf == false)
                {
                    ingredient.SetActive(true);
                    break;
                }
            }
            
            // Get the name to remove from ingredients
            string ingredientName = other.gameObject.transform.parent.name;
            
            // Handle the object destruction
            GameObject objectToDestroy = other.gameObject.transform.parent.gameObject;
            
            // If it has a NetworkObject, we need to notify the server about the ingredient use
            if (IsServer)
            {
                // Server side - broadcast the ingredient removal to all clients
                RemoveIngredientClientRpc(ingredientName);
                
                // Destroy the object directly (no despawn)
                Destroy(objectToDestroy);
            }
            else
            {
                // Client side - tell the server we used this ingredient
                NotifyIngredientUsedServerRpc(ingredientName);
                
                // Locally destroy the object
                Destroy(objectToDestroy);
            }
            
            // Update our local ingredients list
            if (ingredients.Contains(ingredientName))
            {
                ingredients.Remove(ingredientName);
            }
            
            CheckForCompletion();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void NotifyIngredientUsedServerRpc(string ingredientName)
    {
        // Server received notification that a client used an ingredient
        if (ingredients.Contains(ingredientName))
        {
            ingredients.Remove(ingredientName);
        }
        
        // Broadcast to all clients to remove this ingredient from their lists
        RemoveIngredientClientRpc(ingredientName);
    }
    
    [ClientRpc]
    void RemoveIngredientClientRpc(string ingredientName)
    {
        // Remove from ingredients list on all clients
        if (ingredients.Contains(ingredientName))
        {
            ingredients.Remove(ingredientName);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void CompleteDishServerRpc()
    {
        CompleteDishServerSide();
    }
    
    // Server-side implementation of dish completion
    void CompleteDishServerSide()
    {
        GameObject dish = Resources.Load<GameObject>("Prefabs/" + dishType.ToString());
        GameObject newDish = Instantiate(dish, gameObject.transform.position, Quaternion.identity);
        
        // If the dish has a NetworkObject component, spawn it on the network
        NetworkObject netObj = newDish.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        
        GameManager.instance.currentOrderDone = true;
        
        // If this GameObject has a NetworkObject, despawn it properly
        NetworkObject thisNetObj = GetComponent<NetworkObject>();
        if (thisNetObj != null && thisNetObj.IsSpawned)
        {
            thisNetObj.Despawn(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
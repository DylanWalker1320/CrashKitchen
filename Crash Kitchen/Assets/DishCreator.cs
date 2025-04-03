using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using Unity.Netcode;

public class DishCreator : NetworkBehaviour
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

            // Get the name to remove from ingredients
            string ingredientName = other.gameObject.transform.parent.name;
            
            // Handle the object destruction via server RPC
            NetworkObject netObj = other.gameObject.transform.parent.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                DestroyIngredientServerRpc(netObj.NetworkObjectId, ingredientName);
            }
            else
            {
                // For non-networked objects
                ingredients.Remove(ingredientName);
                Destroy(other.gameObject.transform.parent.gameObject);
            }
            
            CheckForCompletion();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyIngredientServerRpc(ulong networkObjectId, string ingredientName)
    {
        // Find the network object with this ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            // Tell clients to remove from ingredients list
            RemoveIngredientClientRpc(ingredientName);
            
            // Make sure the object won't have parenting issues when despawned
            if (netObj.transform.parent != null)
            {
                // Unparent the object before despawning to avoid reparenting errors
                netObj.transform.SetParent(null);
            }
            
            // Now despawn the object on the server (proper way to destroy networked objects)
            netObj.Despawn(true);
        }
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

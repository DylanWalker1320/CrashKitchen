using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public TextAsset textJSON;
    [System.Serializable]
    public class Recipe
    {
        public string name;
        public List<string> ingredients;
    }
    [System.Serializable]
    public class RecipeList
    {
        public Recipe[] recipes;
    }
    public RecipeList myRecipeList = new RecipeList();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRecipeList = JsonUtility.FromJson<RecipeList>(textJSON.text);
    }

    // Update is called once per frame
    void Update()
    {

    }

}

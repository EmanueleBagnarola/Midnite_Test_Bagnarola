using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientsContainer", menuName = "Data/IngredientsContainer")]
public class IngredientsContainer : ScriptableObject
{
    public IngredientData[] IngredientDataArray = null;

    public Ingredient GetIngredient(IngredientID ingredientID)
    {
        for (int i = 0; i < IngredientDataArray.Length; i++)
        {
            Ingredient ingredient = IngredientDataArray[i].IngredientPrefab;
            if (ingredient.GetIngredientID == ingredientID)
            {
                return ingredient;
            }
        }

        return null;
    }
}

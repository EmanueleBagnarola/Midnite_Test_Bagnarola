using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridHandler : MonoBehaviour
{
    public static GridHandler Instance = null;

    /// <summary>
    /// The single ingredients on the grid (the ones that are not into a stack)
    /// </summary>
    [SerializeField]
    private List<Ingredient> _ingredientsOnGrid = new List<Ingredient>();

    /// <summary>
    /// The ingredients stack on the grid (at least two ingredients on top of each other)
    /// </summary>
    [SerializeField]
    private List<IngredientStack> _stacksOnGrid = new List<IngredientStack>();

    private void Start()
    {
        EventsHandler.Instance.OnIngredientMovement?.AddListener(OnIngredientMovement);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Ingredient GetIngredientAtDestination(Vector2 coords)
    {
        if (_ingredientsOnGrid.Count <= 0)
            return null;

        for (int i = 0; i < _ingredientsOnGrid.Count; i++)
        {
            Ingredient ingredient = _ingredientsOnGrid[i];
            if (ingredient.GetCoords == coords)
                return ingredient;
        }

        return null;
    }

    public IngredientStack GetStackAtDestination(Vector2 coords)
    {
        if (_stacksOnGrid.Count <= 0)
            return null;

        for (int i = 0; i < _stacksOnGrid.Count; i++)
        {
            IngredientStack stack = _stacksOnGrid[i];
            if (stack.Coords == coords)
                return stack;
        }

        return null;
    }

    public void RemoveIngredientFromGrid(Ingredient ingredient)
    {
        _ingredientsOnGrid.Remove(ingredient);
    }

    public bool IsDestinationOccupied(Vector2 coords)
    {
        if (_ingredientsOnGrid.Count > 0)
        {
            for (int i = 0; i < _ingredientsOnGrid.Count; i++)
            {
                Ingredient ingredient = _ingredientsOnGrid[i];
                if (ingredient.GetCoords == coords)
                    return true;
            }
        }

        if (_stacksOnGrid.Count > 0)
        {
            for (int i = 0; i < _stacksOnGrid.Count; i++)
            {
                IngredientStack stack = _stacksOnGrid[i];
                if (stack.Coords == coords)
                    return true;
            }
        }

        return false;
    }

    public bool IsDestinationOccupiedByAStack(Vector2 coords)
    {
        if (_stacksOnGrid.Count <= 0)
            return false;

        for (int i = 0; i < _stacksOnGrid.Count; i++)
        {
            IngredientStack ingredientStack = _stacksOnGrid[i];
            if (ingredientStack.Coords == coords)
                return true;
        }

        return false;
    }

    private void OnIngredientMovement(Ingredient ingredient, Vector2 coords)
    {
        // Single Ingredient move
        if (_ingredientsOnGrid.Contains(ingredient))
        {
            Debug.LogWarning("SINGLE INGREDIENTS MOVE");

            // Single Ingredient on a stack
            if (IsDestinationOccupiedByAStack(coords))
            {
                Debug.LogWarning("SINGLE INGREDIENT ON A STACK");

                // Get the destination stack
                IngredientStack destinationStack = GetStackAtDestination(coords);

                // Add the moving ingredient to the stack list
                destinationStack.IngredientsInStack.Add(ingredient);

                // Move the moving ingredient on the top of the stack
                ingredient.transform.position = new Vector3(coords.x, destinationStack.IngredientsInStack.Count * 0.1f, coords.y);

                // Remove the ingredient from the ingredient list of the grid
                RemoveIngredientFromGrid(ingredient);
                return;
            }

            // Create a new stack
            IngredientStack stack = new IngredientStack(coords);

            // Add the ingredient where the moved ingredient went on to
            Ingredient ingredientOnGrid = GetIngredientAtDestination(coords);
            stack.AddIngredient(ingredientOnGrid);

            // Add the moved ingredient
            stack.AddIngredient(ingredient);

            // Move the ingredient
            ingredient.transform.position = new Vector3(coords.x, GetIngredientAtDestination(coords).transform.position.y + 0.1f, coords.y);

            // Remove both ingredient from the single ingredients list on the grid
            RemoveIngredientFromGrid(ingredientOnGrid);
            RemoveIngredientFromGrid(ingredient);

            // Add the new stack in the IngredientStack list
            _stacksOnGrid.Add(stack);
            return;
        }

        // Stack move
        for (int i = 0; i < _stacksOnGrid.Count; i++)
        {
            IngredientStack stack = _stacksOnGrid[i];
            if (stack.IngredientsInStack.Contains(ingredient))
            {
                Debug.LogWarning("THE INGREDIENT [" + ingredient.name + "] IS PART OF A STACK");

                // Stack on single ingredient move
                if (!IsDestinationOccupiedByAStack(coords))
                {
                    Debug.LogWarning("MOVING THE STACK ON TOP OF A SINGLE INGREDIENT");

                    // If not

                    // Create a new stack list
                    List<Ingredient> newStack = new List<Ingredient>();
                    newStack.Add(GetIngredientAtDestination(coords));

                    // Move the ingredients on the single ingredient from last to first
                    for (int x = stack.IngredientsInStack.Count - 1; x >= 0; x--)
                    {
                        Ingredient ingredientInStack = stack.IngredientsInStack[x];
                        ingredientInStack.transform.position = new Vector3(coords.x, 0.1f * (stack.IngredientsInStack.Count - x), coords.y);

                        // Add each ingredient in the new stack with new list order
                        newStack.Add(ingredientInStack);
                    }

                    // Modify the current stack list
                    stack.SetStack(newStack);

                    // Remove the ingredient from the grid list
                    RemoveIngredientFromGrid(GetIngredientAtDestination(coords));

                    // Update the stack position
                    stack.Coords = stack.IngredientsInStack[0].GetCoords;
                }

                // Stack on a stack move
                else
                {
                    IngredientStack movingStack = GetStackAtDestination(ingredient.GetCoords);
                    IngredientStack destinationStack = GetStackAtDestination(coords);

                    // Add every ingredient of the moving stack in the destination stack ingredients list, from last to first
                    for (int x = movingStack.IngredientsInStack.Count - 1; x >= 0; x--)
                    {
                        Ingredient topIngredient = movingStack.IngredientsInStack[x];

                        destinationStack.AddIngredient(topIngredient);

                        // Move the top ingredient of the moving stack
                        topIngredient.transform.position = new Vector3(coords.x, destinationStack.IngredientsInStack.Count * 0.1f, coords.y);
                    }

                    // Remove the moving stack from the current stacks list
                    _stacksOnGrid.Remove(movingStack);
                }
            }
        }

        EventsHandler.Instance.OnMoveSuccess?.Invoke();
    }

    public bool CheckWinCondition()
    {
        if (_stacksOnGrid.Count <= 0 || _stacksOnGrid.Count > 1 || _ingredientsOnGrid.Count > 0)
            return false;

        List<Ingredient> ingredientsInLastStack = _stacksOnGrid[0].IngredientsInStack;
        IngredientID firstIngredientID = ingredientsInLastStack[0].GetIngredientID;
        IngredientID lastIngredientID = ingredientsInLastStack[ingredientsInLastStack.Count - 1].GetIngredientID;
        if (firstIngredientID == IngredientID.bread && lastIngredientID == IngredientID.bread)
            return true;

        return false;
    }

    public bool IsInGridRange(Vector2 positionToCheck)
    {
        return positionToCheck.x >= 0 && positionToCheck.x <= 3 && positionToCheck.y >= -3 && positionToCheck.y <= 0;
    }
}

[Serializable]
public class IngredientStack
{
    public List<Ingredient> IngredientsInStack = new List<Ingredient>();
    public Vector2 Coords = Vector2.zero;

    public IngredientStack(Vector2 coords)
    {
        Coords = coords;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        IngredientsInStack.Add(ingredient);
    }

    public void SetStack(List<Ingredient> newStackList)
    {
        IngredientsInStack.Clear();
        IngredientsInStack = newStackList;
    }

    public void MoveStack(Vector2 newCoords)
    {
        Coords = newCoords;
    }
}

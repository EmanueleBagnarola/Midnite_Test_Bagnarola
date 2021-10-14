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
    private List<Tile> _tilesOnGrid = new List<Tile>();

    /// <summary>
    /// The ingredients stack on the grid (at least two ingredients on top of each other)
    /// </summary>
    [SerializeField]
    private List<IngredientStack> _stacksOnGrid = new List<IngredientStack>();

    [SerializeField]
    private List<Tile> _startingGrid = new List<Tile>();

    private void Start()
    {
        EventsHandler.Instance.OnTileMovement?.AddListener(OnTileMovement);
        EventsHandler.Instance.OnLevelGenerationEnded?.AddListener(OnLevelGenerationEnded);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Tile GetTileAtDestination(Vector2 coords)
    {
        if (_tilesOnGrid.Count <= 0)
            return null;

        for (int i = 0; i < _tilesOnGrid.Count; i++)
        {
            Tile tile = _tilesOnGrid[i];
            if (tile.GetCoords == coords)
                return tile;
        }

        return null;
    }

    public void AddTileToGrid(Tile tile)
    {
        _tilesOnGrid.Add(tile);
    }

    public void RemoveTileFromGrid(Tile tile)
    {
        _tilesOnGrid.Remove(tile);
    }

    public bool IsDestinationOccupied(Vector2 coords)
    {
        if (_tilesOnGrid.Count > 0)
        {
            for (int i = 0; i < _tilesOnGrid.Count; i++)
            {
                Tile tile = _tilesOnGrid[i];
                if (tile.GetCoords == coords)
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

    #region Food Handlers

    public Ingredient GetRandomIngredientOnGrid()
    {
        int randomIndex = UnityEngine.Random.Range(0, _tilesOnGrid.Count - 1);
        return (Ingredient)_tilesOnGrid[randomIndex];
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

    private void OnTileMovement(Tile tile, Vector2 coords)
    {
        if (tile is Ingredient)
        {
            // Single Ingredient move
            if (_tilesOnGrid.Contains(tile))
            {
                Debug.LogWarning("SINGLE INGREDIENTS MOVE");

                // Single Ingredient on a stack
                if (IsDestinationOccupiedByAStack(coords))
                {
                    Debug.LogWarning("SINGLE INGREDIENT ON A STACK");

                    // Get the destination stack
                    IngredientStack destinationStack = GetStackAtDestination(coords);

                    // Add the moving ingredient to the stack list
                    destinationStack.IngredientsInStack.Add((Ingredient)tile);

                    // Move the moving ingredient on the top of the stack
                    tile.transform.position = new Vector3(coords.x, destinationStack.IngredientsInStack.Count * 0.1f, coords.y);

                    // Remove the ingredient from the ingredient list of the grid
                    RemoveTileFromGrid(tile);
                    return;
                }

                // Create a new stack
                IngredientStack stack = new IngredientStack(coords);

                // Add the ingredient where the moved ingredient went on to
                Ingredient ingredientOnGrid =  (Ingredient)GetTileAtDestination(coords);
                stack.AddIngredient(ingredientOnGrid);

                // Add the moved ingredient
                stack.AddIngredient((Ingredient)tile);

                // Move the ingredient
                tile.transform.position = new Vector3(coords.x, GetTileAtDestination(coords).transform.position.y + 0.1f, coords.y);

                // Remove both ingredient from the single ingredients list on the grid
                RemoveTileFromGrid(ingredientOnGrid);
                RemoveTileFromGrid(tile);

                // Add the new stack in the IngredientStack list
                _stacksOnGrid.Add(stack);
                return;
            }

            // Stack move
            for (int i = 0; i < _stacksOnGrid.Count; i++)
            {
                IngredientStack stack = _stacksOnGrid[i];
                if (stack.IngredientsInStack.Contains((Ingredient)tile))
                {
                    Debug.LogWarning("THE INGREDIENT [" + tile.name + "] IS PART OF A STACK");

                    // Stack on single ingredient move
                    if (!IsDestinationOccupiedByAStack(coords))
                    {
                        Debug.LogWarning("MOVING THE STACK ON TOP OF A SINGLE INGREDIENT");

                        // If not

                        // Create a new stack list
                        List<Ingredient> newStack = new List<Ingredient>();
                        newStack.Add((Ingredient)GetTileAtDestination(coords));

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
                        RemoveTileFromGrid(GetTileAtDestination(coords));

                        // Update the stack position
                        stack.Coords = stack.IngredientsInStack[0].GetCoords;
                    }

                    // Stack on a stack move
                    else
                    {
                        IngredientStack movingStack = GetStackAtDestination(tile.GetCoords);
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
        }

       

        EventsHandler.Instance.OnMoveSuccess?.Invoke();
    }

    private void OnLevelGenerationEnded()
    {
        for (int i = 0; i < _tilesOnGrid.Count; i++)
        {
            Tile tile = _tilesOnGrid[i];
            _startingGrid.Add(tile);
        }
    }

    public void ResetTilePositions()
    {
        _tilesOnGrid.Clear();

        for (int i = 0; i < _startingGrid.Count; i++)
        {
            Tile tile = _startingGrid[i];
            _tilesOnGrid.Add(tile);
        }

        for (int i = 0; i < _tilesOnGrid.Count; i++)
        {
            Tile tile = _tilesOnGrid[i];
            tile.transform.position = tile.GetStartingPosition;
        }

        _stacksOnGrid.Clear();
    }

    public bool CheckWinCondition()
    {
        if (_stacksOnGrid.Count <= 0 || _stacksOnGrid.Count > 1 || _tilesOnGrid.Count > 0)
            return false;

        List<Ingredient> ingredientsInLastStack = _stacksOnGrid[0].IngredientsInStack;
        IngredientID firstIngredientID = ingredientsInLastStack[0].GetIngredientID;
        IngredientID lastIngredientID = ingredientsInLastStack[ingredientsInLastStack.Count - 1].GetIngredientID;
        if (firstIngredientID == IngredientID.bread && lastIngredientID == IngredientID.bread)
            return true;

        return false;
    }
    #endregion

    #region Number Handlers
    #endregion

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
}
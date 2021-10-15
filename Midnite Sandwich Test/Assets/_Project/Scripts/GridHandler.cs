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
    private List<Tile> _tilesOnGrid = new List<Tile>();

    /// <summary>
    /// The ingredients stack on the grid (at least two ingredients on top of each other)
    /// </summary>
    [SerializeField]
    private List<TilesStack> _tilesStackOnGrid = new List<TilesStack>();

    private List<Tile> _startingGrid = new List<Tile>();

    private float _tilesHeight = 0.0f;

    private void Start()
    {
        _tilesHeight = GameHandler.Instance.GetGameSettings.TilesHeight;

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

        if (_tilesStackOnGrid.Count > 0)
        {
            for (int i = 0; i < _tilesStackOnGrid.Count; i++)
            {
                TilesStack stack = _tilesStackOnGrid[i];
                if (stack.Coords == coords)
                    return true;
            }
        }
        return false;
    }

    public TilesStack GetTileStack(Tile tile)
    {
        if (_tilesStackOnGrid.Count <= 0)
        {
            Debug.LogWarning("There are no stacks onto the grid");
            return null;
        }

        for (int i = 0; i < _tilesStackOnGrid.Count; i++)
        {
            TilesStack tilesStack = _tilesStackOnGrid[i];
            if (tilesStack.TilesInStack.Contains(tile))
                return tilesStack;
        }

        Debug.LogWarning("There is not tiles stack containing the tile");
        return null;
    }

    #region Food Handlers
    private void HandleIngredientMovement(Tile tile, Vector2 coords)
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
                TilesStack destinationStack = GetStackAtDestination(coords);

                // Add the moving ingredient to the stack list
                destinationStack.TilesInStack.Add((Ingredient)tile);

                // Move the moving ingredient on the top of the stack
                Ingredient movingIngredient = tile as Ingredient;
                movingIngredient.MoveTile(new Vector3(coords.x, (destinationStack.TilesInStack.Count * _tilesHeight) - (_tilesHeight / 2), coords.y));

                // Remove the ingredient from the ingredient list of the grid
                RemoveTileFromGrid(tile);
            }
            else
            {
                Debug.LogWarning("SINGLE INGEDIENT ON A SINGLE INGREDIENT");

                // Create a new stack
                TilesStack stack = new TilesStack(coords);

                // Add the ingredient where the moved ingredient went on to
                Ingredient ingredientOnGrid = (Ingredient)GetTileAtDestination(coords);
                stack.AddTile(ingredientOnGrid);

                // Add the moved ingredient
                stack.AddTile((Ingredient)tile);

                // Move the ingredient
                Ingredient ingredient = tile as Ingredient;
                ingredient.MoveTile(new Vector3(coords.x, GetTileAtDestination(coords).transform.position.y + _tilesHeight, coords.y));

                // Remove both ingredient from the single ingredients list on the grid
                RemoveTileFromGrid(ingredientOnGrid);
                RemoveTileFromGrid(tile);

                // Add the new stack in the IngredientStack list
                _tilesStackOnGrid.Add(stack);
            }
        }
        else
        {
            // Stack move
            for (int i = 0; i < _tilesStackOnGrid.Count; i++)
            {
                TilesStack stack = _tilesStackOnGrid[i];
                if (stack.TilesInStack.Contains((Ingredient)tile))
                {
                    Debug.LogWarning("THE INGREDIENT [" + tile.name + "] IS PART OF A STACK");

                    // Stack on single ingredient move
                    if (!IsDestinationOccupiedByAStack(coords))
                    {
                        Debug.LogWarning("MOVING THE STACK ON TOP OF A SINGLE INGREDIENT");

                        // Create a new stack list
                        List<Tile> newStack = new List<Tile>();
                        newStack.Add(GetTileAtDestination(coords));

                        // Move the ingredients on the single ingredient from last to first
                        for (int x = stack.TilesInStack.Count - 1; x >= 0; x--)
                        {
                            Ingredient ingredientInStack = (Ingredient)stack.TilesInStack[x];
                
                            ingredientInStack.MoveTile(new Vector3(coords.x, (_tilesHeight * (stack.TilesInStack.Count - x)) + (_tilesHeight / 2), coords.y));

                            // Add each ingredient in the new stack with new list order
                            newStack.Add(ingredientInStack);
                        }

                        // Modify the current stack list
                        stack.SetStack(newStack);

                        // Remove the ingredient from the grid list
                        RemoveTileFromGrid(GetTileAtDestination(coords));

                        // Update the stack position
                        stack.Coords = stack.TilesInStack[0].GetCoords;
                    }

                    // Stack on a stack move
                    else
                    {
                        TilesStack movingStack = GetStackAtDestination(tile.GetCoords);
                        TilesStack destinationStack = GetStackAtDestination(coords);

                        // Add every ingredient of the moving stack in the destination stack ingredients list, from last to first
                        for (int x = movingStack.TilesInStack.Count - 1; x >= 0; x--)
                        {
                            Ingredient topIngredient = (Ingredient)movingStack.TilesInStack[x];

                            destinationStack.AddTile(topIngredient);

                            // Move the top ingredient of the moving stack
                            topIngredient.MoveTile(new Vector3(coords.x, ((destinationStack.TilesInStack.Count - 1) * _tilesHeight) + (_tilesHeight / 2), coords.y));
                        }

                        // Remove the moving stack from the current stacks list
                        _tilesStackOnGrid.Remove(movingStack);
                    }
                }
            }
        }

        EventsHandler.Instance.OnMoveSuccess?.Invoke();
    }

    public Ingredient GetRandomIngredientOnGrid()
    {
        int randomIndex = UnityEngine.Random.Range(0, _tilesOnGrid.Count - 1);
        return (Ingredient)_tilesOnGrid[randomIndex];
    }

    public TilesStack GetStackAtDestination(Vector2 coords)
    {
        if (_tilesStackOnGrid.Count <= 0)
            return null;

        for (int i = 0; i < _tilesStackOnGrid.Count; i++)
        {
            TilesStack stack = _tilesStackOnGrid[i];
            if (stack.Coords == coords)
                return stack;
        }

        return null;
    }

    public bool IsDestinationOccupiedByAStack(Vector2 coords)
    {
        if (_tilesStackOnGrid.Count <= 0)
            return false;

        for (int i = 0; i < _tilesStackOnGrid.Count; i++)
        {
            TilesStack ingredientStack = _tilesStackOnGrid[i];
            if (ingredientStack.Coords == coords)
                return true;
        }

        return false;
    }
    #endregion

    #region Number Handlers
    private void HandleNumberMovement(Tile tile, Vector2 coords)
    {
        if(GameHandler.Instance.GetGameMode == GameMode.numbers_simple)
        {
            #region 2048 Type 1
            Number movedNumber = (Number)tile;
            Number numberAtDestination = (Number)GetTileAtDestination(coords);

            if (movedNumber.GetNumberID != numberAtDestination.GetNumberID)
            {
                Debug.LogWarning("CAN'T MOVE A NUMBER ON A DIFFERENT NUMBER");
                return;
            }

            numberAtDestination.SetNumberID(numberAtDestination.GetNumberID + movedNumber.GetNumberID);
            movedNumber.MoveNumber(new Vector3(coords.x, 0, coords.y));
            //movedNumber.gameObject.SetActive(false);

            EventsHandler.Instance.OnMoveSuccess?.Invoke();
            #endregion
        }

        else if(GameHandler.Instance.GetGameMode == GameMode.numbers_stack)
        {
            #region 2048 Type 2
            // Single number move
            if (_tilesOnGrid.Contains(tile))
            {
                Debug.LogWarning("SINGLE NUMBER MOVE");

                // Single number on a stack
                if (IsDestinationOccupiedByAStack(coords))
                {
                    Debug.LogWarning("SINGLE NUMBER ON A STACK");

                    // Get the destination stack
                    TilesStack destinationStack = GetStackAtDestination(coords);

                    // Move the moving number on the top of the stack
                    Number movingNumber = tile as Number;
                    movingNumber.MoveTile(new Vector3(coords.x, destinationStack.TilesInStack.Count * _tilesHeight, coords.y));

                    // Remove the number from the ingredient list of the grid
                    RemoveTileFromGrid(tile);

                    Number topNumber = (Number)destinationStack.TilesInStack[destinationStack.TilesInStack.Count - 1];
                    if(!CheckNumbersMerge(movingNumber, topNumber))
                    {
                        // Add the moving ingredient to the stack list
                        destinationStack.TilesInStack.Add((Number)tile);
                    }
                }
                else
                {
                    // Create a new stack
                    TilesStack stack = new TilesStack(coords);

                    // Add the ingredient where the moved ingredient went on to
                    Number numberOnGrid = (Number)GetTileAtDestination(coords);
                    stack.AddTile(numberOnGrid);

                    // Add the moved ingredient
                    stack.AddTile((Number)tile);

                    // Move the ingredient
                    Number number = tile as Number;
                    number.MoveTile(new Vector3(coords.x, GetTileAtDestination(coords).transform.position.y + _tilesHeight, coords.y));

                    // Remove both ingredient from the single ingredients list on the grid
                    RemoveTileFromGrid(numberOnGrid);
                    RemoveTileFromGrid(tile);

                    // Add the new stack in the IngredientStack list
                    _tilesStackOnGrid.Add(stack);

                    CheckNumbersMerge(number, numberOnGrid);
                }
            }
            else
            {
                // Stack move
                for (int i = 0; i < _tilesStackOnGrid.Count; i++)
                {
                    TilesStack stack = _tilesStackOnGrid[i];
                    if (stack.TilesInStack.Contains((Number)tile))
                    {
                        // Stack on single number move
                        if (!IsDestinationOccupiedByAStack(coords))
                        {
                            Debug.LogWarning("MOVING THE STACK ON TOP OF A SINGLE NUMBER");

                            // Check if the top number of the stack can be merged
                            for (int x = GetStackAtDestination(tile.GetCoords).TilesInStack.Count - 1; x >= 0; x--)
                            {
                                Number topNumber = GetStackAtDestination(tile.GetCoords).TilesInStack[x] as Number;
                                if (!CheckNumbersMerge(topNumber, (Number)GetTileAtDestination(coords)))
                                    break;
                            }
                            
                            // Create a new stack list
                            List<Tile> newStack = new List<Tile>();
                            newStack.Add(GetTileAtDestination(coords));

                            // Move the ingredients on the single ingredient from last to first
                            for (int x = stack.TilesInStack.Count - 1; x >= 0; x--)
                            {
                                Number numberInStack = (Number)stack.TilesInStack[x];
                                numberInStack.MoveTile(new Vector3(coords.x, (_tilesHeight * (stack.TilesInStack.Count - x)) + (_tilesHeight / 2), coords.y));

                                newStack.Add(numberInStack);
                            }

                            // Modify the current stack list
                            stack.SetStack(newStack);

                            // Remove the ingredient from the grid list
                            RemoveTileFromGrid(GetTileAtDestination(coords));

                            // Update the stack position
                            stack.Coords = stack.TilesInStack[0].GetCoords;
                        }

                        // Stack on a stack move
                        else
                        {
                            Debug.LogWarning("STACK ON STACK MOVE");

                            TilesStack movingStack = GetStackAtDestination(tile.GetCoords);
                            TilesStack destinationStack = GetStackAtDestination(coords);

                            // Add every ingredient of the moving stack in the destination stack ingredients list, from last to first
                            for (int x = movingStack.TilesInStack.Count - 1; x >= 0; x--)
                            {
                                Number topMovingNumber = (Number)movingStack.TilesInStack[x];

                                for (int y = destinationStack.TilesInStack.Count - 1; y >= 0; y--)
                                {
                                    Number topDestinationNumber = (Number)destinationStack.TilesInStack[y];
                                    if (!CheckNumbersMerge(topMovingNumber, topDestinationNumber))
                                    {
                                        break;
                                    }
                                }

                                destinationStack.AddTile(topMovingNumber);

                                // Move the top ingredient of the moving stack
                                topMovingNumber.MoveTile(new Vector3(coords.x, ((destinationStack.TilesInStack.Count - 1) * _tilesHeight) + +(_tilesHeight / 2), coords.y));
                            }

                            // Remove the moving stack from the current stacks list
                            _tilesStackOnGrid.Remove(movingStack);
                        }
                    }
                }
            }
            #endregion
        }

        EventsHandler.Instance.OnMoveSuccess?.Invoke();
    }

    private bool CheckNumbersMerge(Number movedNumber, Number destinationNumber)
    {
        if (movedNumber.GetNumberID != destinationNumber.GetNumberID)
            return false;

        Debug.LogWarning("NUMBERS CAN STACK"); 
        StartCoroutine(HandleNumbersMerge(movedNumber, destinationNumber));

        EventsHandler.Instance.OnMoveSuccess?.Invoke();
        return true;
    }

    private IEnumerator HandleNumbersMerge(Number movedNumber, Number destinationNumber)
    {
        yield return new WaitForSeconds(0.3f);

        movedNumber.MoveNumber(destinationNumber.transform.position);
        destinationNumber.SetNumberID(destinationNumber.GetNumberID + movedNumber.GetNumberID);
        
        // Check if the moved number created a stack (single number on single number situation)
        TilesStack tileStack = GetTileStack(destinationNumber);
        if(tileStack != null)
        {
            // then remove the moved number from stack
            tileStack.TilesInStack.Remove(movedNumber);

            // if there is one remainin number in the stack, reinsert it in the grid list, as one number can't be in a stack alone
            if (tileStack.TilesInStack.Count == 1)
            {
                _tilesOnGrid.Add(destinationNumber);
                tileStack.TilesInStack.Remove(movedNumber);
                _tilesStackOnGrid.Remove(tileStack);
            }
            else
            {
                // if the remaining stack has more numbers remaining, check if numbers of each side of the movement can merge together
                for (int i = tileStack.TilesInStack.Count - 1; i > 0; i--)
                {
                    Number topNumberInStack = tileStack.TilesInStack[i] as Number;
                    Number previousNumberInStack = tileStack.TilesInStack[i - 1] as Number;

                    yield return new WaitForSeconds(0.2f);

                    CheckNumbersMerge(topNumberInStack, previousNumberInStack);
                }
            }
        }        
    }
    #endregion

    private void OnTileMovement(Tile tile, Vector2 coords)
    {
        if (tile is Ingredient)
        {
            HandleIngredientMovement(tile, coords);
        }
        else if(tile is Number)
        {
            HandleNumberMovement(tile, coords);
        }
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
            tile.ResetToStartingPosition();
        }

        _tilesStackOnGrid.Clear();
    }

    public bool CheckWinCondition()
    {
        GameMode currentGameMode = GameHandler.Instance.GetGameMode;

        if(currentGameMode == GameMode.food_template || currentGameMode == GameMode.food_random)
        {
            if (_tilesStackOnGrid.Count <= 0 || _tilesStackOnGrid.Count > 1 || _tilesOnGrid.Count > 0)
                return false;

            List<Tile> ingredientsInLastStack = _tilesStackOnGrid[0].TilesInStack;
            Ingredient firstIngredient = ingredientsInLastStack[0] as Ingredient;
            IngredientID firstIngredientID = firstIngredient.GetIngredientID;
            Ingredient lastIngredient = ingredientsInLastStack[ingredientsInLastStack.Count - 1] as Ingredient;
            IngredientID lastIngredientID = lastIngredient.GetIngredientID;
            if (firstIngredientID == IngredientID.bread && lastIngredientID == IngredientID.bread)
                return true;
        }
        else if(currentGameMode == GameMode.numbers_simple)
        {
            int currentUnmovedTiles = 0;

            for (int i = 0; i < _tilesOnGrid.Count; i++)
            {
                Tile tile = _tilesOnGrid[i];
                Number number = (Number)tile;
                if (!number.IsMoved)
                    currentUnmovedTiles++;
            }

            if (currentUnmovedTiles == 1)
                return true;
        }
        else if(currentGameMode == GameMode.numbers_stack)
        {
            Tile[] tilesInScene = FindObjectsOfType<Tile>();

            int activeTiles = 0;

            for (int i = 0; i < tilesInScene.Length; i++)
            {
                Tile tile = tilesInScene[i];
                Number number = (Number)tile;
                if (number.gameObject.activeSelf)
                    activeTiles++;
            }

            if (activeTiles == 1)
                return true;
        }

        return false;
    }

    public bool IsInGridRange(Vector2 positionToCheck)
    {
        int gridSize = GameHandler.Instance.GetGameSettings.GridSize;
        return positionToCheck.x >= 0 && positionToCheck.x <= gridSize - 1 && positionToCheck.y >= -(gridSize-1) && positionToCheck.y <= 0;
    }
}

[Serializable]
public class TilesStack
{
    public List<Tile> TilesInStack = new List<Tile>();
    public Vector2 Coords = Vector2.zero;

    public TilesStack(Vector2 coords)
    {
        Coords = coords;
    }

    public void AddTile(Tile tile)
    {
        TilesInStack.Add(tile);
    }

    public void SetStack(List<Tile> newStackList)
    {
        TilesInStack.Clear();
        TilesInStack = newStackList;
    }
}

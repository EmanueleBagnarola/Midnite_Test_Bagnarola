using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Food Level Settings")]
    [SerializeField]
    private IngredientsContainer _ingredientsContainer = null;
    [SerializeField]
    private LevelTemplate _foodLevelTemplate = null;
    /// <summary>
    /// Extra ingredients to spawn after bread
    /// </summary>
    [SerializeField]
    private int _additionalPiecesToGenerate = 3;


    [Header("Numbers Level Settings")]
    [SerializeField]
    private NumberData _numberData = null;
    [SerializeField]
    private LevelTemplate _numbersLevelTemplate = null;

    /// <summary>
    /// How many rows and columns the grid must have.
    /// The first coordinate it's always (0,0)
    /// </summary>
    [SerializeField]
    private int _gridSize = 4;

    /// <summary>
    /// All the coords generate at Start
    /// </summary>
    private List<Vector2> _coordinates = new List<Vector2>();

    /// <summary>
    /// All the coords occupied during level generation
    /// </summary>
    private List<Vector2> _usedCoordinates = new List<Vector2>();

    private Vector2 _nearPositionNotFound = new Vector2(1000, 1000);

    private void Start()
    {
        GenerateCoordinates();
    }

    private void GenerateCoordinates()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y > -_gridSize; y--)
            {
                Vector2 coord = new Vector2(x, y);
                _coordinates.Add(coord);
            }
        }

        switch (GameHandler.Instance.GetGameMode)
        {
            case GameMode.food_random:
                StartCoroutine(GenerateRandomLevel());
                break;
            case GameMode.food_template:
                StartCoroutine(FoodLevelGenerationFromTemplate());
                break;
            case GameMode.numbers:
                StartCoroutine(NumberGenerationLevelFromTemplate());
                break;
        }
    }

    #region Food Generation From Template
    private IEnumerator FoodLevelGenerationFromTemplate()
    {
        yield return null;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                InstantiateIngredientFromTemplate(_foodLevelTemplate.data.GetRowsData(x)[y], x, y);
            }
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }

    private void InstantiateIngredientFromTemplate(string ingrdientCode, int xTemplatePosition, int yTemplatePosition)
    {
        // Code R: random
        // Code 0: bread
        // Code 1: bacon
        // Code 2: cheese
        // Code 3: egg
        // Code 4: ham
        // Code 5: salad
        // Code 6: onion
        // Code 7: salami
        // Code 8: tomato

        if (ingrdientCode == "R" || ingrdientCode == "r")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(GetRandomIngredientInContainer(), new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }

        if (ingrdientCode == "0")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "1")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bacon).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "2")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.cheese).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "3")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.egg).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "4")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.ham).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "5")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salad).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "6")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.onion).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "7")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salami).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "8")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.tomato).gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
        }
    } 
    #endregion

    #region NumberGenerationFromTemplate
    private IEnumerator NumberGenerationLevelFromTemplate()
    {
        yield return null;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                InstantiateNumbersFromTemplate(_numbersLevelTemplate.data.GetRowsData(x)[y], x, y);
            }
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }

    private void InstantiateNumbersFromTemplate(string numberID, int xTemplatePosition, int yTemplatePosition)
    {
        if (numberID == string.Empty)
            return;

        Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
        GameObject number = Instantiate(_numberData.NumberPrefab.gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
        Number numberRef = number.GetComponent<Number>();
        numberRef.SetNumberID(numberID);
        GridHandler.Instance.AddTileToGrid(number.GetComponent<Number>());
    }
    #endregion

    #region Random Food Generation
    private IEnumerator GenerateRandomLevel()
    {
        // Spawn the first piece of bread
        Vector2 firstPosition = GetRandomFreePosition();
        GameObject firstBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(firstPosition.x, 0f, firstPosition.y), Quaternion.identity);
        // Add the first bread to the Grid Ingredients list
        GridHandler.Instance.AddTileToGrid(firstBread.GetComponent<Ingredient>());

        // Spawn the second piece of bread next to it, it any available position
        Vector2 nearPosition = GetRandomNearPosition(new Vector2(firstBread.transform.position.x, firstBread.transform.position.z));
        GameObject secondBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(nearPosition.x, 0f, nearPosition.y), Quaternion.identity);
        // Add the second bread to the Grid Ingredients list
        GridHandler.Instance.AddTileToGrid(secondBread.GetComponent<Ingredient>());


        // Spawn each ingredient near to an existing one
        for (int i = 0; i <= _additionalPiecesToGenerate; i++)
        {
            yield return new WaitForSeconds(0.2f);

            // Init the random near position as a non valid position to let at least one control
            Vector2 randomNearPosition = _nearPositionNotFound;

            while (randomNearPosition == _nearPositionNotFound)
            {
                Ingredient randomGridIngredient = GridHandler.Instance.GetRandomIngredientOnGrid();
                randomNearPosition = GetRandomNearPosition(randomGridIngredient.GetCoords);
            }

            GameObject ingredient = GetRandomIngredientInContainer();
            GameObject newIngredient = Instantiate(ingredient, new Vector3(randomNearPosition.x, 0f, randomNearPosition.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(newIngredient.GetComponent<Ingredient>());
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }
    private GameObject GetRandomIngredientInContainer()
    {
        int randomIngredientIndex = Random.Range(0, _ingredientsContainer.IngredientDataArray.Length);
        Ingredient ingredient = _ingredientsContainer.IngredientDataArray[randomIngredientIndex].IngredientPrefab;

        if (ingredient.GetIngredientID != IngredientID.bread)
            return ingredient.gameObject;

        return GetRandomIngredientInContainer();
    }

    private Vector2 GetRandomFreePosition()
    {
        int randomIndex = Random.Range(0, _coordinates.Count - 1);

        Vector2 randomCoord = _coordinates[randomIndex];
        if (!_usedCoordinates.Contains(randomCoord))
        {
            _usedCoordinates.Add(randomCoord);
            return randomCoord;
        }

        return GetRandomFreePosition();
    }

    private bool IsFreePosition(Vector2 position)
    {
        return !_usedCoordinates.Contains(position);
    }

    private Vector2 GetRandomNearPosition(Vector2 centerPosition)
    {
        List<Vector2> availablePositions = new List<Vector2>();

        // check the x-1 position and if it's in grid range
        Vector2 xMinusOne = new Vector2(centerPosition.x - 1, centerPosition.y);
        if (GridHandler.Instance.IsInGridRange(xMinusOne))
        {
            if (IsFreePosition(xMinusOne))
                availablePositions.Add(xMinusOne);
        }

        // check the x+1 position and if it's in grid range
        Vector2 xPlusOne = new Vector2(centerPosition.x + 1, centerPosition.y);
        if (GridHandler.Instance.IsInGridRange(xPlusOne))
        {
            if (IsFreePosition(xPlusOne))
                availablePositions.Add(xPlusOne);
        }

        // check the z-1 position and if it's in grid range
        Vector2 zMinusOne = new Vector2(centerPosition.x, centerPosition.y - 1);
        if (GridHandler.Instance.IsInGridRange(zMinusOne))
        {
            if (IsFreePosition(zMinusOne))
                availablePositions.Add(zMinusOne);
        }

        // check the z+1 position and if it's in grid range
        Vector2 zPlusOne = new Vector2(centerPosition.x, centerPosition.y + 1);
        if (GridHandler.Instance.IsInGridRange(zPlusOne))
        {
            if (IsFreePosition(zPlusOne))
                availablePositions.Add(zPlusOne);
        }

        if (availablePositions.Count <= 0)
        {
            // if any available position is found, return a value that tells to find another ingredient to use for the search
            return _nearPositionNotFound;
        }

        Vector2 returnPosition = availablePositions[Random.Range(0, availablePositions.Count - 1)];
        _usedCoordinates.Add(returnPosition);

        return returnPosition;
    }
    #endregion

    private Vector2 GetCoordAtTemplatePosition(int x, int y)
    {
        int xCoord;
        int yCoord;

        xCoord = (int)MapValue.Map(y, 0, 3, 0, 3);
        yCoord = (int)MapValue.Map(x, 0, 3, 0, -3);

        return new Vector2(xCoord, yCoord);
    }
}

public struct MapValue
{
    public static float Map(float input, float inMin, float inMax, float outMin, float outMax)
    {
        return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Food Level Settings")]
    [SerializeField]
    private IngredientsContainer _ingredientsContainer = null;
    [SerializeField]
    private LevelTemplate[] _foodLevelTemplates = null;

    [Header("Numbers Level Settings")]
    [SerializeField]
    private NumberData _numberData = null;
    [SerializeField]
    private LevelTemplate[] _numbersSimpleLevelTemplates = null;
    [SerializeField]
    private LevelTemplate[] _numbersStackLevelTemplate = null;

    [Space]
    [SerializeField]
    private float _instantiateHeight = 0.0f;

    /// <summary>
    /// All the coords generate at Start
    /// </summary>
    private List<Vector2> _coordinates = new List<Vector2>();

    /// <summary>
    /// All the coords occupied during level generation
    /// </summary>
    private List<Vector2> _usedCoordinates = new List<Vector2>();

    /// <summary>
    /// Used as a callback after the near ingredient position research
    /// </summary>
    private Vector2 _nearPositionNotFound = new Vector2(10000, 10000);

    private int _gridSize = 0;

    /// <summary>
    /// Extra ingredients to spawn after bread
    /// </summary>
    private int _additionalPiecesToGenerate = 0;

    private float _spawnDelay = 0.0f;

    private void Start()
    {
        _gridSize = GameHandler.Instance.GetGameSettings.GridSize;
        _spawnDelay = GameHandler.Instance.GetGameSettings.TilesSpawnDelay;
        _additionalPiecesToGenerate = GameHandler.Instance.GetExtraRandomIngredients;

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
            case GameMode.numbers_simple:
                StartCoroutine(NumberGenerationLevelFromTemplate());
                break;
            case GameMode.numbers_stack:
                StartCoroutine(NumberGenerationLevelFromTemplate());
                break;
        }
    }

    #region Food Generation From Template
    private IEnumerator FoodLevelGenerationFromTemplate()
    {
        yield return null;

        LevelTemplate randomTemplate = _foodLevelTemplates[Random.Range(0, _foodLevelTemplates.Length)];

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                string code = randomTemplate.data.GetRowsData(x)[y];
                if(code != string.Empty) // Skip wait time for animation if template row is empty
                    yield return new WaitForSeconds(_spawnDelay);

                InstantiateIngredientFromTemplate(code, x, y);
            }
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }

    private void InstantiateIngredientFromTemplate(string ingredientCode, int xTemplatePosition, int yTemplatePosition)
    {
        Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
        GameObject ingredient = null;

        if (ingredientCode == "R" || ingredientCode == "r")
        {
            ingredient = Instantiate(GetRandomIngredientInContainer(), new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "0")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "1")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bacon).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "2")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.cheese).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "3")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.egg).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "4")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.ham).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "5")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salad).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "6")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.onion).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "7")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salami).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }
        if (ingredientCode == "8")
        {
            ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.tomato).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }

        if (ingredient == null)
            return;

        ingredient.transform.localScale = Vector3.zero;
        iTween.ScaleTo(ingredient, Vector3.one, 0.15f);
        GridHandler.Instance.AddTileToGrid(ingredient.GetComponent<Ingredient>());
    }
    #endregion

    #region Number Generation From Template
    private IEnumerator NumberGenerationLevelFromTemplate()
    {
        yield return null;

        LevelTemplate[] currentLevelTemplateArray = null;

        if (GameHandler.Instance.GetGameMode == GameMode.numbers_simple)
            currentLevelTemplateArray = _numbersSimpleLevelTemplates;
        else if (GameHandler.Instance.GetGameMode == GameMode.numbers_stack)
            currentLevelTemplateArray = _numbersStackLevelTemplate;

        LevelTemplate randomTemplate = currentLevelTemplateArray[Random.Range(0, currentLevelTemplateArray.Length)];

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                string code = randomTemplate.data.GetRowsData(x)[y];
                if (code != string.Empty) // Skip wait time for animation if template row is empty
                    yield return new WaitForSeconds(_spawnDelay);

                InstantiateNumbersFromTemplate(code, x, y);
            }
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }

    private void InstantiateNumbersFromTemplate(string numberID, int xTemplatePosition, int yTemplatePosition)
    {
        if (numberID == string.Empty)
            return;

        Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
        GameObject number = Instantiate(_numberData.NumberPrefab.gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        number.transform.localScale = Vector3.zero;
        iTween.ScaleTo(number, Vector3.one, 0.15f);
        Number numberRef = number.GetComponent<Number>();
        numberRef.SetNumberID(int.Parse(numberID));
        GridHandler.Instance.AddTileToGrid(numberRef);
    }
    #endregion

    #region Random Food Generation
    private IEnumerator GenerateRandomLevel()
    {
        // Spawn the first piece of bread
        Vector2 firstPosition = GetRandomFreePosition();
        GameObject firstBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(firstPosition.x, _instantiateHeight, firstPosition.y), Quaternion.identity);
        firstBread.transform.localScale = Vector3.zero;
        iTween.ScaleTo(firstBread, Vector3.one, 0.15f);
        // Add the first bread to the Grid Ingredients list
        GridHandler.Instance.AddTileToGrid(firstBread.GetComponent<Ingredient>());

        yield return new WaitForSeconds(_spawnDelay);

        // Spawn the second piece of bread next to it, it any available position
        Vector2 nearPosition = GetRandomNearPosition(new Vector2(firstBread.transform.position.x, firstBread.transform.position.z));
        GameObject secondBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(nearPosition.x, _instantiateHeight, nearPosition.y), Quaternion.identity);
        secondBread.transform.localScale = Vector3.zero;
        iTween.ScaleTo(secondBread, Vector3.one, 0.15f);
        // Add the second bread to the Grid Ingredients list
        GridHandler.Instance.AddTileToGrid(secondBread.GetComponent<Ingredient>());


        // Spawn each ingredient near to an existing one
        for (int i = 0; i < _additionalPiecesToGenerate; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);

            // Init the random near position as a non valid position to let at least one control
            Vector2 randomNearPosition = _nearPositionNotFound;

            while (randomNearPosition == _nearPositionNotFound)
            {
                Ingredient randomGridIngredient = GridHandler.Instance.GetRandomIngredientOnGrid();
                randomNearPosition = GetRandomNearPosition(randomGridIngredient.GetCoords);
            }

            GameObject ingredient = GetRandomIngredientInContainer();
            GameObject newIngredient = Instantiate(ingredient, new Vector3(randomNearPosition.x, _instantiateHeight, randomNearPosition.y), Quaternion.identity);
            newIngredient.transform.localScale = Vector3.zero;
            iTween.ScaleTo(newIngredient, Vector3.one, 0.15f);
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

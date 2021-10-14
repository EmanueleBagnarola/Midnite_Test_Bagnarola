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

    /// <summary>
    /// Extra ingredients to spawn after bread
    /// </summary>
    [Header("Random Food Generation only")]
    [SerializeField]
    private int _additionalPiecesToGenerate = 3;

    [Header("Numbers Level Settings")]
    [SerializeField]
    private NumberData _numberData = null;
    [SerializeField]
    private LevelTemplate[] _numbersLevelTemplates = null;

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

    private int _currentLevelIndex = -1;

    private void Start()
    {
        _gridSize = GameHandler.Instance.GetGameSettings.GridSize;

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

        LevelTemplate randomTemplate = _foodLevelTemplates[Random.Range(0, _foodLevelTemplates.Length)];

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                string code = randomTemplate.data.GetRowsData(x)[y];
                if(code != string.Empty) // Skip wait time for animation if template row is empty
                    yield return new WaitForSeconds(0.2f);

                InstantiateIngredientFromTemplate(code, x, y);
            }
        }

        EventsHandler.Instance.OnLevelGenerationEnded?.Invoke();
    }

    private void InstantiateIngredientFromTemplate(string ingredientCode, int xTemplatePosition, int yTemplatePosition)
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

        Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
        Ingredient ingredient = null;

        if (ingredientCode == "R" || ingredientCode == "r")
        {
            GameObject randomIngredient = Instantiate(GetRandomIngredientInContainer(), new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
            GridHandler.Instance.AddTileToGrid(randomIngredient.GetComponent<Ingredient>());
            randomIngredient.transform.localScale = Vector3.zero;
            iTween.ScaleTo(randomIngredient, Vector3.one, 0.15f);
        }
        if (ingredientCode == "0")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.bread);  
        }
        if (ingredientCode == "1")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.bacon);
        }
        if (ingredientCode == "2")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.cheese);
        }
        if (ingredientCode == "3")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.egg);
        }
        if (ingredientCode == "4")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.ham);
        }
        if (ingredientCode == "5")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.salad);
        }
        if (ingredientCode == "6")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.onion);
        }
        if (ingredientCode == "7")
        {
            ingredient = _ingredientsContainer.GetIngredient(IngredientID.salami);
        }
        if (ingredientCode == "8")
        {
            Instantiate(_ingredientsContainer.GetIngredient(IngredientID.tomato).gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        }

        if (ingredient == null)
            return;

        GameObject ingredientObj = Instantiate(ingredient.gameObject, new Vector3(coords.x, _instantiateHeight, coords.y), Quaternion.identity);
        GridHandler.Instance.AddTileToGrid(ingredient);
        ingredientObj.transform.localScale = Vector3.zero;
        iTween.ScaleTo(ingredientObj, Vector3.one, 0.15f);
    }
    #endregion

    #region Number Generation From Template
    private IEnumerator NumberGenerationLevelFromTemplate()
    {
        yield return null;

        LevelTemplate randomTemplate = _numbersLevelTemplates[Random.Range(0, _numbersLevelTemplates.Length)];

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                string code = randomTemplate.data.GetRowsData(x)[y];
                if (code != string.Empty) // Skip wait time for animation if template row is empty
                    yield return new WaitForSeconds(0.2f);

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
        GameObject number = Instantiate(_numberData.NumberPrefab.gameObject, new Vector3(coords.x, 0f, coords.y), Quaternion.identity);
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

        yield return new WaitForSeconds(0.2f);

        // Spawn the second piece of bread next to it, it any available position
        Vector2 nearPosition = GetRandomNearPosition(new Vector2(firstBread.transform.position.x, firstBread.transform.position.z));
        GameObject secondBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(nearPosition.x, _instantiateHeight, nearPosition.y), Quaternion.identity);
        secondBread.transform.localScale = Vector3.zero;
        iTween.ScaleTo(secondBread, Vector3.one, 0.15f);
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

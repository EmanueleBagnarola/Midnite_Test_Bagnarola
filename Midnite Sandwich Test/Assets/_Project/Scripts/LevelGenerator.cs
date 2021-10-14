using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private IngredientsContainer _ingredientsContainer = null;
    [SerializeField]
    private LevelTemplate _levelTemplate = null;

    /// <summary>
    /// How many rows and columns the grid must have.
    /// The first coordinate it's always (0,0)
    /// </summary>
    [SerializeField]
    private int _gridSize = 4;

    /// <summary>
    /// Extra ingredients to spawn after bread
    /// </summary>
    [SerializeField]
    private int _additionalPiecesToGenerate = 3;

    [SerializeField]
    private bool _randomGeneration = true;

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

        if (_randomGeneration)
            StartCoroutine(GenerateRandomLevel());

        else
            StartCoroutine(GenerateLevelFromTemplate());
    }


    #region Generation From Template
    private IEnumerator GenerateLevelFromTemplate()
    {
        yield return null;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                InstantiateIngredientFromTemplate(_levelTemplate.data.GetRowsData(x)[y], x, y);
            }
        }
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
            GameObject ingredient = Instantiate(GetRandomIngredientInContainer(), new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }

        if (ingrdientCode == "0")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "1")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bacon).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "2")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.cheese).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "3")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.egg).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "4")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.ham).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "5")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salad).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "6")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.onion).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "7")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.salami).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
        if (ingrdientCode == "8")
        {
            Vector2 coords = GetCoordAtTemplatePosition(xTemplatePosition, yTemplatePosition);
            GameObject ingredient = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.tomato).gameObject, new Vector3(coords.x, 0.05f, coords.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(ingredient.GetComponent<Ingredient>());
        }
    }

    private Vector2 GetCoordAtTemplatePosition(int x, int y)
    {
        int xCoord;
        int yCoord;

        xCoord = (int)MapValue.Map(y, 0, 3, 0, 3);
        yCoord = (int)MapValue.Map(x, 0, 3, 0, -3);

        return new Vector2(xCoord, yCoord);
    }
    #endregion

    #region Random Generation
    private IEnumerator GenerateRandomLevel()
    {
        // Spawn the first piece of bread
        Vector2 firstPosition = GetRandomFreePosition();
        GameObject firstBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(firstPosition.x, 0.05f, firstPosition.y), Quaternion.identity);
        // Add the first bread to the Grid Ingredients list
        GridHandler.Instance.AddIngredientToGrid(firstBread.GetComponent<Ingredient>());

        // Spawn the second piece of bread next to it, it any available position
        Vector2 nearPosition = GetRandomNearPosition(new Vector2(firstBread.transform.position.x, firstBread.transform.position.z));
        GameObject secondBread = Instantiate(_ingredientsContainer.GetIngredient(IngredientID.bread).gameObject, new Vector3(nearPosition.x, 0.05f, nearPosition.y), Quaternion.identity);
        // Add the second bread to the Grid Ingredients list
        GridHandler.Instance.AddIngredientToGrid(secondBread.GetComponent<Ingredient>());


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
            GameObject newIngredient = Instantiate(ingredient, new Vector3(randomNearPosition.x, 0.05f, randomNearPosition.y), Quaternion.identity);
            GridHandler.Instance.AddIngredientToGrid(newIngredient.GetComponent<Ingredient>());
        }

        EventsHandler.Instance.OnRandomGenerationEnded?.Invoke();
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
}

public struct MapValue
{
    public static float Map(float input, float inMin, float inMax, float outMin, float outMax)
    {
        return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}

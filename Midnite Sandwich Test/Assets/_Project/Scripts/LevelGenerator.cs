using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /// <summary>
    /// How many rows and columns the grid must have.
    /// The first coordinate it's always (0,0)
    /// </summary>
    [SerializeField]
    private int _gridSize = 4;

    [SerializeField]
    private IngredientsContainer ingredientsContainer = null;

    private List<Vector2> _coordinates = new List<Vector2>();

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
    }
}

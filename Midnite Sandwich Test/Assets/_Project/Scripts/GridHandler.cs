using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridHandler : MonoBehaviour
{
    public static GridHandler Instance = null;

    [SerializeField]
    private List<Vector2> _occupiedPositions = new List<Vector2>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public bool IsDestinationOccupied(Vector2 Coords)
    {
        return _occupiedPositions.Contains(Coords);
    }

    public bool IsInGridRange(Vector2 positionToCheck)
    {
        return positionToCheck.x >= 0 || positionToCheck.x < 3 || positionToCheck.y > -3 || positionToCheck.y <= 0;
    }
}
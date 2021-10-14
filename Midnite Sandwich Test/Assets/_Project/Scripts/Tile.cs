using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 GetCoords
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    public Vector3 GetStartingPosition
    {
        get
        {
            return _startingPosition;
        }
    }

    private Vector3 _startingPosition = Vector3.zero;

    private void Start()
    {
        _startingPosition = transform.position;
    }
}

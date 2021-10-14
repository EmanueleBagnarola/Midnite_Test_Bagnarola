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

    private Vector3 _startingPosition = Vector3.zero;

    public virtual void Start()
    {
        _startingPosition = transform.position;
    }

    public virtual void ResetToStartingPosition()
    {
        transform.position = _startingPosition;
    }
}

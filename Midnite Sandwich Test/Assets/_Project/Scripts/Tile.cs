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
    private Vector3 _startingRotation = Vector3.zero;

    public virtual void Start()
    {
        _startingPosition = transform.position;
        _startingRotation = transform.eulerAngles;
    }

    public virtual void ResetToStartingPosition()
    {
        //transform.position = _startingPosition;
        //transform.rotation = _startingRotation;

        gameObject.SetActive(true);

        iTween.MoveTo(gameObject, _startingPosition, 0.2f);
        iTween.RotateTo(gameObject, _startingRotation, 0.2f);
    }
}

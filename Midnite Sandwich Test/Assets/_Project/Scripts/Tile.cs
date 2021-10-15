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

    public void MoveTile(Vector3 destination)
    {
        if (destination.x > GetCoords.x)
        {
            // right
            StartCoroutine(MoveAnimation(RotationDirection.right, destination));
        }
        if (destination.x < GetCoords.x)
        {
            // left
            StartCoroutine(MoveAnimation(RotationDirection.left, destination));
        }
        if (destination.z < GetCoords.y)
        {
            // down
            StartCoroutine(MoveAnimation(RotationDirection.down, destination));
        }
        if (destination.z > GetCoords.y)
        {
            // up
            StartCoroutine(MoveAnimation(RotationDirection.up, destination));
        }

        //transform.position = destination;
    }

    private IEnumerator MoveAnimation(RotationDirection rotationDirection, Vector3 destination)
    {
        Vector3 newRotation = Vector3.zero;

        switch (rotationDirection)
        {
            case RotationDirection.down:
                newRotation = new Vector3(-180, 0, 0);
                break;
            case RotationDirection.up:
                newRotation = new Vector3(180, 0, 0);
                break;
            case RotationDirection.left:
                newRotation = new Vector3(0, 0, 180);
                break;
            case RotationDirection.right:
                newRotation = new Vector3(0, 0, -180);
                break;
        }


        iTween.RotateAdd(transform.gameObject, newRotation, 0.2f);
        iTween.MoveTo(gameObject, destination, 0.3f);
        yield return null;
    }
}

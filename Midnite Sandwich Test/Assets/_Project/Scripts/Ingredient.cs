using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum IngredientID
{
    bread,
    cheese,
    bacon,
    egg,
    salad,
    ham,
    onion,
    salami,
    tomato
}

public enum RotationDirection
{
    up,
    down,
    left,
    right
}

public class Ingredient : Tile, IPointerDownHandler
{
    public IngredientID GetIngredientID
    {
        get
        {
            return _ingredientID;
        }
    }

    [SerializeField]
    private IngredientID _ingredientID;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch Ingredient: " + _ingredientID.ToString());
        EventsHandler.Instance.OnTileTouch?.Invoke(this);
    }

    public void MoveIngredient(Vector3 destination)
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
            StartCoroutine(MoveAnimation( RotationDirection.up, destination));
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

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
}

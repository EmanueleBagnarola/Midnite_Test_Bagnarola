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

public class Ingredient : MonoBehaviour, IPointerDownHandler
{
    public IngredientID GetIngredientID
    {
        get
        {
            return _ingredientID;
        }
    }

    public Vector2 GetCoords
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    [SerializeField]
    private IngredientID _ingredientID;
  
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch Ingredient: " + gameObject.name);
        EventsHandler.Instance.OnIngredientTouch?.Invoke(this);
    }
}

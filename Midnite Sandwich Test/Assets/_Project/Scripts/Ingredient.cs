using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ingredient : MonoBehaviour, IPointerDownHandler
{
    public Vector2 GetCoords
    {
        get
        {
            return _coords;
        }
    }

    private Vector2 _coords;

    private void Start()
    {
        _coords = new Vector2(transform.position.x, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnIngredientTouch: " + gameObject.name);
        EventsHandler.Instance.OnIngredientTouch?.Invoke(this);
    }
}
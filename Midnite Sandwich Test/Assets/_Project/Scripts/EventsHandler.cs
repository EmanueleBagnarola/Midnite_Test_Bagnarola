using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsHandler : MonoBehaviour
{
    public static EventsHandler Instance = null;

    [HideInInspector] public Events.EventIngredientTouch OnIngredientTouch = null;
    [HideInInspector] public Events.EventIngredientMovement OnIngredientMovement = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }
}

public class Events
{
    [System.Serializable] public class EventIngredientTouch : UnityEvent<Ingredient> { };
    [System.Serializable] public class EventIngredientMovement : UnityEvent<Ingredient, Vector2> { }

}
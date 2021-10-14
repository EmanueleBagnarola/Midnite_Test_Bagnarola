using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsHandler : MonoBehaviour
{
    public static EventsHandler Instance = null;

    [HideInInspector] public Events.EventTileTouch OnTileTouch = null;
    [HideInInspector] public Events.EventTileMovement OnTileMovement = null;
    [HideInInspector] public Events.EventMoveSuccess OnMoveSuccess = null;
    [HideInInspector] public Events.EventLevelCompleted OnLevelCompleted = null;
    [HideInInspector] public Events.LevelGenerationEnded OnLevelGenerationEnded = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}

public class Events
{
    [System.Serializable] public class EventTileTouch : UnityEvent<Tile> { };
    [System.Serializable] public class EventTileMovement : UnityEvent<Tile, Vector2> { }
    [System.Serializable] public class EventMoveSuccess : UnityEvent { }
    [System.Serializable] public class EventLevelCompleted : UnityEvent { }
    [System.Serializable] public class LevelGenerationEnded : UnityEvent { }
}

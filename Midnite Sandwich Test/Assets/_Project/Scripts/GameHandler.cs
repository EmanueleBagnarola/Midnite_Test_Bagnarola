using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EventsHandler.Instance.OnMoveSuccess?.AddListener(OnMoveSuccess);
    }

    private void OnMoveSuccess()
    {
        if (GridHandler.Instance.CheckWinCondition())
        {
            Debug.LogWarning("WIN");
        }
    }
}

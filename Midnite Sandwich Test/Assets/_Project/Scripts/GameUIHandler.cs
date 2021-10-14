using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler Instance = null;

    [SerializeField]
    private Button _restartSceneButton = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _restartSceneButton.onClick.AddListener(GameHandler.Instance.RestartScene);
    }
}

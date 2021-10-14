using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler Instance = null;

    [SerializeField]
    private Button _restartLevelButton = null;
    [SerializeField]
    private Button _generateLevelButton = null;

    [SerializeField]
    private GameObject _victoryPanel = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _restartLevelButton.transform.parent.gameObject.SetActive(false);

        EventsHandler.Instance.OnLevelCompleted?.AddListener(() => _victoryPanel.SetActive(true));
        EventsHandler.Instance.OnLevelGenerationEnded.AddListener(() => _restartLevelButton.transform.parent.gameObject.SetActive(true));

        _victoryPanel.SetActive(false);

        _restartLevelButton.onClick.AddListener(GameHandler.Instance.RestartLevel);
        _generateLevelButton.onClick.AddListener(GameHandler.Instance.GenerateNewLevel);
    }
}

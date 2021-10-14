using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    food_random,
    food_template,
    numbers
}

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance = null;

    public GameMode GetGameMode
    {
        get
        {
            return _gameMode;
        }
    }

    [SerializeField]
    private GameMode _gameMode = GameMode.food_random;

    private bool _canRestartLevel = false;

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
        InitLog();

        EventsHandler.Instance.OnMoveSuccess?.AddListener(OnMoveSuccess);
        EventsHandler.Instance.OnLevelGenerationEnded?.AddListener(() => _canRestartLevel = true);
    }

    public void RestartLevel()
    {
        if (!_canRestartLevel)
            return;

        GridHandler.Instance.ResetTilePositions();
    }

    public void GenerateNewLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMoveSuccess()
    {
        if (GridHandler.Instance.CheckWinCondition())
        {
            Debug.LogWarning("WIN");
            EventsHandler.Instance.OnLevelCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Disable Unity Log outside the editor
    /// </summary>
    private void InitLog()
    {
        #if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
        #else
        Debug.unityLogger.logEnabled = false;
        #endif
    }

}

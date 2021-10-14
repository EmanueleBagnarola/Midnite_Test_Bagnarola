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

    public GameSettings GetGameSettings
    {
        get
        {
            return _gameSettings;
        }
    }

    [SerializeField]
    private GameMode _gameMode = GameMode.food_random;

    [SerializeField]
    private GameSettings _gameSettings = null;

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

        Screen.orientation = ScreenOrientation.Portrait;

        EventsHandler.Instance.OnMoveSuccess?.AddListener(OnMoveSuccess);

    }

    public void RestartLevel()
    {
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

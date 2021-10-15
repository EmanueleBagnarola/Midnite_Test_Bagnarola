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

    public int GetExtraRandomIngredients
    {
        get
        {
            return _extraRandomIngredients;
        }
    }

    [SerializeField]
    private GameMode _gameMode = GameMode.food_random;

    [SerializeField]
    private GameSettings _gameSettings = null;

    [SerializeField]
    private int _extraRandomIngredients = 1;

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

    public void InitGameModeAndLoadScene(int extraRandomIngredients, GameMode gameMode)
    {
        _gameMode = gameMode;
        _extraRandomIngredients = extraRandomIngredients;
        SceneManager.LoadScene("Scene_Game");
    }

    public void RestartLevel()
    {
        GridHandler.Instance.ResetTilePositions();
    }

    public void GenerateNewLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

    private void OnMoveSuccess()
    {
        Invoke(nameof(CheckWinConditionDelay), 0.8f);
    }

    private void CheckWinConditionDelay()
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

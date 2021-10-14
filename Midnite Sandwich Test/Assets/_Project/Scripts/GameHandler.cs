using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance = null;

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
        EventsHandler.Instance.OnRandomGenerationEnded?.AddListener(() => _canRestartLevel = true);
    }

    public void RestartLevel()
    {
        if (!_canRestartLevel)
            return;

        GridHandler.Instance.ResetIngredientsPositions();
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

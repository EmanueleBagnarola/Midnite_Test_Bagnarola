using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGameMode(int gameModeIndex)
    {
        switch (gameModeIndex)
        {
            case 0:
                GameHandler.Instance.InitGameModeAndLoadScene(GameMode.food_random);
                break;
            case 1:
                GameHandler.Instance.InitGameModeAndLoadScene(GameMode.food_template);
                break;
            case 2:
                GameHandler.Instance.InitGameModeAndLoadScene(GameMode.numbers);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _difficultyDropdown = null;

    public void LoadGameMode(int gameModeIndex)
    {
        switch (gameModeIndex)
        {
            case 0:
                GameHandler.Instance.InitGameModeAndLoadScene(int.Parse(_difficultyDropdown.options[_difficultyDropdown.value].text), GameMode.food_random);
                break;
            case 1:
                GameHandler.Instance.InitGameModeAndLoadScene(0, GameMode.food_template);
                break;
            case 2:
                GameHandler.Instance.InitGameModeAndLoadScene(0, GameMode.numbers);
                break;
        }
    }
}

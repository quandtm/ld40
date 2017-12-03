using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public List<GameConfig> configs;

    public void BeginGame(int difficulty)
    {
        if (difficulty >= 0 && difficulty < configs.Count)
        {
            GameDirector.NextConfig = configs[difficulty];
            SceneManager.LoadScene("GameLogic", LoadSceneMode.Single);
            SceneManager.LoadScene("CamTest", LoadSceneMode.Additive);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void BeginGame(string difficulty)
    {
        SceneManager.LoadScene("GameLogic", LoadSceneMode.Single);
        SceneManager.LoadScene("CamTest", LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

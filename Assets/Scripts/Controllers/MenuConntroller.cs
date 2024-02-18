using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuConntroller : MonoBehaviour
{
    public string gameScene;
    public void NewGame()
    {
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
        
    }
}

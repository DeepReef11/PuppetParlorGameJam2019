using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void SceneChange(int changeTheScene)
    {
        SceneManager.LoadScene(changeTheScene);
        Debug.Log("change");
    }
    public void Exit()
    {
        Application.Quit();
    }
}

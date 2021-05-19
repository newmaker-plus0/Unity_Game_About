using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EnterGame : MonoBehaviour
{
    public void PlayGame() //場景載入
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() //退出遊戲
    {
        Application.Quit();
    }
}

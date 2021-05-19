using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void Load()
    {
        SceneManager.LoadScene(1); //載入場景
    }
}

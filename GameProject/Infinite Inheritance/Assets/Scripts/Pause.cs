using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject status;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Play()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Status()
    {
        status.SetActive(true);
    }

    public void StatusQuit()
    {
        status.SetActive(false);
    }
}

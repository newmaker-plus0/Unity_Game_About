using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaringSystem : MonoBehaviour
{
    public static WaringSystem instance;
    public GameObject waringHome;
    public Text waring;

    //單例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    //顯示
    public void Show(string reminder)
    {
        waring.text = reminder;
        waringHome.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Events;

public class ClassManager : MonoBehaviour
{
    [Header("關卡屬性")]
    public Text level;

    static public int levelNum = 1; //現在關卡數

    public Animator anim;

    public CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void Run()
    {
        if(levelNum!=1)
        {
            GameObject.Find("Buff").SendMessage("Button");
            Time.timeScale = 0.2f;
        }
        PlayGame(); 
        Invoke("Record", 2f);
    }

    public void Record() //怪物激活
    {
        if ((levelNum % 10) == 1)
        {
            virtualCamera.m_Lens.OrthographicSize = 10;
        }
        else { virtualCamera.m_Lens.OrthographicSize = 7; }
        EnemyAppearManager.enemy = 0;
        EnemyAppearManager.run = 0;
        anim.SetBool("classOn", false);
    }

    void PlayGame() //關卡數顯示
    {
        level.text = levelNum.ToString();
        levelNum += 1;
        anim.SetBool("classOn", true);
    }
}

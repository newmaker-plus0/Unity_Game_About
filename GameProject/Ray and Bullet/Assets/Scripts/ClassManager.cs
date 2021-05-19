using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClassManager : MonoBehaviour
{
    [Header("關卡屬性")]
    public Text level;

    static public int levelNum = 0; //現在關卡數

    public Animator anim;
    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        if (levelNum < 0) levelNum = 0;
    }

    public void Run()
    {
        PlayGame(); 
        Invoke("Record", 2f); //兩秒之後激活
    }

    public void Record() //怪物激活
    {
        EnemyAppearManager.enemyStatic = 0; //是否召喚怪物
        EnemyAppearManager.runStatic = 0; //是否啟動中
        anim.SetBool("classOn", false);
    }

    void PlayGame() //關卡數顯示
    {
        level.text = levelNum.ToString();
        anim.SetBool("classOn", true);
    }
}

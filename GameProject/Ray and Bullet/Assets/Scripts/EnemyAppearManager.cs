using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearManager : MonoBehaviour
{
    [Header("關卡中敵人訊息")]
    static public int enemyStatic = 1; //是否召喚怪物
    public int enemyCount; //標準怪物數量
    static public int nowEnemyCountStatic; //現有怪物數量
    static public int runStatic = 0; //是否啟動中

    private float nowX, nowY; //怪物生成點的初始位置

    public GameObject enemyFireElement; //怪物:火元素
    
    private void Start()
    {
        enemyStatic = 1;
        enemyCount = 0;
        nowEnemyCountStatic = 0;
        runStatic = 0;

        nowX = this.transform.position.x;
        nowY = this.transform.position.y;
    }

    private void Update()
    {      
        if (enemyStatic == 0) //召喚怪物
        {
            enemyCount = (ClassManager.levelNum-1) % 5; //該關卡的怪物計數器

            for (int i = 0; i <= enemyCount; i++) //根據計術結果召喚怪物
            {
                Appear();
            }
            enemyStatic = 1; //設為無須召喚怪物
        }
        
        Record();
    }

    void Appear() //隨機出現位置
    {
        float x = Random.Range(-14, 14);
        float y = Random.Range(-5, 5);

        this.transform.position = new Vector2(this.transform.position.x - x, this.transform.position.y - y);
        Instantiate(enemyFireElement, this.transform.position, enemyFireElement.transform.rotation);

        ReSetDir(); //重設初始座標

        nowEnemyCountStatic += 1; //現有怪物數量增加
    }

    void Record() //重新激活
    {
        if (nowEnemyCountStatic == 0 && runStatic == 0) //現有怪物數量為零，及未處於召喚怪物的期間
        {
            ClassManager.levelNum++;
            GameObject.Find("GameStartCanvas").SendMessage("Run");
            runStatic = 1;
        }
    }

    void ReSetDir() //重設座標點
    {
        this.transform.position = new Vector2(nowX, nowY);
    }
}

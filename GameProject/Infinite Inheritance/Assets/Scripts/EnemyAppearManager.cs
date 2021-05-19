using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearManager : MonoBehaviour
{
    [Header("關卡中敵人訊息")]
    static public int enemy = 1;
    public int enemyCount;
    static public int nowEnemyCount;
    static public int run = 0;

    public GameObject enemyBat;
    public GameObject enemyBeast;
    public GameObject boss10;

    public int score=0;

    public Transform point; //出現位置

    public int timeScore;

    private void Start()
    {
        enemy = 1;
        enemyCount = 0;
        nowEnemyCount = 0;
        run = 0;
        timeScore = 10;
        InvokeRepeating("Score", 1.0f, 1.0f);
    }

    private void Update()
    {      
        if (enemy == 0) //召喚怪物
        {
            enemyCount = (ClassManager.levelNum-1) * 1 % 5;
            if(enemyCount==0)
            {
                Appear();
            }

            for (int i = 0; i < enemyCount; i++)
            {
                Appear();
            }
            enemy = 1;
        }
        
        Record();
    }

    void Appear() //隨機出現位置
    {
        float x = Random.Range(0, 10);
        float y = Random.Range(0, 10);

        if ((ClassManager.levelNum % 10) != 1)
        {
            point.transform.position = new Vector2(point.transform.position.x + x, point.transform.position.y + y);
            Instantiate(enemyBat, point.transform.position, enemyBat.transform.rotation);
            point.transform.position = new Vector2(point.transform.position.x - x, point.transform.position.y - y);
            Instantiate(enemyBeast, point.transform.position, enemyBeast.transform.rotation);

            nowEnemyCount += 2;
        }else if((ClassManager.levelNum%10)==1)
        {
            point.transform.position = new Vector2(point.transform.position.x - x, point.transform.position.y - y);
            Instantiate(boss10, point.transform.position, boss10.transform.rotation);
            
            nowEnemyCount += 1;
        }
    }

    void Record() //重新激活
    {
        if (nowEnemyCount == 0 && run == 0)
        {
            if(score==1)
            {
                ScoreInformation.scorePower += timeScore * 10 * ((ClassManager.levelNum / 2) + 1);
                timeScore = 50;
            }

            GameObject.Find("GameStartCanvas").SendMessage("Run");
            run = 1;
            score = 1;
        }
    }

    public void Score()
    {
        if(timeScore>1)
        {
            timeScore -= 1;
        }
    }
}

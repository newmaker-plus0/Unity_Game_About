using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store: MonoBehaviour
{
    public static Store instance;

    /*//單字
    public struct wordMode
    {
        public string foreign;
        public string native;
        public float correct;
        public float error;
        public float ratio;

        public void Count()
        {
            if (error != 0f) ratio = correct / (error + correct);
            else ratio = 1;
        }
    };
    //群組
    public struct groupMode
    {
        public string name;
        public int wordNum;
        public float correct;
        public float error;
        public float ratio;
        public wordMode[] word;

        public void Count()
        {
            correct = 0;
            error = 0;
            for(int i=0;i<word.Length;i++)
            {
                correct += word[i].correct;
                error += word[i].error;
            }

            if (error > 0) ratio = correct / (error + correct);
            else ratio = 1;
        }
    };
    //玩家資料
    public struct Info
    {
        public string name;
        public string classRank;
        public int lv;
        public float experienceValue;
        public int point;
        public int nextLvExprience, nowLvExprience;

        //計算下一個等級的經驗值，並清空現有經驗值
        public void CountLV()
        {
            nextLvExprience = (int)(Mathf.Pow((lv / 2) + 2, 2) / 2);
        }

        //計算出顯示滑條
        public void CountLvValue()
        {
            experienceValue = (float)nowLvExprience / nextLvExprience;
        }

        //檢查等級是否上升
        public void LvUpCheck()
        {
            if (experienceValue >= 1)
            {
                lv += 1;
                nowLvExprience -= nextLvExprience;
                CountLV();
                CountLvValue();
                if (experienceValue >= 1) LvUpCheck();
            }
        }
    }
    */

    public Struct.groupMode [] group = new Struct.groupMode [0];

    public Struct.Info player;

    //單例
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    [Header("Buff基礎輸入輸出")]
    int a, b, c;
    public Text name1, name2, name3;
    public Text intro1, intro2, intro3;

    [Header("Buff_004巨大劍刃")]
    public GameObject slashScale;
    public float scaleX, scaleY;
    static public float nowScaleX=1.661f;
    static public float nowScaleY=1.414f;

    [Header("Buff_005移動速度增加")]
    public float speedUP;

    [Header("Buff_006衝鋒時間增加")]
    public float dashTimeUp;

    [Header("Buff_007衝鋒速度增加")]
    public float dashSpeedUp;

    [Header("Buff_008衝鋒冷卻時間減少")]
    public float dashCoolTimeDown;

    [Header("Buff_012流血效果")]
    static public int bloodEffectDamage = 0;
    static public int bloodEffectOn = 0;

    static public float critDamage=2.0f; //暴擊傷害
    static public int critChance = 21; //暴擊機率

    private void Start()
    {
        if (Load01.save == 0)
        {
            slashScale.gameObject.transform.localScale = new Vector3(1.661f, 1.414f, 0f);
        }
        else if(Load01.save == 1)
        {
            slashScale.gameObject.transform.localScale = new Vector3(nowScaleX, nowScaleY, 0f);
        }  
    }

    public void Button() //呼叫BUFF系統
    {
        transform.GetChild(0).gameObject.SetActive(true);
        a = Random.Range(1, 13);
        b = Random.Range(1, 13);
        c = Random.Range(1, 13);
        Show(a, name1, intro1);
        Show(b, name2, intro2);
        Show(c, name3, intro3);
    }

    public void Show(int a, Text name, Text intro) //展示BUFF屬性
    {
        if (a > 10) Show_001(a, name, intro);
        switch (a)
        {
            case 1:
                name.text = BuffData.buff_001.name;
                intro.text = BuffData.buff_001.intro;
                break;
            case 2:
                name.text = BuffData.buff_002.name;
                intro.text = BuffData.buff_002.intro;
                break;
            case 3:
                name.text = BuffData.buff_003.name;
                intro.text = BuffData.buff_003.intro;
                break;
            case 4:
                name.text = BuffData.buff_004.name;
                intro.text = BuffData.buff_004.intro;
                break;
            case 5:
                name.text = BuffData.buff_005.name;
                intro.text = BuffData.buff_005.intro;
                break;
            case 6:
                name.text = BuffData.buff_006.name;
                intro.text = BuffData.buff_006.intro;
                break;
            case 7:
                name.text = BuffData.buff_007.name;
                intro.text = BuffData.buff_007.intro;
                break;
            case 8:
                name.text = BuffData.buff_008.name;
                intro.text = BuffData.buff_008.intro;
                break;
            case 9:
                name.text = BuffData.buff_009.name;
                intro.text = BuffData.buff_009.intro;
                break;
            case 10:
                name.text = BuffData.buff_010.name;
                intro.text = BuffData.buff_010.intro;
                break;
        }
    }

    public void Show_001(int a, Text name, Text intro) //展示BUFF屬性_001
    {
        switch (a)
        {
            case 11:
                name.text = BuffData.buff_011.name;
                intro.text = BuffData.buff_011.intro;
                break;
            case 12:
                name.text = BuffData.buff_012.name;
                intro.text = BuffData.buff_012.intro;
                break;
        }
    }

    public void Inter1() //一號輸入埠
    {
        BuffOn(a);
    }

    public void Inter2() //二號輸入埠
    {
        BuffOn(b);
    }

    public void Inter3() //三號輸入埠
    {
        BuffOn(c);
    }

    public void BuffOn(int a) //BUFF效果啟動機
    {
        if (a > 10) BuffOn_001(a);
        switch(a)
        {
            case 1:
                LimitAtkUp_001();
                break;
            case 2:
                FloorAtkUp_002();
                break;
            case 3:
                SlashNumUp_003();
                break;
            case 4:
                SlashZoom_004();
                break;
            case 5:
                MoveSpeedUp_005();
                break;
            case 6:
                DashTimeUp_006();
                break;
            case 7:
                DashSpeedUp_007();
                break;
            case 8:
                DashCoolTimeDown_008();
                break;
            case 9:
                CritChanceUp_009();
                break;
            case 10:
                CritDamageUp_010();
                break;
        }
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BuffOn_001(int a) //BUFF效果啟動機_001
    {
        switch (a)
        {
            case 11:
                HpUp_011();
                break;
            case 12:
                BloodEffect_012();
                break;
        }
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1f;
    }






    public void LimitAtkUp_001() //攻擊力上限增加
    {
        Slash.maxDamage += 100;
        Load01.max += 100;
    }

    public void FloorAtkUp_002() //攻擊力下限增加
    {
        Slash.minDamage += 100;
        Load01.min += 100;
    }

    public void SlashNumUp_003() //劍刃數增加
    {
        PlayerAttack.slashNum += 1;
    }

    public void SlashZoom_004() //劍刃巨大化
    {
        slashScale.gameObject.transform.localScale += new Vector3(scaleX, scaleY, 0f);
        nowScaleX += scaleX;
        nowScaleY += scaleY;
    }

    public void MoveSpeedUp_005() //移動速度增加
    {
        PlayerMovement.speedUp += speedUP;
        GameObject.Find("Player").SendMessage("SpeedUp");
    }

    public void DashTimeUp_006() //衝鋒時間增加
    {
        PlayerMovement.dashTimeUp += dashTimeUp;
        GameObject.Find("Player").SendMessage("DashTimeUp");
    }

    public void DashSpeedUp_007() //衝峰速度增加
    {
        PlayerMovement.dashSpeedUp += dashSpeedUp; 
        GameObject.Find("Player").SendMessage("DashSpeedUp");
    }

    public void DashCoolTimeDown_008() //衝峰冷卻時間減少
    {
        PlayerMovement.dashCoolTimeDown += dashCoolTimeDown;
        GameObject.Find("Player").SendMessage("DashCoolTimeDown");
    }

    public void CritChanceUp_009() //暴擊機率上升
    {
        critChance -= 1;
    }

    public void CritDamageUp_010() //暴擊傷害上升
    {
        critDamage += 0.5f;
    }

    public void HpUp_011() //生命值增加
    {
        PlayerHealth.hpUp += 200.0f;
        GameObject.Find("Player").SendMessage("HpUp");
    }

    public void BloodEffect_012() //流血效果
    {
        bloodEffectOn = 1;
        bloodEffectDamage += 30;
    }
}

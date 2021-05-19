using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load01 : MonoBehaviour
{
    static public int save;

    static public float maxHp;
    static public float hp;
    static public float speed;
    public int level;
    public int slashNum;
    public float dashTime, dashCoolDown, dashSpeed;
    public int bloodEffectOn, bloosEffectDamage;
    public float critDamage;
    public int critChance;
    public float scaleX, scaleY;
    static public float max, min;
    public int score;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            save = 1;

            //數據獲取
            maxHp = PlayerHealth.maxNowHp;
            hp = PlayerHealth.nowHp;
            speed = PlayerMovement.nowSpeed;
            level = ClassManager.levelNum;
            slashNum = PlayerAttack.slashNum;
            dashTime = PlayerMovement.dashTime;
            dashCoolDown = PlayerMovement.dashCoolDown;
            dashSpeed = PlayerMovement.dashSpeed;
            bloodEffectOn = Buff.bloodEffectOn;
            bloosEffectDamage = Buff.bloodEffectDamage;
            critDamage = Buff.critDamage;
            critChance = Buff.critChance;
            scaleX = Buff.nowScaleX;
            scaleY = Buff.nowScaleY;
            max = Slash.maxDamage;
            min = Slash.minDamage;
            score = ScoreInformation.scorePower;

            PlayerPrefs.SetInt("save", save); //是否儲存

            //數據預製化
            PlayerPrefs.SetFloat("maxHp", maxHp);
            PlayerPrefs.SetFloat("hp", hp);
            PlayerPrefs.SetFloat("speed", speed);
            PlayerPrefs.SetInt("level", level);
            PlayerPrefs.SetInt("slashNum", slashNum);
            PlayerPrefs.SetFloat("dashTime", dashTime);
            PlayerPrefs.SetFloat("dashCoolDown", dashCoolDown);
            PlayerPrefs.SetFloat("dashSpeed", dashSpeed);
            PlayerPrefs.SetInt("bloodEffectOn", bloodEffectOn);
            PlayerPrefs.SetInt("bloodEffectDamage", bloosEffectDamage);
            PlayerPrefs.SetFloat("critDamage", critDamage);
            PlayerPrefs.SetInt("critChance", critChance);
            PlayerPrefs.SetFloat("scaleX", scaleX);
            PlayerPrefs.SetFloat("scaleY", scaleY);
            PlayerPrefs.SetFloat("max", max);
            PlayerPrefs.SetFloat("max", max);
            PlayerPrefs.SetInt("score", score);
        }
    }

    public void Load()
    {
        if(save==0)
        {
            maxHp = 100;
            hp = 100;
            speed = 5;
            ClassManager.levelNum = 1;
            PlayerAttack.slashNum = 0;
            PlayerMovement.dashTime = 0.1f;
            PlayerMovement.dashCoolDown = 3;
            PlayerMovement.dashSpeed = 10;
            Buff.bloodEffectOn = 0;
            Buff.bloodEffectDamage = 0;
            Buff.critDamage = 2.0f;
            Buff.critChance = 21;
            Buff.nowScaleX = 1.616f;
            Buff.nowScaleY = 1.414f;
            Slash.maxDamage = 50;
            Slash.minDamage = 30;
            ScoreInformation.scorePower = 0;
        }else if(save==1)
        {
            maxHp = PlayerPrefs.GetFloat("maxHp");
            hp = PlayerPrefs.GetFloat("hp");
            speed = PlayerPrefs.GetFloat("speed");
            ClassManager.levelNum = PlayerPrefs.GetInt("level");
            PlayerAttack.slashNum = PlayerPrefs.GetInt("slashNum");
            PlayerMovement.dashTime = PlayerPrefs.GetFloat("dashTime");
            PlayerMovement.dashCoolDown = PlayerPrefs.GetFloat("dashCoolDown");
            PlayerMovement.dashSpeed = PlayerPrefs.GetFloat("dashSpeed");
            Buff.bloodEffectOn = PlayerPrefs.GetInt("bloodEffectOn");
            Buff.bloodEffectDamage = PlayerPrefs.GetInt("bloodEffectDamage");
            Buff.critDamage = PlayerPrefs.GetFloat("critDamage");
            Buff.critChance = PlayerPrefs.GetInt("critChance");
            Buff.nowScaleX = PlayerPrefs.GetFloat("scaleX");
            Buff.nowScaleY = PlayerPrefs.GetFloat("scaleY");
            Slash.maxDamage = PlayerPrefs.GetFloat("max");
            Slash.minDamage = PlayerPrefs.GetFloat("min");
            ScoreInformation.scorePower = PlayerPrefs.GetInt("score");
        }
    }
    
}

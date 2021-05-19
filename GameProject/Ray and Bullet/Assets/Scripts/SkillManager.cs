using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance; //單例，方便調用，記得Awake
    public SkillData activeSkill; //激活的技能

    [Header("UI")]
    public Image skillImage; //技能圖案
    public Text skillNameText, skillLvText, skillDesText; //技能相關描述

    [Header("技能點數")]
    public Text pointText;

    public SkillButton[] skillButtons; //存在的按鈕數組，要新增在unity中，才可被使用

    public void Awake() //喚醒該物件
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

        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        activeSkill = skillButtons[0].skillData; //預設激活的技能為0

        for (int i = 0; i < skillButtons.Length; i++) //更新儲存技能
        {
            skillButtons[i].skillData.skillLevel = NowSkill.nowSkillLevelInGame[i];
        }
        DisplaySkillInfo(); //顯示技能點，及更新面板
        UpdateUI(); //同上

        

    }

    public void UpgradeButton() //升級按鈕
    {
        if (activeSkill == null) //如未獲取技能，返回
            return;
        if(NowSkill.nowSkillPoint > 0 && activeSkill.preSkills.Length == 0 && activeSkill.skillLevel < activeSkill.skillMaxLevel) //如果技能點大於0，且前置技能為0，未達最大技能等級，可升級
        {
            UpdateSkill();
        }

        if(NowSkill.nowSkillPoint > 0 && activeSkill.skillLevel < activeSkill.skillMaxLevel) //存在前置技能，判斷是否全解鎖
        {
            int skillOk = activeSkill.preSkills.Length;
            for(int i=0;i<activeSkill.preSkills.Length;i++)
            {
                if(activeSkill.preSkills[i].isUnlocked == true)
                {
                    skillOk--;
                }
            }

            if (skillOk == 0)
                UpdateSkill();
        }
    }

    private void UpdateSkill() //升級技能
    {
        skillButtons[activeSkill.skillID].GetComponent<Image>().color = Color.white; //點亮技能圖標
        skillButtons[activeSkill.skillID].transform.GetChild(0).gameObject.SetActive(true); //激活技能等級
        activeSkill.skillLevel++; //升級
        skillButtons[activeSkill.skillID].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = activeSkill.skillLevel.ToString();

        DisplaySkillInfo(); //刷新面板

        NowSkill.nowSkillPoint--;
        UpdateUI();

        if (activeSkill.skillLevel == activeSkill.skillMaxLevel) //滿級技能，即解鎖
            activeSkill.isUnlocked = true;
    }

    public void DisplaySkillInfo()
    {
        skillImage.sprite = activeSkill.skillSprite;
        skillNameText.text = activeSkill.skillName;
        skillLvText.text = "LV: " + activeSkill.skillLevel + "/" + activeSkill.skillMaxLevel;
        skillDesText.text = "DESCRIPTIONS:\n" + activeSkill.skillDes;
    }

    private void UpdateUI()
    {
        pointText.text = "POINTS: " + NowSkill.nowSkillPoint.ToString() + "/100";
        
        for(int i=0;i<skillButtons.Length;i++)
        {
            if(skillButtons[i].skillData.skillLevel > 0)
            {
                skillButtons[i].GetComponent<Image>().color = Color.white;
                skillButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                skillButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skillButtons[i].skillData.skillLevel.ToString();
            }
        }
    }
}

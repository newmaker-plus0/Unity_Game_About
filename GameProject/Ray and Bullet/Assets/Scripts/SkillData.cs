using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Skill", fileName = "Skill")]
public class SkillData : ScriptableObject
{
    public int skillID; //技能ID
    public Sprite skillSprite; //技能圖片

    public string skillName; //技能名字
    public int skillLevel; //技能等級
    public int skillMaxLevel; //技能最大等級
    [TextArea(1, 8)]
    public string skillDes; //技能描述

    public bool isUnlocked; //是否解鎖
    public SkillData[] preSkills; //前置技能
}


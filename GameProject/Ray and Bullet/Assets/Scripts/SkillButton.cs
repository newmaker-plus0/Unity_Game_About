using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    public SkillData skillData; //此按鍵對應技能資料

    public void OnPointerClick(PointerEventData eventData) //鼠標點擊，激活
    {
        SkillManager.instance.activeSkill = skillData; //將激活技能設為此技能
        SkillManager.instance.DisplaySkillInfo(); //更新面板
    }
}

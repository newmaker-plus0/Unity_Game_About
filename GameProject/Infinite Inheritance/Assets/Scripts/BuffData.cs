using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffData : MonoBehaviour
{
    public struct buff //標準化數據庫
    {
        public string name;
        public string intro;
    }; 

    static public buff buff_001 = new buff //攻擊力上限增加
    {
        name = "Limit_AtkUp",
        intro = "Limit attack +100",
    }; 

    static public buff buff_002 = new buff //攻擊力下限增加
    {
        name = "Floor_AtkUp",
        intro = "Floor attack +100",
    };

    static public buff buff_003 = new buff //劍刃數增加
    {
        name = "Slash_NumUp",
        intro = "Increased damage per attack +1"
    };

    static public buff buff_004 = new buff //劍刃巨大化
    {
        name = "Slash_ZooM",
        intro = "Magnify the slash +0.3",
    };

    static public buff buff_005 = new buff //移動速度增加
    {
        name = "Move_SpeedUp",
        intro = "Increased movement speed +0.5",
    };

    static public buff buff_006 = new buff //衝鋒時間增加
    {
        name = "Dash_TimeUp",
        intro = "Increased dash time +0.005s",
    };

    static public buff buff_007 = new buff //衝鋒速度增加
    {
        name = "Dash_SpeedUp",
        intro = "Increased dash speed +3",
    };

    static public buff buff_008 = new buff //衝鋒冷卻時間
    {
        name = "Dash_CoolTimeDown",
        intro = "Reduce dash cooldown",
    };

    static public buff buff_009 = new buff //暴擊機率上升
    {
        name = "Crit_ChanceUp",
        intro = "Critical chance +5%",
    };

    static public buff buff_010 = new buff //暴擊傷害增加
    {
        name = "Crit_DamageUp",
        intro = "Critical damage + 50%",
    };

    static public buff buff_011 = new buff //生命值增加
    {
        name = "Hp_Up",
        intro = "Hp increased +200",
    };

    static public buff buff_012 = new buff //斬擊附帶流血效果
    {
        name = "Bleed_Effect",
        intro = "Slash comes bleed effect, causing damage per second +30",
    };
}


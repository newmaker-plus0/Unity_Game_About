using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//宣告儲存內容的區塊

[System.Serializable]
public class Save
{
    //玩家
    public int playerLevelSave;
    public int levelNumSave;
    public float currentExpSave;
    public bool bulletBaseGiveSave;
    public int[] bulletNowNumSave = new int[7];
    public float nowTrackerEnergySave;
    public int storeTimesSave;
}
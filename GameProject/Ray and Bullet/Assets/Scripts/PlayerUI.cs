using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerUI : MonoBehaviour
{
    public Animator anim;
    private int skillPointSave;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) //暫停
        {
            if (anim.GetInteger("Open") == 1)
                Open();
            else
                Zero();
        }
    }

    void Open()
    {
        anim.SetInteger("Open", 0);
    }

    void Zero()
    {
        Time.timeScale = 1; //將時間流速轉回正常
        anim.SetInteger("Open", 1);
    }

    private Save CreateSaveGame() //生成遊戲檔案
    {
        Save save = new Save();

        save.playerLevelSave = PlayerMove.playerLevel; //玩家最大等級
        save.levelNumSave = (ClassManager.levelNum - 1); //儲存關卡數
        save.currentExpSave = PlayerMove.currentExp; //儲存玩家現有經驗值
        save.bulletBaseGiveSave = Weapon.bulletBaseGive; //儲存是否給基本子彈
        save.nowTrackerEnergySave = Weapon.nowTrackerEnergy; //儲存現有能量
        save.storeTimesSave = Weapon.storeTimes; //儲存現有能量

        //儲存玩家現有子彈
        for (int i=0 ; i<7 ; i++) 
        {
            save.bulletNowNumSave[i] = Weapon.bulletNowNum[i];
        }

        return save;
    }

    private SaveSkill CreatSaveSkill() //生成技能檔案
    {
        SaveSkill saveSkill = new SaveSkill();

        SkillManager.instance.Awake();
        for (int i = 0; i < SkillManager.instance.skillButtons.Length; i++) //儲存技能升級情形
        {
            saveSkill.skillLevelSave[i] = SkillManager.instance.skillButtons[i].skillData.skillLevel;
            Debug.Log(SkillManager.instance.skillButtons[i].skillData.skillName);
            Debug.Log(SkillManager.instance.skillButtons[i].skillData.skillLevel);
        }
        

        return saveSkill;
    }

    private SaveSkillPoint CreatSaveSkillPoint() //生成技能點數檔案
    {
        SaveSkillPoint saveSkillPoint = new SaveSkillPoint();

        saveSkillPoint.skillPointSave = NowSkill.nowSkillPoint;

        return saveSkillPoint;
    }

    private void SaveBySerization() //儲存遊戲檔案
    {
        Save save = CreateSaveGame();

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fileStream = File.Create(Application.persistentDataPath + "/Data.text"); 
        bf.Serialize(fileStream, save); //將資料存入

        //儲存技能點
        SaveSkillPoint saveSkillPoint = CreatSaveSkillPoint();

        fileStream = File.Create(Application.persistentDataPath + "/DataSkillPoint.text");
        bf.Serialize(fileStream, saveSkillPoint); 

        fileStream.Close(); //閉上流

        Debug.Log("-------SAVED--------");
    }

    private void SaveBySerizationSkill() //儲存技能檔案
    {
        SaveSkill saveSkill = CreatSaveSkill();

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fileStream = File.Create(Application.persistentDataPath + "/DataSkill.text");
        bf.Serialize(fileStream, saveSkill); //將資料存入

        //儲存技能點
        SaveSkillPoint saveSkillPoint = CreatSaveSkillPoint();

        fileStream = File.Create(Application.persistentDataPath + "/DataSkillPoint.text");
        bf.Serialize(fileStream, saveSkillPoint);

        fileStream.Close(); //閉上流

        Debug.Log("-------SAVEDSKILL--------");
    }

    private void LoadByJson() //載入遊戲檔案
    {
        if(File.Exists(Application.persistentDataPath + "/Data.text"))
        {
            BinaryFormatter bf = new BinaryFormatter(); 

            FileStream fileStream = File.Open(Application.persistentDataPath + "/Data.text", FileMode.Open); //從路徑中找到資料

            Save save = bf.Deserialize(fileStream) as Save; //獲取儲存的數據，轉為Save庫
            fileStream.Close(); //閉上流

            Debug.Log("-------LOADED--------");

            ClassManager.levelNum = save.levelNumSave;
            PlayerMove.playerLevel = save.playerLevelSave;
            PlayerMove.currentExp = save.currentExpSave;
            Weapon.bulletBaseGive = save.bulletBaseGiveSave;
            Weapon.nowTrackerEnergy = save.nowTrackerEnergySave;
            Weapon.storeTimes = save.storeTimesSave;
            
            //載入玩家現有子彈
            for (int i = 0; i < 7; i++) 
            {
                Weapon.bulletNowNum[i] = save.bulletNowNumSave[i];
            }
        }
        else
        {
            Debug.Log("NO FIND FILES");
        }

        if (File.Exists(Application.persistentDataPath + "/DataSkill.text"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/DataSkill.text", FileMode.Open); //從路徑中找到資料

            SaveSkill saveSkill = bf.Deserialize(fileStream) as SaveSkill; //獲取儲存的數據，轉為Save庫
            fileStream.Close(); //閉上流

            Debug.Log("-------LOADEDSKILL--------");

            //進入遊戲介面，獲取技能等級
            for (int i = 0; i < saveSkill.skillLevelSave.Length; i++)
            {
                NowSkill.nowSkillLevelInGame[i] = saveSkill.skillLevelSave[i];
            }

        }
    }

    private void LoadByJsonSkill() //載入技能檔案
    {
        if (File.Exists(Application.persistentDataPath + "/DataSkill.text"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/DataSkill.text", FileMode.Open); //從路徑中找到資料

            SaveSkill saveSkill = bf.Deserialize(fileStream) as SaveSkill; //獲取儲存的數據，轉為Save庫
            fileStream.Close(); //閉上流

            Debug.Log("-------LOADEDSKILL--------");

            //進入技能樹介面，載入技能等級
            for (int i = 0; i < saveSkill.skillLevelSave.Length; i++)
            {
                NowSkill.nowSkillLevelInGame[i] = saveSkill.skillLevelSave[i];
            }
        }

        if (File.Exists(Application.persistentDataPath + "/DataSkillPoint.text"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/DataSkillPoint.text", FileMode.Open); //從路徑中找到資料

            SaveSkillPoint saveSkillPoint = bf.Deserialize(fileStream) as SaveSkillPoint; //獲取儲存的數據，轉為Save庫
            fileStream.Close(); //閉上流


            NowSkill.nowSkillPoint = saveSkillPoint.skillPointSave;
            Debug.Log("-------LOADEDSKILLPOINT-------- : " + NowSkill.nowSkillPoint);
        }
    }

    public void Save() //存檔
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SaveBySerizationSkill();
        else if(SceneManager.GetActiveScene().buildIndex == 1)
            SaveBySerization();
    }

    public void Load() //載入
    {
        LoadByJson();
    }

    public void Pause() //暫停
    {
        Time.timeScale = 0;
    }

    public void Back() //回到主場景
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void LoadSkillTree() //載入技能樹
    {
        LoadByJsonSkill();
        SceneManager.LoadScene(2);
    }
}

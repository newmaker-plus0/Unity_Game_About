using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour
{
    public PlayerSystem instance;

    //資料
    [Header("玩家資料")]
    public Text playerName;
    public Text playerLv;
    public Text playerClass;
    public Text playerPoint;
    public GameObject playerInformation;

    //詳細資料
    [Header("玩家詳細資料面板")]
    public InputField nameInput;
    public Text show;
    public Text get;
    public Text classRank;
    public Text lv;
    public Slider exprience;
    public Text totalWord, ratio, exprienceText;

    //單例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        nameInput.onEndEdit.AddListener(SetName);
    }

    private void Update()
    {
        playerName.text = Store.instance.player.name;
        playerLv.text = "LV:" + Store.instance.player.lv.ToString();
        playerClass.text = Store.instance.player.classRank;
        playerPoint.text = "P:" + Store.instance.player.point.ToString();
    }

    //打開資訊界面
    public void OpenInformation()
    {
        Store.instance.player.CountLV();
        Store.instance.player.CountLvValue();
        Store.instance.player.LvUpCheck();
        playerInformation.SetActive(true);
        show.text = Store.instance.player.name;
        classRank.text = Store.instance.player.classRank;
        lv.text = "LV:" + Store.instance.player.lv.ToString();
        exprience.value = Store.instance.player.experienceValue;
        exprienceText.text = Store.instance.player.experienceValue.ToString("0") + "%";

        int totalWordTem = 0;
        float ratioTem = 0f;
        for(int i=0;i<Store.instance.group.Length;i++)
        {
            totalWordTem += Store.instance.group[i].wordNum;
            ratioTem += Store.instance.group[i].ratio;
        }
        ratioTem /= Store.instance.group.Length;

        totalWord.text = "總單字數:" + totalWordTem.ToString();
        ratio.text = "答對率:" + (ratioTem * 100).ToString("0.00") + "%";
    }

    //關閉資訊界面
    public void CloseInformation()
    {
        playerInformation.SetActive(false);
    }

    //設定名字
    public void SetName(string nameTem)
    {
        Store.instance.player.name = nameTem;
        get.text = null;
        show.text = Store.instance.player.name;

        StoreSystem.instance.Save();
        //StoreSystem.instance.SaveBySerialization();
    }
}

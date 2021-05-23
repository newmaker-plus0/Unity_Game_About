using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSystem : MonoBehaviour
{
    public static BasicSystem instance;

    //主頁
    [Header("主要頁面")]
    public GameObject catalog; //目錄
    public GameObject wareHouse; //倉庫

    //輸入
    [Header("輸入")]
    public GameObject inputFieldForeign; //外語輸入域
    public GameObject inputFieldNative; //母語輸入域
    
    //群組
    [Header("群組")]
    public GameObject newGroup; //新增群組頁面
    public GameObject inputFieldGroup; //輸入名字區域
    public GameObject groupParent; //父群組
    public GameObject groupObject; //群組顯示物件
    public GameObject [] groupPos = new GameObject[7]; //群組生成位置
    public int nowExitGroup = 1;
    public GameObject newGroupButton; //建立群組按鈕
    public GameObject groupPage; //群組頁面
    public Text groupPageName; //群組名
    public GameObject groupPageDropDown; //群組左邊的滑條
    public int index; //現在鎖定的群組
    public GameObject homePageDropDown; //主頁面的滑條


    [Header("現有群組")]
    public GameObject [] group = new GameObject[1]; //存在倉庫頁面的群組物件

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

    // Start is called before the first frame update
    void Start()
    {
        int exit = 0;
        exit = StoreSystem.instance.Load();

        //如果沒有存檔，加載原始項目
        if(exit == 0)
        {
            //轉成List添加群組
            List<Struct.groupMode> tem = new List<Struct.groupMode>(Store.instance.group);
            Struct.groupMode groupMode;
            groupMode.name = "英文";
            groupMode.wordNum = 0;
            groupMode.correct = 0;
            groupMode.error = 0;
            groupMode.ratio = 0f;
            groupMode.word = new Struct.wordMode[0];
            tem.Add(groupMode);

            //轉回去
            Store.instance.group = tem.ToArray();

            //群組初始化(名字、單字數、答對率滑條、答對%數)
            group[0].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[0].name;
            group[0].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[0].wordNum.ToString();
            group[0].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[0].ratio;
            group[0].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[0].ratio * 100).ToString("0") + "%";

            //Store.instance.player.name = "";
            Store.instance.player.classRank = "初學者";
            Store.instance.player.lv = 0;
            Store.instance.player.experienceValue = 0;
        }
    }

    //開啟和關閉生成群組頁面
    public void OpenNewGroup()
    {
        newGroup.SetActive(true);
    }
    public void CloseNewGroup()
    {
        newGroup.SetActive(false);
    }

    //開啟和關閉現有群組
    public void OpenGroup()
    {
        //鎖定是哪一個群組(按鈕接收物件、循環檢索是否相同)
        GameObject tem = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for(int i=0;i<group.Length;i++)
        {
            if (tem == group[i])
            {
                index = i;
                break;
            }
        }

        //開啟頁面(群組頁面打開、底下目錄關閉、倉庫頁面關閉)
        groupPage.SetActive(true);
        catalog.SetActive(false);
        wareHouse.SetActive(false);

        //初始化
        WordShow.instance.mode = 0; //向上滑或向下滑的單字生成模式
        WordShow.instance.first = 0; //初始化單字生成座標的索引次數
        WordShow.instance.setWordMode = 0; //重製搜尋模式向上或向下
        groupPageName.text = Store.instance.group[index].name; //給予群組名字
        WordShow.instance.noeExitWord = 0; //刷新單字生成
        WordShow.instance.content.GetComponent<RectTransform>().sizeDelta = new Vector2(WordShow.instance.area.x, WordShow.instance.area.y); //刷新包括區域(似乎沒用，但先保留著)
        WordShow.instance.groupIndex = index; //設定進入群組編號
        WordShow.instance.wordNum = Store.instance.group[index].wordNum; //給予進入群組內單字數量
        if(Store.instance.group[index].word.Length > 0) WordShow.instance.CreateWord(); //如果可以生成單字，優先生成一個

        //填充左邊滑條(單字編號)
        for (int i=0;i<Store.instance.group[index].wordNum;i++)
        {
            Dropdown.OptionData optionTem = new Dropdown.OptionData();
            optionTem.text = (i + 1).ToString();
            groupPageDropDown.GetComponent<Dropdown>().options.Add(optionTem);
        }
    }
    public void CloseGroup()
    {
        groupPage.SetActive(false);
        catalog.SetActive(true);
        wareHouse.SetActive(true);

        //清空
        groupPageDropDown.GetComponent<Dropdown>().ClearOptions(); //清空左邊滑條
        WordShow.instance.word = new GameObject[0]; //單字數組清空
        WordShow.instance.nowWord = 0; //將已建好的單字設為0
        WordShow.instance.wordNum = 0; //將單字總數設為0
        WordShow.instance.content.GetComponent<RectTransform>().position = new Vector2(WordShow.instance.content.GetComponent<RectTransform>().position.x, WordShow.instance.posContent.y); //更新滑屏位置

        //摧毀殘留單字
        for (int i=1;i<(WordShow.instance.wordParent.transform.childCount - 1);i++)
        {
            Destroy(WordShow.instance.wordParent.transform.GetChild(i).gameObject);
        }
    }

    //儲存新群組
    public void StoreGroup()
    {
        InputField input = inputFieldGroup.GetComponent<InputField>();
        bool legalName = true;
        int newGroupIndex = Store.instance.group.Length;

        //檢索是否重名
        for (int i=0;i<Store.instance.group.Length;i++)
        {
            if (input.text == Store.instance.group[i].name) legalName = false;
        }

        if (legalName && Store.instance.group.Length < 8)
        {
            string groupName = input.text;

            //轉成List添加群組
            List<Struct.groupMode> tem = new List<Struct.groupMode>(Store.instance.group);
            Struct.groupMode groupMode;
            groupMode.name = groupName;
            groupMode.wordNum = 0;
            groupMode.correct = 0;
            groupMode.error = 0;
            groupMode.ratio = 0f;
            groupMode.word = new Struct.wordMode[0];
            tem.Add(groupMode);

            //轉回去
            Store.instance.group = tem.ToArray();

            Instantiate(groupObject, groupPos[nowExitGroup - 1].transform.position, groupObject.transform.rotation, groupParent.transform);            
            if(nowExitGroup <= 6) newGroupButton.transform.position = new Vector2(groupPos[nowExitGroup].transform.position.x, groupPos[nowExitGroup].transform.position.y);
            nowExitGroup++;

            //將新增群組物件放入數組
            List<GameObject> tem1 = new List<GameObject>(group);
            tem1.Add(groupParent.transform.GetChild(newGroupIndex).gameObject);
            group = tem1.ToArray();

            //群組初始化
            group[newGroupIndex].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[newGroupIndex].name;
            group[newGroupIndex].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[newGroupIndex].wordNum.ToString();
            group[newGroupIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[newGroupIndex].ratio;
            group[newGroupIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[newGroupIndex].ratio * 100).ToString("0") + "%";
            group[newGroupIndex].GetComponent<Button>().onClick.AddListener(OpenGroup); //添加按鈕事件

            //更新滑條(群組名稱)
            homePageDropDown.GetComponent<Dropdown>().options.Clear();
            for (int i = 0; i < Store.instance.group.Length; i++)
            {
                Dropdown.OptionData optionTem = new Dropdown.OptionData();
                optionTem.text = Store.instance.group[i].name;
                homePageDropDown.GetComponent<Dropdown>().options.Add(optionTem);
            }

            CloseNewGroup();
            StoreSystem.instance.Save();
        }
        else if (!legalName) WaringSystem.instance.Show("發現同樣名字的群組!!!");
        else if(Store.instance.group.Length >= 8) WaringSystem.instance.Show("總群組數已經達到6個!!!");
    }

    //儲存新單字
    public void StoreWord()
    {
        InputField foreign = inputFieldForeign.GetComponent<InputField>();
        InputField native = inputFieldNative.GetComponent<InputField>();
        int storeIndex = homePageDropDown.GetComponent<Dropdown>().value;
        bool legalName = true;

        for(int i=0;i<Store.instance.group[storeIndex].word.Length;i++)
        {
            if (foreign.text == Store.instance.group[storeIndex].word[i].foreign && native.text == Store.instance.group[storeIndex].word[i].native) legalName = false;
        }

        if (foreign.text != null && native.text != null && legalName)
        {
            string foreignWord = foreign.text;
            string nativeWord = native.text;

            //清空輸入欄
            inputFieldForeign.GetComponent<InputField>().text = null;
            inputFieldNative.GetComponent<InputField>().text = null;

            //轉成List添加單字
            List<Struct.wordMode> tem = new List<Struct.wordMode>(Store.instance.group[storeIndex].word);
            Struct.wordMode word;
            word.foreign = foreignWord;
            word.native = nativeWord;
            word.correct = 0;
            word.error = 0;
            word.ratio = 0f;
            tem.Add(word);

            //轉回去
            Store.instance.group[storeIndex].word = tem.ToArray();

            //群組資料重新整理
            Store.instance.group[storeIndex].wordNum += 1;
            group[storeIndex].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[storeIndex].name;
            group[storeIndex].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[storeIndex].wordNum.ToString();
            group[storeIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[storeIndex].ratio;
            group[storeIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[storeIndex].ratio * 100).ToString("0") + "%";

            Store.instance.group[storeIndex].Count();
            BasicSystem.instance.group[storeIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[storeIndex].ratio;
            BasicSystem.instance.group[storeIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[storeIndex].ratio * 100).ToString("0") + "%";

            //Debug.Log(Store.instance.group[0].wordNum + " " + Store.instance.group[0].word.Length);
            //Debug.Log(Store.instance.group[0].word[0].foreign);

            StoreSystem.instance.Save();
        }
        else if (!legalName) WaringSystem.instance.Show("發現同樣內容的單詞!!!");
    }

    //載入群組
    public void LoadStoreGroup()
    {
        //InputField input = inputFieldGroup.GetComponent<InputField>();
        //bool legalName = true;
        int newGroupIndex = group.Length;
        //Debug.Log(newGroupIndex);

        /*//檢索是否重名
        for (int i = 0; i < Store.instance.group.Length; i++)
        {
            if (input.text == Store.instance.group[i].name) legalName = false;
        }*/

        group[0].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[0].name;
        group[0].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[0].wordNum.ToString();
        group[0].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[0].ratio;
        group[0].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[0].ratio * 100).ToString("0") + "%";
        //Debug.Log(Store.instance.group.Length);

        if (/*legalName &&*/ group.Length < 8 && Store.instance.group.Length > 1)
        {
            //string groupName = input.text;

            /*//轉成List添加群組
            List<Store.groupMode> tem = new List<Store.groupMode>(Store.instance.group);
            Store.groupMode groupMode;
            groupMode.name = groupName;
            groupMode.wordNum = 0;
            groupMode.correct = 0;
            groupMode.error = 0;
            groupMode.ratio = 0f;
            groupMode.word = new Store.wordMode[0];
            tem.Add(groupMode);

            //轉回去
            Store.instance.group = tem.ToArray();*/

            Instantiate(groupObject, groupPos[nowExitGroup - 1].transform.position, groupObject.transform.rotation, groupParent.transform);
            if (nowExitGroup <= 6) newGroupButton.transform.position = new Vector2(groupPos[nowExitGroup].transform.position.x, groupPos[nowExitGroup].transform.position.y);
            nowExitGroup++;

            //將新增群組物件放入數組
            List<GameObject> tem1 = new List<GameObject>(group);
            tem1.Add(groupParent.transform.GetChild(newGroupIndex).gameObject);
            group = tem1.ToArray();

            //群組初始化
            group[newGroupIndex].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[newGroupIndex].name;
            group[newGroupIndex].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[newGroupIndex].wordNum.ToString();
            group[newGroupIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[newGroupIndex].ratio;
            group[newGroupIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[newGroupIndex].ratio * 100).ToString("0") + "%";
            group[newGroupIndex].GetComponent<Button>().onClick.AddListener(OpenGroup); //添加按鈕事件
            Debug.Log(Store.instance.group.Length);
            Debug.Log(Store.instance.group[0].name);
            Debug.Log(Store.instance.group[1].name);

            //更新滑條(群組名稱)
            homePageDropDown.GetComponent<Dropdown>().options.Clear();
            for (int i = 0; i < Store.instance.group.Length; i++)
            {
                Dropdown.OptionData optionTem = new Dropdown.OptionData();
                optionTem.text = Store.instance.group[i].name;
                homePageDropDown.GetComponent<Dropdown>().options.Add(optionTem);
            }
        }
    }
}

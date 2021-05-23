using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSystem : MonoBehaviour
{
    public static TestSystem instance;

    //測試
    [Header("測試")]
    public Dropdown groupTest;
    public InputField wordNumTest;
    public Button foreignToNative, nativeToForeign;
    public GameObject isTest;
    public int lockGroupIndex = 0, mode = 0, testWordNum = 0;
    public Struct.wordMode[] alreadyChoose = new Struct.wordMode[0];

    //頁面
    [Header("頁面")]
    public GameObject home;
    public GameObject catalog;
    public GameObject test;
    public GameObject end;

    //正式測試
    [Header("正式測試")]
    public Text question;
    public Text o, x, notyet;
    public int oNum = 0, xNum = 0, notyetNum = 0;
    public Text [] choose = new Text[4];
    public string answer;
    public int answerNum = 4;
    public bool testing = false;
    int chooseWordIndex = 0;

    //結算頁面
    public Text groupPageName; //群組名
    public GameObject groupPageDropDown; //群組左邊的滑條

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
        //事件監聽
        groupTest.onValueChanged.AddListener(LockGroup);
        wordNumTest.onEndEdit.AddListener(GetWordNum);
    }

    private void Update()
    {
        //是否在測試中
        if (testing) TestJudge();
    }

    //開關測試頁面
    public void OpenIsTest()
    {
        //重置
        isTest.SetActive(true);
        mode = 0;

        //更新滑條(測試中群組選項)
        groupTest.options.Clear();
        for (int i = 0; i < Store.instance.group.Length; i++)
        {
            Dropdown.OptionData optionTem = new Dropdown.OptionData();
            optionTem.text = Store.instance.group[i].name;
            groupTest.options.Add(optionTem);
        }
    }
    public void CloseIsTest()
    {
        isTest.SetActive(false);
    }

    //鎖定頁面編號
    public void LockGroup(int index)
    {
        lockGroupIndex = index;
        //Debug.Log(index);
    }

    //模式選擇
    public void ForeignToNative()
    {
        mode = 1; //外與對母語
    }
    public void NativeToForeign()
    {
        mode = 2; //母語對外語
    }

    //得到想測試的單字總數
    public void GetWordNum(string num)
    {
        testWordNum = int.Parse(num);
    }

    //開始測試
    public void StartTest()
    {
        //如果想測試的單字數大於3，群組長度大於想測試的單字數，模式不為零
        if (testWordNum > 3 && Store.instance.group[lockGroupIndex].word.Length >= testWordNum && mode != 0)
        {
            //開啟頁面
            home.SetActive(false);
            catalog.SetActive(false);
            test.SetActive(true);

            //更新上方顯示
            notyetNum = testWordNum;
            oNum = 0;
            xNum = 0;
            chooseWordIndex = 0;

            o.text = "O:" + oNum.ToString();
            x.text = "X:" + xNum.ToString();
            notyet.text = "?:" + notyetNum.ToString();

            //將儲存的單字複製一份
            Struct.wordMode[] question = Store.instance.group[lockGroupIndex].word;

            //轉成List添加單字
            List<Struct.wordMode> tem1 = new List<Struct.wordMode>(alreadyChoose);

            for (int i = 0; i < testWordNum; i++)
            {
                List<Struct.wordMode> tem = new List<Struct.wordMode>(question);

                int indexWord = 0;
                indexWord = Random.Range(0, question.Length - 1);

                tem1.Add(tem[indexWord]);
                tem.RemoveAt(indexWord);
                question = tem.ToArray();
            }

            //轉回去
            alreadyChoose = tem1.ToArray();
            testing = true;
        }
        else if (testWordNum <= 3) WaringSystem.instance.Show("測試單字數必須超過3個!!!");
        else if (Store.instance.group[lockGroupIndex].word.Length < testWordNum) WaringSystem.instance.Show("測試單字數超過群組單字數!!!");
        else if (mode == 0) WaringSystem.instance.Show("請選擇模式!!!");
    }

    //測試裁判
    public void TestJudge()
    {
        if(mode == 1 && notyetNum > 0)
        {
            //先將問題以及答案設定好
            question.text = alreadyChoose[chooseWordIndex].foreign;
            answer = alreadyChoose[chooseWordIndex].native;

            //隨機生成答案所在按鈕
            int indexWord = 0;
            indexWord = Random.Range(0, answerNum);

            choose[indexWord].text = answer;

            //隨機填充其他按鈕
            for (int j = 0; j <= answerNum; j++)
            {
                if (j != indexWord)
                {
                    choose[j].text = alreadyChoose[Random.Range(0, alreadyChoose.Length - 1)].native;
                    //Debug.Log(j);
                }
            }

            chooseWordIndex += 1;
            testing = false;
        }
        else if(mode == 2 && notyetNum > 0)
        {
            question.text = alreadyChoose[chooseWordIndex].native;
            answer = alreadyChoose[chooseWordIndex].foreign;

            int indexWord = 0;
            indexWord = Random.Range(0, answerNum);

            choose[indexWord].text = answer;

            for (int j = 0; j <= answerNum; j++)
            {
                if (j != indexWord)
                {
                    choose[j].text = alreadyChoose[Random.Range(0, alreadyChoose.Length - 1)].foreign;
                    //Debug.Log(j);
                }
            }

            chooseWordIndex += 1;
            testing = false;
        }
        else if(notyetNum <= 0) //結束
        {
            testing = false;
            test.SetActive(false);

            //開啟頁面(群組頁面打開、底下目錄關閉、倉庫頁面關閉)
            end.SetActive(true);

            //初始化
            EndSystem.instance.mode = 0; //向上滑或向下滑的單字生成模式
            EndSystem.instance.first = 0; //初始化單字生成座標的索引次數
            EndSystem.instance.setWordMode = 0; //重製搜尋模式向上或向下
            groupPageName.text = Store.instance.group[lockGroupIndex].name; //給予群組名字
            EndSystem.instance.nowExitWord = 0; //刷新單字生成索引
            EndSystem.instance.content.GetComponent<RectTransform>().sizeDelta = new Vector2(EndSystem.instance.area.x, EndSystem.instance.area.y); //刷新包括區域(似乎沒用，但先保留著)
            EndSystem.instance.groupIndex = lockGroupIndex; //設定進入群組編號
            EndSystem.instance.wordNum = alreadyChoose.Length; //給予進入群組內單字數量
            if (alreadyChoose.Length > 0) EndSystem.instance.CreateWord(); //如果可以生成單字，優先生成一個

            //填充左邊滑條(單字編號)
            for (int i = 0; i < alreadyChoose.Length; i++)
            {
                Dropdown.OptionData optionTem = new Dropdown.OptionData();
                optionTem.text = (i + 1).ToString();
                groupPageDropDown.GetComponent<Dropdown>().options.Add(optionTem);
            }
        }
    }

    //答案選擇
    public void ChooseAnswer(int index)
    {
        //獲得按下的按鈕
        GameObject tem = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (tem.transform.GetChild(0).GetComponent<Text>().text == answer)
        {
            oNum += 1;

            //標誌這個單字答對了
            alreadyChoose[chooseWordIndex - 1].correct = -1;
        }
        else
        {
            xNum += 1;
            alreadyChoose[chooseWordIndex - 1].error = -1;
        }

        //還未測驗的單字數減一
        notyetNum -= 1;

        o.text = "O:" + oNum.ToString();
        x.text = "X:" + xNum.ToString();
        notyet.text = "?:" + notyetNum.ToString();

        testing = true;

        EndSystem.instance.O.text = o.text;
        EndSystem.instance.X.text = x.text;
    }

    //結束
    public void CloseEndPage()
    {
        home.SetActive(true);
        catalog.SetActive(true);
        end.SetActive(false);
        List<Struct.wordMode> tem1 = new List<Struct.wordMode>(alreadyChoose);
        tem1.Clear();
        alreadyChoose = tem1.ToArray();

        //清空
        groupPageDropDown.GetComponent<Dropdown>().ClearOptions(); //清空左邊滑條
        EndSystem.instance.word = new GameObject[0]; //單字數組清空
        EndSystem.instance.nowWord = 0; //將已建好的單字設為0
        EndSystem.instance.wordNum = 0; //將單字總數設為0
        EndSystem.instance.content.GetComponent<RectTransform>().position = new Vector2(EndSystem.instance.content.GetComponent<RectTransform>().position.x, EndSystem.instance.posContent.y); //更新滑屏位置

        //摧毀殘留單字
        for (int i = 1; i < EndSystem.instance.wordParent.transform.childCount; i++)
        {
            Destroy(EndSystem.instance.wordParent.transform.GetChild(i).gameObject);
        }
        
        Store.instance.player.CountLV();
        Store.instance.player.CountLvValue();
        Store.instance.player.LvUpCheck();
        Store.instance.group[lockGroupIndex].Count();
        BasicSystem.instance.group[lockGroupIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[lockGroupIndex].ratio;
        BasicSystem.instance.group[lockGroupIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[lockGroupIndex].ratio * 100).ToString("0") + "%";

        StoreSystem.instance.Save();
    }
}

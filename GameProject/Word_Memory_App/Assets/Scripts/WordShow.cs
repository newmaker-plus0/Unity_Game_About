using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordShow : MonoBehaviour
{
    public static WordShow instance;

    //雜項
    [Header("雜項")]
    public int first = 0; //單字生成位置是否就位
    public int mode = 0; //生成單字模式
    public int setWordNum = -1; //鎖定單字編號
    public int setWordMode = 0; //檢索模式鎖定

    //滑動生成時間
    private float time;
    private float timeSet;

    //單字
    [Header("單字")]
    public GameObject wordModePrefers; //單字預製體
    public int wordNum = 0; //單字數量
    public GameObject [] wordPos = new GameObject[6]; //單字生成位置
    public int noeExitWord = 0;
    public int nowWord = 0; //下一個要生成的單字
    public GameObject wordParent; //父單字
    public Vector2 area, posContent; //初始位置
    public GameObject content; //滑屏的包括區域

    [Header("現有單字")]
    public GameObject[] word = new GameObject[0];

    //群組
    public int groupIndex = 0; //群組編號
    private GameObject groupPageDropDown; //群組左邊的滑條
    public InputField groupInputName; //群組名
    public Text get, show;

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
        //獲取值
        groupPageDropDown = BasicSystem.instance.groupPageDropDown;
        area = content.GetComponent<RectTransform>().sizeDelta;
        posContent = content.GetComponent<RectTransform>().position;
        timeSet = 0.5f;
        BasicSystem.instance.groupPageDropDown.GetComponent<Dropdown>().onValueChanged.AddListener(SetWord);
        groupInputName.onEndEdit.AddListener(SetName);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (wordNum > 0 && word.Length < 6 && Store.instance.group[groupIndex].word.Length > nowWord)
        {
            CreateWord();
        }

        //向下滑動，動態新增
        if(content.GetComponent<RectTransform>().position.y >= (posContent.y + 100f) && wordNum > 0 && Store.instance.group[groupIndex].word.Length > nowWord && time >= timeSet)
        {
            time = 0f;
            mode = 0;
            //Debug.Log(content.GetComponent<RectTransform>().position.y + "下方" + wordPos[2].transform.position.y);

            //刪除物件
            Destroy(word[0].gameObject);
            
            //轉成List刪除
            List<GameObject> tem = new List<GameObject>(word);
            tem.RemoveAt(0);

            //轉回去
            word = tem.ToArray();
            //Debug.Log(word.Length);
            for (int i=0;i<word.Length;i++)
            {
                //Debug.Log(i);
                word[i].gameObject.transform.position = new Vector2(wordPos[i].transform.position.x, wordPos[i].transform.position.y);
            }
        }
        //向上滑動，動態新增
        if (content.GetComponent<RectTransform>().position.y <= (posContent.y - 100f) && wordNum > 0 && (nowWord - 6) > 0 && time >= timeSet && nowWord > 6)
        {
            time = 0;
            mode = 1;
            //Debug.Log(content.GetComponent<RectTransform>().position.y + "上方");

            //刪除物件
            Destroy(word[5].gameObject);

            //轉成List刪除
            List<GameObject> tem = new List<GameObject>(word);
            tem.RemoveAt(5);

            //轉回去
            word = tem.ToArray();

            for (int i=0;i<word.Length;i++)
            {
                word[i].gameObject.transform.position = new Vector2(wordPos[i + 1].transform.position.x, wordPos[i + 1].transform.position.y);
            }

            nowWord -= 7;
        }

        //鎖定單字
        if(setWordNum != -1)
        {
            if (setWordNum >= wordNum)
            {
                setWordNum = -1;
            }else
            {
                if (setWordNum > nowWord && setWordMode != 2)
                {
                    setWordMode = 1;
                    content.GetComponent<RectTransform>().position = new Vector2(content.GetComponent<RectTransform>().position.x, (posContent.y + 120f));
                }
                else if (setWordNum <= nowWord && setWordMode != 1)
                {
                    setWordMode = 2;
                    content.GetComponent<RectTransform>().position = new Vector2(content.GetComponent<RectTransform>().position.x, (posContent.y - 120f));
                }

                timeSet = 0.0005f;

                for (int i = 0; i < word.Length; i++)
                {
                    if (Store.instance.group[groupIndex].word[setWordNum].foreign == word[i].transform.GetChild(0).GetComponent<Text>().text
                        && Store.instance.group[groupIndex].word[setWordNum].native == word[i].transform.GetChild(1).GetComponent<Text>().text)
                    {
                        setWordNum = -1;
                        timeSet = 0.5f;
                        setWordMode = 0;
                        content.GetComponent<RectTransform>().position = new Vector2(content.GetComponent<RectTransform>().position.x, posContent.y);
                        break;
                    }
                }
            }
        }
    }

    //生成單字
    public void CreateWord()
    {
        int newWordIndex = word.Length + 1;

        GameObject wordMode = wordModePrefers;

        //初始化
        wordMode.transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[groupIndex].word[nowWord].foreign;
        wordMode.transform.GetChild(1).GetComponent<Text>().text = Store.instance.group[groupIndex].word[nowWord].native;
        wordMode.transform.GetChild(3).GetComponent<Slider>().value = Store.instance.group[groupIndex].word[nowWord].ratio;
        wordMode.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = (Store.instance.group[groupIndex].word[nowWord].ratio * 100).ToString("0") + "%";

        //Debug.Log(Store.instance.group[groupIndex].word[nowWord].foreign + " " + Store.instance.group[groupIndex].word[nowWord].native);

        if(mode == 0)
        {
            Instantiate(wordMode, wordPos[noeExitWord].transform.position, wordMode.transform.rotation, wordParent.transform);

            //轉成List添加單字
            List<GameObject> tem = new List<GameObject>(word);
            GameObject wordTem;
            wordTem = wordParent.transform.GetChild(newWordIndex).gameObject;
            tem.Add(wordTem);

            //轉回去
            word = tem.ToArray();
            
            //添加按鈕事件
            word[word.Length - 1].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(DeleteWord);

            nowWord += 1;

            if (nowWord < 6 && first < 5)
            {
                noeExitWord += 1;
                first += 1;
            }
        }
        else if(mode == 1)
        {
            Instantiate(wordMode, wordPos[0].transform.position, wordMode.transform.rotation, wordParent.transform);

            //轉成List添加單字
            List<GameObject> tem = new List<GameObject>(word);
            GameObject wordTem;
            wordTem = wordParent.transform.GetChild(newWordIndex).gameObject;
            tem.Insert(0, wordTem);

            //轉回去
            word = tem.ToArray();

            //添加按鈕事件
            word[0].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(DeleteWord);

            nowWord += 6;
        }
        
        /*最初的想法，增加包括區域大小，瘋狂增加單字物件(手機應該會爆掉)
        if(content.GetComponent<RectTransform>().sizeDelta.y < word.Length * 200f)
        {
            Debug.Log("ok");
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetComponent<RectTransform>().sizeDelta.y + 220f);
        }*/
    }

    //刪除單字
    public void DeleteWord()
    {
        mode = 0;
        GameObject temWord = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        int index = -1;
        for (int i=0;i<word.Length;i++)
        {
            if(temWord.transform.parent == word[i].transform)
            {
                index = i;
                //Debug.Log("刪除的物件索引值為:" + index);
                break;
            }
        }

        word[index].transform.GetChild(4).gameObject.SetActive(true);

        //轉成List刪除(真正儲存的單字)
        List<Struct.wordMode> temWordMode = new List<Struct.wordMode>(Store.instance.group[groupIndex].word);
        for(int i=0;i<Store.instance.group[groupIndex].word.Length;i++)
        {
            if(word[index].transform.GetChild(0).GetComponent<Text>().text == Store.instance.group[groupIndex].word[i].foreign 
                && word[index].transform.GetChild(1).GetComponent<Text>().text == Store.instance.group[groupIndex].word[i].native)
            {
                index = i;
                break;
            }
        }
        temWordMode.RemoveAt(index);

        Store.instance.group[groupIndex].word = temWordMode.ToArray();

        /*for (int i = 0; i < Store.instance.group[groupIndex].word.Length; i++)
        {
            Debug.Log(Store.instance.group[groupIndex].word.Length);
        }*/

        //群組資料重新整理
        Store.instance.group[groupIndex].wordNum -= 1;
        BasicSystem.instance.group[groupIndex].transform.GetChild(0).GetComponent<Text>().text = Store.instance.group[groupIndex].name;
        BasicSystem.instance.group[groupIndex].transform.GetChild(1).GetComponent<Text>().text = "單字數:" + Store.instance.group[groupIndex].wordNum.ToString();
        BasicSystem.instance.group[groupIndex].transform.GetChild(2).GetComponent<Slider>().value = Store.instance.group[groupIndex].ratio;
        BasicSystem.instance.group[groupIndex].transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = (Store.instance.group[groupIndex].ratio * 100).ToString("0") + "%";

        nowWord--;
        wordNum--;

        //更新左邊滑條(單字編號)
        groupPageDropDown.GetComponent<Dropdown>().options.Clear();
        for (int i = 0; i < Store.instance.group[groupIndex].word.Length; i++)
        {
            Dropdown.OptionData optionTem = new Dropdown.OptionData();
            optionTem.text = (i + 1).ToString();
            groupPageDropDown.GetComponent<Dropdown>().options.Add(optionTem);
        }

        StoreSystem.instance.Save();
    }

    //鎖定單字
    public void SetWord(int i)
    {
        setWordNum = i;
    }

    //重設群組名字
    public void SetName(string name)
    {
        Store.instance.group[groupIndex].name = name;
        get.text = "";
        show.text = Store.instance.group[groupIndex].name;

        StoreSystem.instance.Save();
        WaringSystem.instance.Show("更新的名字將在下次重啟應用程式後生效!!!");
    }
}

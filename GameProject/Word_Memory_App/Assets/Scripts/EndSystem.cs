using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSystem : MonoBehaviour
{
    public static EndSystem instance;

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
    public GameObject [] wordModePrefers = new GameObject[2]; //單字預製體
    public int wordNum = 0; //單字數量
    public GameObject [] wordPos = new GameObject[6]; //單字生成位置
    public int nowExitWord = 0; //生成單字索引
    public int nowWord = 0; //下一個要生成的單字
    public GameObject wordParent; //父單字
    public Vector2 area, posContent; //初始位置
    public GameObject content; //滑屏的包括區域
    public Text O, X; //顯示OX

    [Header("現有單字")]
    public GameObject[] word = new GameObject[0];

    //群組
    public int groupIndex = 0; //群組編號
    private GameObject groupPageDropDown; //群組左邊的滑條

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
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (wordNum > 0 && word.Length < 6 && TestSystem.instance.alreadyChoose.Length > nowWord)
        {
            CreateWord();
        }

        //向下滑動，動態新增
        if (content.GetComponent<RectTransform>().position.y >= (posContent.y + 100f) && wordNum > 0 && TestSystem.instance.alreadyChoose.Length > nowWord && time >= timeSet)
        {
            time = 0f;
            mode = 0;
            //Debug.Log(content.GetComponent<RectTransform>().position.y + "下方");

            //刪除物件
            Destroy(word[0].gameObject);

            //轉成List刪除
            List<GameObject> tem = new List<GameObject>(word);
            tem.RemoveAt(0);

            //轉回去
            word = tem.ToArray();

            for (int i = 0; i < word.Length; i++)
            {
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

            for (int i = 0; i < word.Length; i++)
            {
                word[i].gameObject.transform.position = new Vector2(wordPos[i+1].transform.position.x, wordPos[i+1].transform.position.y);
            }

            nowWord -= 7;
        }

        //鎖定單字
        if (setWordNum != -1)
        {
            if (setWordNum >= wordNum)
            {
                setWordNum = -1;
            }
            else
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
                    if (TestSystem.instance.alreadyChoose[setWordNum].foreign == word[i].transform.GetChild(0).GetComponent<Text>().text
                        && TestSystem.instance.alreadyChoose[setWordNum].native == word[i].transform.GetChild(1).GetComponent<Text>().text)
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
        int status = -1, indexWord = 0;

        //選擇哪一種單字模板
        if (TestSystem.instance.alreadyChoose[nowWord].correct == -1) status = 0;
        else if (TestSystem.instance.alreadyChoose[nowWord].error == -1) status = 1;
        GameObject wordMode = wordModePrefers[status];

        //找出存檔的單字編號，更新單字資料
        for (int i = 0; i < Store.instance.group[groupIndex].word.Length; i++)
        {
            if (TestSystem.instance.alreadyChoose[nowWord].foreign == Store.instance.group[groupIndex].word[i].foreign 
                && TestSystem.instance.alreadyChoose[nowWord].native == Store.instance.group[groupIndex].word[i].native)
            {
                indexWord = i;
                break;
            }
        }

        if(status == 0)
        {
            Store.instance.group[groupIndex].word[indexWord].correct += 1;
            Store.instance.player.nowLvExprience += 1;
            Store.instance.player.point += 1;
        }
        else if (status == 1) Store.instance.group[groupIndex].word[indexWord].error += 1;

        Store.instance.group[groupIndex].word[indexWord].Count();

        //初始化
        wordMode.transform.GetChild(0).GetComponent<Text>().text = TestSystem.instance.alreadyChoose[nowWord].foreign;
        wordMode.transform.GetChild(1).GetComponent<Text>().text = TestSystem.instance.alreadyChoose[nowWord].native;
        wordMode.transform.GetChild(3).GetComponent<Slider>().value = TestSystem.instance.alreadyChoose[nowWord].ratio;
        wordMode.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = (TestSystem.instance.alreadyChoose[nowWord].ratio * 100).ToString("0");

        //Debug.Log(Store.instance.group[groupIndex].word[nowWord].foreign + " " + Store.instance.group[groupIndex].word[nowWord].native);

        if (mode == 0)
        {
            Instantiate(wordMode, wordPos[nowExitWord].transform.position, wordMode.transform.rotation, wordParent.transform);

            //轉成List添加單字
            List<GameObject> tem = new List<GameObject>(word);
            GameObject wordTem;
            wordTem = wordParent.transform.GetChild(newWordIndex).gameObject;
            tem.Add(wordTem);

            //轉回去
            word = tem.ToArray();

            nowWord += 1;

            if (nowWord < 6 && first < 5)
            {
                nowExitWord += 1;
                first += 1;
            }
        }
        else if (mode == 1)
        {
            Instantiate(wordMode, wordPos[0].transform.position, wordMode.transform.rotation, wordParent.transform);

            //轉成List添加單字
            List<GameObject> tem = new List<GameObject>(word);
            GameObject wordTem;
            wordTem = wordParent.transform.GetChild(newWordIndex).gameObject;
            tem.Insert(0, wordTem);

            //轉回去
            word = tem.ToArray();

            nowWord += 6;
        }

        /*最初的想法，增加包括區域大小，瘋狂增加單字物件(手機應該會爆掉)
        if(content.GetComponent<RectTransform>().sizeDelta.y < word.Length * 200f)
        {
            Debug.Log("ok");
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetComponent<RectTransform>().sizeDelta.y + 220f);
        }*/
    }

    //鎖定單字
    public void SetWord(int i)
    {
        setWordNum = i;
    }
}

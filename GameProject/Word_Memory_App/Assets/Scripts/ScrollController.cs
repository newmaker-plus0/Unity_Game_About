using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //獲取組件
    ScrollRect rect;
    private float[] posArray = new float[] { 0f, 0.25f, 0.5f, 0.75f, 1.0f};
    int pageNum = 5;
    int firstPage = 2;

    private float targetPos;
    //判斷是否拖動
    private bool isDrag = false;
    int index = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        Vector2 pos = rect.normalizedPosition;

        //檢測目前距離哪個頁面最近
        float x = Mathf.Abs(pos.x - posArray[0]);
        for(int i=0;i<pageNum;i++)
        {
            float temp = Mathf.Abs(pos.x - posArray[i]);
            if(temp <= x)
            {
                x = temp;
                index = i;
            }
        }
        targetPos = posArray[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<ScrollRect>();
        targetPos = posArray[firstPage];
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDrag)
        {
            //將頁面調整的目標位置
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targetPos, Time.deltaTime * 4);
        }
    }
}

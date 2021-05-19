using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadoeSprites : MonoBehaviour
{
    [Header("物件屬性")]
    private Transform player;

    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;

    private Color color;

    [Header("時間控制參數")]
    public float activeTime;//顯示時間
    public float activeStart;//開始顯示的時間點

    [Header("不透明度控制")]
    private float alpha;
    public float alphaSet;//初始值
    public float alphaMultiplier;

    private void OnEnable() //取出對象池
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;

        thisSprite.sprite = playerSprite.sprite;

        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        activeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        alpha = alphaMultiplier * alpha;

        color = new Color(0.5f, 0.5f, 1, alpha);

        thisSprite.color = color;

        if(Time.time >= activeStart + activeTime)
        {
            //返回對象池
            ShadowPool.instance.ReturnPool(this.gameObject);
        }
    }
}

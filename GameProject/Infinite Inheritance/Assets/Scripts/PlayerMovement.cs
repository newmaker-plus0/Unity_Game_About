using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("移動屬性")]
    private float moveH, moveV;
    [SerializeField] private float moveSpeed;
    static public float speedUp;
    static public float nowSpeed;

    [Header("CD的UI組件")]
    public Image cdImage;

    static public float dashTime = 0.1f;//dash時長
    private float dashTimeLeft;//衝鋒剩餘時間
    private float lastDash = -10f;//上一次dash時間點
    static public float dashCoolDown = 3;
    static public float dashSpeed = 10;

    static public float dashTimeUp;
    static public float dashSpeedUp;
    static public float dashCoolTimeDown;

    public bool isDashing;

    private float direction;

    Vector2 vector2 = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        Boss10_Rank1.set = 0;
        moveSpeed = Load01.speed;
        //moveSpeed = Load01.speed; 
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {       
        nowSpeed = moveSpeed;
        if (Boss10_Rank1.set == 0)
        {
            moveH = Input.GetAxis("Horizontal") * moveSpeed; 
            moveV = Input.GetAxis("Vertical") * moveSpeed;
            vector2 = this.transform.position;
        }else if(Boss10_Rank1.set == 1)
        {
            
            this.transform.position = new Vector3(vector2.x, vector2.y);
        }
        Flip();

        if (Input.GetMouseButtonDown(1))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            {
                //可以執行dash
                ReadyToDash();
            }
        }

        cdImage.fillAmount -= 1.0f / dashCoolDown * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveH, moveV); //移動
        Dash(); 
    }

    private void Flip() //圖片翻轉
    {
        //if (moveH > 0)
        if(transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = 1;
        }
        //if (moveH < 0)
        if (transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = -1;
        }
    }

    void ReadyToDash() //衝鋒許可
    {
        isDashing = true;

        dashTimeLeft = dashTime;

        lastDash = Time.time;

        cdImage.fillAmount = 1;
    }

    void Dash() //衝鋒
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                var directionY = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(dashSpeed * direction, dashSpeed * directionY);

                dashTimeLeft -= Time.deltaTime;

                ShadowPool.instance.GetFromPool();
            }
            else
            {
                isDashing = false;
            }
        }
    }

    public void SpeedUp() //移動速度增加
    {
        moveSpeed += speedUp;
    }

    public void DashTimeUp() //衝鋒時間增加
    {
        dashTime += dashTimeUp;
    }

    public void DashSpeedUp() //衝鋒速度增加
    {
        dashSpeed += dashSpeedUp;
    }

    public void DashCoolTimeDown() //衝鋒冷卻時間減少
    {
        dashCoolDown -= dashCoolTimeDown;
    }
}

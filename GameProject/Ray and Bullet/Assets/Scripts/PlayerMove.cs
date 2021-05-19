using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("基礎屬性")]
    private float moveH, moveV; //適用於XY軸的移動變量儲存
    [SerializeField] static public float moveSpeed=5f; //移動速度
    public float maxHp, hp, baseHp=100; //生命值(最大，現有)
    static public float hpStatic; //公開生命值
    
    private bool isAttacked = false; //是否被攻擊，以防被連續傷害

    [Header("血條動畫")]
    public Image hpImage; 
    public Image hpEffectImage;
    [SerializeField] private float hurtSpeed = 0.005f; //受傷效果，血條滑動時間

    public GameObject damageCanvas;

    [Header("受傷效果調整")]
    public float hurtLengh; //效果持續時間
    private float hurtCounter; //相當於計數器
    private SpriteRenderer sp;

    [Header("升級系統")]
    public float nextCurrentExp=10;
    static public int playerLevel=0;
    private float baseExp = 10;
    static public float currentExp = 0;
    public Image lvImage;
    public ParticleSystem lvEffect;

    // Start is called before the first frame update
    void Start()
    {
        isAttacked = false;
        hpStatic = Mathf.Round(Mathf.Pow(baseHp + (playerLevel*5), 2) / 100);
        maxHp = hpStatic;
        hp = hpStatic;
        nextCurrentExp = (int)Mathf.Pow(baseExp + (playerLevel / 2), 2) / 10;
        

        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        moveH = Input.GetAxis("Horizontal") * moveSpeed; 
        moveV = Input.GetAxis("Vertical") * moveSpeed;
        Flip();

        if (hp <= 0) //死亡
        {
            Destroy(this.gameObject);
            Weapon.bulletNowNum[0] = 100;
            ClassManager.levelNum--;
            SceneManager.LoadScene(0);
        }
        
        hpImage.fillAmount = hp / maxHp; //血量顯示
        lvImage.fillAmount = (currentExp + 1f) / (nextCurrentExp + 1f);

        if (hpEffectImage.fillAmount > hpImage.fillAmount) //血條緩衝掉血
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }
        

        if (hurtCounter <= 0) //受傷效果
        {
            sp.material.SetFloat("_FlashAmount", 0);
        }
        else
        {
            hurtCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveH, moveV); //移動
    }

    private void Flip() //圖片翻轉
    {
        //if (moveH > 0)
        if (transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);           
        }
        //if (moveH < 0)
        if (transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);          
        }
    }

    public void Hurt() //火元素射線檢測呼叫受傷
    {
        if (!isAttacked)
        {
            TakenDamagePlayer(FireElement.atkStatic);
        }
    }

    public void TakenDamagePlayer(float _amount) //玩家受到攻擊
    {
        isAttacked = true;
        HurtShader();
        hp -= _amount;
        StartCoroutine(isAttackCoPlayer());
    }

    IEnumerator isAttackCoPlayer() //防止玩家重複扣血
    {
        yield return new WaitForSeconds(0.1f);
        isAttacked = false;
    }

    private void HurtShader() //受傷效果
    {
        sp.material.SetFloat("_FlashAmount", 0.5f);
        hurtCounter = hurtLengh;
    }

    public void ExpCheck()
    {
        if(currentExp >= nextCurrentExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerLevel++;
        currentExp = 0;
        hpStatic = Mathf.Round(Mathf.Pow(baseHp + playerLevel, 2) / 100);
        maxHp = hpStatic;
        hp = hpStatic;
        nextCurrentExp = (int)Mathf.Pow(baseExp + playerLevel, 2) / 10;
        NowSkill.nowSkillPoint++;
        StartCoroutine(LevelUpEffectCo());
    }

    IEnumerator LevelUpEffectCo()
    {
        lvEffect.gameObject.SetActive(true);
        lvEffect.Play();
        yield return new WaitForSeconds(lvEffect.duration);
        lvEffect.gameObject.SetActive(false);
    }
}

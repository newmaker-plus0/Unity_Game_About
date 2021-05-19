using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FireElement : MonoBehaviour
{
    public Transform firePoint; //激光發射點
    public float maxDist; //最遠距離
    public LayerMask mask; //激光可檢測的圖層

    private Transform target; //激光目標

    [Header("雷射")]
    private LineRenderer lineRenderer; //激光渲染
    [SerializeField] private Gradient redColor, greenColor; //兩種顏色的激光
    public GameObject hitEffect; //爆炸效果
    bool shoot = true; //是否在攻擊中

    [Header("基礎屬性")]
    private float maxHp, baseHp=100; //最大生命
    static public float hpFireElementStatic; //公開生命
    public float hp; //現有生命
    public float atk, baseAtk; //現有攻擊
    static public float atkStatic; //公開攻擊
    public GameObject dead; //死亡效果
    public int exp; //給予的經驗值
    
    [Header("血條動畫")]
    public Image hpImage;
    public Image hpEffectImage;
    [SerializeField] private float hurtSpeed = 0.005f;

    [Header("隨機buff")]
    public int noImmunity = 0; //是否存在免疫狀態(0:無, 1:火力免疫, 2:激光免疫)
    private bool jumpSetNo = false; //次元跳躍封印
    private int sizeChange = 0; //更換大小
    public Image grade; //分級
    public float scale = 3f; //原始大小

    public GameObject damageCanvas;

    private void Start()
    {
        maxHp = Mathf.Round(Mathf.Pow(baseHp + ClassManager.levelNum, 2) / 100); //生命計算公式
        atk = Mathf.Round(Mathf.Pow(baseAtk + ClassManager.levelNum, 2) / 10); //攻擊力計算公式

        hp = maxHp; //更新現有生命值
        hpFireElementStatic = hp; //更新公開生命直
        atkStatic = atk; //更新公開攻擊力
        exp = ClassManager.levelNum + 50; //更新經驗值

        lineRenderer = GetComponentInChildren<LineRenderer>(); //獲取激光渲染器
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); //獲取遊戲中目標位置

        InvokeRepeating("Shoot", 3f, 5f); //每五秒重置，射擊兩秒，冷卻三秒
        InvokeRepeating("ShootClose", 0f, 5f);

        Physics2D.queriesStartInColliders = false; //激光不檢測自身

        int buffNum = Random.Range(1, 5);
        RandomBuff(buffNum); //隨機buff

        this.gameObject.transform.localScale = new Vector3(scale, scale, this.transform.localScale.z);

        //分級
        switch (buffNum)
        {
            case 1:
                grade.color = Color.green;
                break;
            case 2:
                grade.color = Color.blue;
                break;
            case 3:
                grade.color = Color.red;
                break;
            case 4:
                grade.color = Color.yellow;
                break;
            case 5:
                grade.color = Color.black;
                break;
            default:
                grade.color = Color.white;
                break;
        }
    }

    private void Update()
    {
        Detect();

        if(hp <= 0) //死亡
        {
            EnemyAppearManager.nowEnemyCountStatic--; //現有怪物數量減一
            Instantiate(dead, transform.position, Quaternion.identity);

            PlayerMove.currentExp += exp; //獲得經驗值
            GameObject.Find("Player").SendMessage("ExpCheck");

            //如果存在次元封印效果，解鎖
            if (jumpSetNo) 
                Weapon.jumpNo--;

            GiveBullet(); //獲得子彈
            Destroy(this.gameObject.transform.parent.gameObject);
            
        }

        hpImage.fillAmount = hp / maxHp; //血量顯示

        if (hpEffectImage.fillAmount > hpImage.fillAmount) //血條緩衝掉血
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }
        
        //WeaponRotation();
    }
    
    public void Shoot() //開啟攻擊
    {
        this.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        shoot = true;
    }

    public void ShootClose() //關閉攻擊
    {
        shoot = false;
        this.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Detect()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, target.position - this.transform.GetChild(0).position, maxDist); //獲取射線命中物
           
        if(hitInfo.collider != null && shoot == true) //開啟射擊模式，且目標不為空
        {
            if(hitInfo.collider.tag == "Wall")
            {
                //Debug.DrawLine(firePoint.position, hitInfo.point, Color.green);

                lineRenderer.SetPosition(1, hitInfo.point);
                lineRenderer.colorGradient = greenColor;
            }
            if (hitInfo.collider.tag == "Player")
            {
                //Debug.DrawLine(firePoint.position, hitInfo.point, Color.red);

                lineRenderer.SetPosition(1, hitInfo.point);
                lineRenderer.colorGradient = redColor;

                GameObject.Find("Player").SendMessage("Hurt"); //命中玩家
            }

            //Instantiate(hitEffect, hitInfo.point, Quaternion.identity); //生成爆炸效果
            lineRenderer.SetPosition(0, firePoint.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet" && noImmunity != 1) //被子彈命中
        {
            float attackDamage = Bullet.atackDamageStatic;
            TakenDamage(attackDamage);
        }
        if (other.gameObject.tag == "TrackerBullet" && noImmunity != 2) //被激光子彈命中
        {
            float attackDamage = Bullet.atackDamageStatic;
            TakenDamage(attackDamage);
        }
        if(other.gameObject.tag == "Slash") //被手刀命中
        {
            TakenDamage(10f);
        }

    }

    public void TakenDamage(float _amount) //怪物受到攻擊
    {
        bool isCrit = false;
        if (Weapon.critChance > 0)
        {
            float chance = Random.Range(0, 10);
            if (Weapon.critChance > chance)
            {
                isCrit = true;
            }
        }

        if (isCrit)
            hp -= _amount * Weapon.critDamage;
        else
            hp -= _amount;
    }

    public void GiveBullet()
    {
        int bulletNum = (int)Random.Range(1, ClassManager.levelNum);
        int whichGun = (int)Random.Range(0, 6);

        if(whichGun == 0) //步槍
            Weapon.bulletNowNum[whichGun] += bulletNum * 25;
        else if (whichGun == 2) //散彈槍
            Weapon.bulletNowNum[whichGun] += bulletNum * 50;
        else if (whichGun == 3) //加特林
            Weapon.bulletNowNum[whichGun] += bulletNum * 150;
        else if (whichGun == 5) //狙擊槍
            Weapon.bulletNowNum[whichGun] += bulletNum * 3;
    }

    public void RandomBuff(int num)
    {
        //隨機buff
        for(int i=0;i<num;i++)
        {
            int chance = Random.Range(1, 100);
            
            //火力免疫
            if(1 <= chance && chance <= 8 && noImmunity == 0)
            {
                noImmunity = 1;
                //Debug.Log("火力免疫");
            }
            //激光免疫
            else if(9 <= chance && chance <= 32 && noImmunity == 0) 
            {
                noImmunity = 2;
                //Debug.Log("激光免疫");
            }
            //次元封印
            else if(33 <= chance && chance <= 36)
            {
                Weapon.jumpNo++;
                jumpSetNo = true;
                //Debug.Log("次元封印");
            }
            //hp倍化
            else if(37 <= chance && chance <= 72)
            {
                maxHp *= 2;
                hp = maxHp; //更新現有生命值
                hpFireElementStatic = hp; //更新公開生命直
                //Debug.Log("hp倍化" + hp);
            }
            //巨大化
            else if (73 <= chance && chance <= 90 && sizeChange != 2)
            {
                scale *= 1.5f;
                sizeChange = 1;
                //Debug.Log("巨大化" + scale);
            }
            //縮小化
            else if(91 <= chance && chance <= 100 && sizeChange != 1)
            {
                scale *= 0.8f;
                sizeChange = 2;
                //Debug.Log("縮小化" + scale);
            }
            //無
            else
            {
                //Debug.Log("無");
                continue;
            }
        }
    }
}

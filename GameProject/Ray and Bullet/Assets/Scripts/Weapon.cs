using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public GameObject slashGameObject;

    [Header("武器旋轉")]
    private Vector2 direction; //方向儲存
    public Transform firePoint; //攻擊點

    [Header("敵人資訊介紹")]
    public Image boardImage;
    public Text boardText;
    [SerializeField] private float maxDist=20f; //最大射程
    public LayerMask mask; //可檢測圖層
    public Text atk;
    public Text hp;
    public Text lv;

    [Header("子彈")]
    static public bool bulletBaseGive = true; 
    [SerializeField] private GameObject bulletBase; //子彈物件
    public float force; 
    public float timeSet; //發射間隔
    private float time = 0.01f;
    static public GameObject bulletArm; //各種子彈
    public int onceBulletNum; //一次發射的子彈數量
    private int nowUseBullet; //現在使用的子彈
    static public int[] bulletNowNum = new int[7]; //現有子彈數量
    public GameObject[] bulletObject = new GameObject[7]; //使用子彈圖片
    private int bulletMode = 0; //預設為火力武器(0)，激光武器(1)，次元武器(2)

    [Header("追蹤彈")]
    public int alreadyShootBullet; //已經發射的子彈
    private int trackerBulletMode = 0; //預設追蹤彈模式為單發 - 三連發 - 五連發
    private float timeStartTracker; //開始計時累積時間
    private float timeTracker = 0.01f;
    public float timeSetTracker; //子彈累積傷害時間
    static public float maxTrackerEnergy = 500, nowTrackerEnergy = 500; //追蹤彈能量
    private float reEnergyTime; //回複能量時間點
    public float reEnergyTimeSet; //回複能量間隔
    public Image energyFillAmount; //能量現存圖案
    public GameObject energyFillGameObject; //能量現存物件

    [Header("次元武器")]
    public GameObject target; //鎖定鼠標位置
    static public int storeTimes; //儲存技能次數    
    private float reStoreTime; //回複次數時間點
    public float reStoreTimeSet; //回複次數間隔
    public Image storeFillAmount; //次數現存圖案
    public GameObject storeFillGameObject; //次數現存物件
    public GameObject jumpEffect; //次元武器特效
    static public int jumpNo = 0; //次元是否被封印

    [Header("技能相關")]
    static public float critChance = 0; //爆擊機率
    static public float critDamage = 1.5f; //爆擊傷害

    private void Start()
    {
        bulletArm = bulletBase; //初始槍械
        bulletMode = 0;
        force = 20;
        timeSet = 0.1f;
        Bullet.atackDamageStatic = 20;
        bulletArm.gameObject.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        bulletArm.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        nowUseBullet = 0;
        bulletObject[0].GetComponent<Animator>().SetBool("Choose", true);

        //激活槍械圖標
        if (NowSkill.nowSkillLevelInGame[0] == 1)
        { 
            bulletObject[1].SetActive(true);
            storeFillGameObject.SetActive(true);
        }
        if (NowSkill.nowSkillLevelInGame[1] == 1) bulletObject[2].SetActive(true); 
        if (NowSkill.nowSkillLevelInGame[2] == 1) bulletObject[3].SetActive(true);
        if (NowSkill.nowSkillLevelInGame[4] == 1) bulletObject[5].SetActive(true);
        if (NowSkill.nowSkillLevelInGame[5] == 1)
        {
            bulletObject[6].SetActive(true);
            energyFillGameObject.SetActive(true);
        }

        if (bulletBaseGive) //給予基本子彈
        {
            bulletNowNum[0] = 100;
            bulletBaseGive = false;
        }

        SkillDetect(); //初始化技能
        jumpNo = 0; //重置是否被封印次元跳躍
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime; //計算時間

        energyFillAmount.fillAmount = nowTrackerEnergy / maxTrackerEnergy; //顯示能量
        storeFillAmount.fillAmount = storeTimes / 5f; //顯示次數

        WeaponRotation();
        ArmDetect();

        if(Input.GetMouseButtonDown(1)) //手刀
        {
            Attack();
        }

        if(Input.GetMouseButton(0) && time >= timeSet && bulletMode == 0 /*&& bulletNowNum[nowUseBullet] > 0*/) //已超過發射間隔，允許攻擊，火力武器
        {
            for(int i=0 ; i<=onceBulletNum ; i++)
                Fire(bulletArm);
            time = 0;
        }
        else if(Input.GetMouseButton(0) && time >= timeSet && bulletMode == 1) //允許攻擊，追蹤彈
        {
            

            if(timeStartTracker == 0)
            {
                timeStartTracker = Time.deltaTime; //設立累積傷害開始點
            }

            //更換模式
            if (alreadyShootBullet > 10 && alreadyShootBullet <= 30)
            {
                trackerBulletMode = 2;
            } 
            else if (alreadyShootBullet > 30)
            {
                trackerBulletMode = 4;
            }
            else trackerBulletMode = 0;
            
            //發射
            for(int i=0 ; i<= trackerBulletMode && nowTrackerEnergy >= 10 ; i++)
            {
                ShadowPool.instance.BulletGetFromPool();
                Bullet.atackDamageStatic += 0.5f;
                alreadyShootBullet++;
                nowTrackerEnergy -= 10;
            }
            time = 0;
        }
        else if(Input.GetMouseButtonDown(0) && bulletMode == 2 && storeTimes > 0 && time >= timeSet && jumpNo == 0)
        {
            storeTimes--;
            Instantiate(jumpEffect, target.transform.position, jumpEffect.transform.rotation);
            Debug.Log(storeTimes);
            Vector3 vector3 = target.transform.position;
            StartCoroutine(Jump(vector3));
        }

        //如果為追蹤彈模式，更新能量
        if(bulletMode == 1)
        {
            timeTracker += Time.deltaTime; //計算累積時間
            reEnergyTime += Time.deltaTime; //計算能量回復時間

            if ((timeTracker - timeStartTracker) > timeSetTracker) //重置追蹤彈累積
            {
                Bullet.atackDamageStatic = NormalBullet.atkTracker;
                alreadyShootBullet = 0;
                timeTracker = 0;
                timeStartTracker = 0;
            }

            if (nowTrackerEnergy < maxTrackerEnergy && reEnergyTime >= reEnergyTimeSet) //回複能量
            {
                nowTrackerEnergy+=5;
                reEnergyTime = 0;
            }
        }

        //如果次數不足，回復次數
        if(storeTimes < 5)
        {
            reStoreTime += Time.deltaTime;

            if(reStoreTime > reStoreTimeSet)
            {
                storeTimes++;
                reStoreTime = 0;
            }
        }

        FireChange();

        //顯示子彈
        for (int i = 0; i < 6; i++)
        {
            if (i == 1) continue; //忽略次元武器(不需要更新子彈數)
            if (bulletObject[i] != null)
                bulletObject[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = bulletNowNum[i].ToString();
        }
    }

    private void WeaponRotation() //武器旋轉
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position; //鼠標位置到發射點位置
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private void ArmDetect()
    {
        RaycastHit2D hitIofo = Physics2D.Raycast(firePoint.position, direction, maxDist, mask); //在右上角顯示目標資訊，未擊中目標時，顯示自己的
        if(hitIofo.collider != null)
        {
            if (hitIofo.collider.gameObject.tag == "Enemy")
            {
                boardImage.gameObject.SetActive(true);
                boardText.text = hitIofo.collider.gameObject.name;
                lv.text = "LV:" + (ClassManager.levelNum - 2).ToString();
                atk.text = "Atk:" + FireElement.atkStatic.ToString();
                hp.text = "Hp:" + FireElement.hpFireElementStatic.ToString();
            }
            else 
            {
                boardImage.gameObject.SetActive(true);
                boardText.text = "Player";
                lv.text = "LV:" + PlayerMove.playerLevel.ToString();
                atk.text = "Atk:" + Bullet.atackDamageStatic.ToString();
                hp.text = "Hp:" + PlayerMove.hpStatic.ToString();
            }

        }
    }

    private void Fire(GameObject bulletNow) //攻擊
    {
        GameObject newBullet = Instantiate(bulletNow, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * force, ForceMode2D.Impulse);
        bulletNowNum[nowUseBullet]--;
    }

    //註釋中的數字為遊戲物件的編號(實際技能編號+1)
    public void FireChange()
    {
        if (Input.GetKeyDown(KeyCode.E)) //步槍:0
        {
            ResetGun();
            force = 20;
            timeSet = 0.1f;
            Bullet.atackDamageStatic = 20;
            bulletArm.gameObject.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
            bulletArm.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            nowUseBullet = 0;
            bulletObject[0].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
        if (Input.GetKeyDown(KeyCode.R) && NowSkill.nowSkillLevelInGame[0] == 1) //次元武器:1，bulletMode=2
        {
            ResetGun();
            timeSet = 0.1f;
            bulletMode = 2;
            bulletObject[1].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
        if (Input.GetKeyDown(KeyCode.T) && NowSkill.nowSkillLevelInGame[1] == 1) //散彈槍:2
        {
            ResetGun();
            onceBulletNum = 10;
            force = 10;
            timeSet = 0.5f;
            Bullet.atackDamageStatic = 10;
            bulletArm.gameObject.GetComponent<SpriteRenderer>().color = new Color(246 / 255f, 0 / 255f, 180 / 255f, 255 / 255f);
            bulletArm.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            nowUseBullet = 2;
            bulletObject[2].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
        if (Input.GetKeyDown(KeyCode.Y) && NowSkill.nowSkillLevelInGame[2] == 1) //加特林:3
        {
            ResetGun();
            force = 10;
            timeSet = 0.01f;
            Bullet.atackDamageStatic = 3;
            bulletArm.gameObject.GetComponent<SpriteRenderer>().color = new Color(245 / 255f, 152 / 255f, 8 / 255f, 255 / 255f);
            bulletArm.gameObject.transform.localScale = new Vector3(0.4f, 0.6f, 1f);
            nowUseBullet = 3;
            bulletObject[3].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
        /*if (Input.GetKeyDown(KeyCode.Y) && NowSkill.nowSkillLevelInGame[3] == 1) //雷射:4
        {
            ResetGun();
            Bullet.atackDamageStatic = 2;
            bulletMode = 2;

            SkillDetect();
        }*/
        if (Input.GetKeyDown(KeyCode.G) && NowSkill.nowSkillLevelInGame[4] == 1) //狙擊槍:5
        {
            ResetGun();
            force = 50;
            timeSet = 1f;
            Bullet.atackDamageStatic = 200;
            bulletArm.gameObject.GetComponent<SpriteRenderer>().color = new Color(61 / 255f, 0 / 255f, 255 / 255f, 255 / 255f);
            bulletArm.gameObject.transform.localScale = new Vector3(0.6f, 0.3f, 1f);
            nowUseBullet = 5;
            bulletObject[5].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
        if(Input.GetKeyDown(KeyCode.H) && NowSkill.nowSkillLevelInGame[5] == 1) //追蹤彈:6
        {
            ResetGun();
            timeSet = 0.5f;
            Bullet.atackDamageStatic = NormalBullet.atkTracker;
            bulletMode = 1;
            bulletObject[6].GetComponent<Animator>().SetBool("Choose", true);
            SkillDetect();
        }
    }

    public void ResetGun()
    {
        for (int i = 0; i < 7; i++)
        {
            if(bulletObject[i] != null) bulletObject[i].GetComponent<Animator>().SetBool("Choose", false);
        }
        onceBulletNum = 0;
        bulletMode = 0;
    }

    public void SkillDetect() //技能檢測，需要用到再添加
    {
        if (NowSkill.nowSkillLevelInGame[6] > 0) //敏捷，增加移動速度
        {
            float increase = (5f + NowSkill.nowSkillLevelInGame[6]) / 5f;
            PlayerMove.moveSpeed = increase + 5f;
            Debug.Log(PlayerMove.moveSpeed);
        }
        if (NowSkill.nowSkillLevelInGame[7] > 0) //暴力，增加攻擊傷害，1/10
        {
            float increase = (10f + NowSkill.nowSkillLevelInGame[7]) / 10f;
            Bullet.atackDamageStatic *= increase;
        }
        if (NowSkill.nowSkillLevelInGame[8] > 0) //幸運兔腳，增加爆級機率，1/10
        {
            critChance = NowSkill.nowSkillLevelInGame[8];
        }
        if (NowSkill.nowSkillLevelInGame[9] > 0) //快槍手，減少攻擊間隔，1/20
        {
            float decrease = (20f - NowSkill.nowSkillLevelInGame[10]) / 20f;
            timeSet *= decrease;
        }
        if (NowSkill.nowSkillLevelInGame[10] > 0) //槍_速度部件，減少攻擊間隔，1/20
        {
            float decrease = (20f - NowSkill.nowSkillLevelInGame[10]) / 20f;
            timeSet *= decrease;
        }
        if (NowSkill.nowSkillLevelInGame[11] > 0) //瘋狂，增加爆擊傷害，1/5
        {
            float increase = NowSkill.nowSkillLevelInGame[11] / 3f;
            critDamage = increase + 1.5f;
        }
    }

    IEnumerator Jump(Vector3 vector3)
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.transform.parent.gameObject.transform.position = vector3;
    }

    private void Attack() //刀刃出現位置
    {
        //鼠標位置「目標點位置」-當前位置「人物所在位置」
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; //Radius > Degree 弧度轉角度
        transform.GetChild(2).gameObject.transform.rotation = Quaternion.Euler(0, 0, rotZ);

        slashGameObject.gameObject.SetActive(true);
    }
}

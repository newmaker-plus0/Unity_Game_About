using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class EnemyBeast : MonoBehaviour
{
    [Header("怪物:野獸 基礎屬性")]
    [SerializeField] public float moveSpeed;
    private Transform target;
    [SerializeField] private float maxHp;
    public float hp;
    public int atk;

    [Header("受傷效果調整")]
    private SpriteRenderer sp;
    public float hurtLemgh; //效果持續時間
    private float hurtCounter; //相當於計數器

    [HideInInspector]
    public bool isAttacked;
    public GameObject explosionEffect;

    public Image hpImage;
    public Image hpEffectImage;
    [SerializeField] private float hurtSpeed = 0.005f;

    public GameObject damageCanvas;

    int on=0;

    private void Start()
    {
        int dou = 1;
        for(int i=1;i<(ClassManager.levelNum/2)-1;i++)
        {
            dou *= 2;
        }

        atk = (ClassManager.levelNum/2) + 10;
        maxHp = dou * 100;
        hp = maxHp;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FollowPlayer();
   
        if (hurtCounter <= 0) //受傷效果
        {
            sp.material.SetFloat("_FlashAmount", 0);
        }
        else
        {
            hurtCounter -= Time.deltaTime;
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

        Turn();

        if (hp <= 0) //死亡
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            EnemyAppearManager.nowEnemyCount -= 1;
            ScoreInformation.scorePower += 100 * ClassManager.levelNum / 20;
            Destroy(gameObject);
        }
    }

    private void FollowPlayer() //追蹤目標
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    public void TakenDamage(float _amount) //怪物受到攻擊
    {
        isAttacked = true;
        for(int i=0;i<=PlayerAttack.slashNum;i++) 
        {
            int cri=0;

            if (Random.Range(1, Buff.critChance) == 1)
            {
                ScoreInformation.scorePower += (int)_amount;
                cri = 1;
                _amount *= Random.Range(Buff.critDamage - 0.5f, Buff.critDamage + 0.5f);
            }

            hp -= _amount;

            Vector3 vector3 = new Vector3(this.transform.position.x + Random.Range(0.1f, 3.0f), this.transform.position.y + Random.Range(0.1f, 3.0f), this.transform.position.z);
            DamageNum damagable = Instantiate(damageCanvas, vector3, Quaternion.identity).GetComponent<DamageNum>();
            damagable.ShowUIDamage(Mathf.RoundToInt(_amount), cri);
            cri = 0;
        }
        StartCoroutine(isAttackCo());
        HurtShader();
    }

    private void HurtShader() //受傷效果
    {
        sp.material.SetFloat("_FlashAmount", 1);
        hurtCounter = hurtLemgh;
    }

    IEnumerator isAttackCo() //防止重複扣血
    {
        yield return new WaitForSeconds(0.2f);
        isAttacked = false;
    }

    private void OnTriggerEnter2D(Collider2D other) //傷害
    {
        if (other.gameObject.tag == "Player") //玩家
        {
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

            if (!player.isAttacked)
            {
                player.TakenDamage(atk-5, atk+5);
            }
        }

        if (other.gameObject.tag == "slash") //怪物
        {
            float minDamage = Slash.minDamage;
            float maxDamage = Slash.maxDamage;
            float attackDamage = Random.Range(minDamage, maxDamage);

            if (Buff.bloodEffectOn == 1)
            {
               if(on==0) InvokeRepeating("BloodEffect", 0.0f, 1f);
            }

            if (!isAttacked)
            {
                TakenDamage(attackDamage);              

                Vector2 diffrence = transform.position - other.transform.position;
                diffrence.Normalize();
                transform.position = new Vector2(transform.position.x + diffrence.x, transform.position.y + diffrence.y);
            }
        }

        if (other.gameObject.tag == "bullet")
        {
            Bullet(other);
        }
    }

    private void Turn() //翻轉
    {
        //if (moveH > 0)
        if (transform.position.x < target.transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        //if (moveH < 0)
        if (transform.position.x > target.transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void BloodEffect() //流血效果
    {
        on = 1;
        hp -= Buff.bloodEffectDamage;
        ScoreInformation.scorePower += Buff.bloodEffectDamage / 10;

        Vector3 vector3 = new Vector3(this.transform.position.x + Random.Range(1, 3), this.transform.position.y + Random.Range(1, 3), this.transform.position.z);
        DamageNum damagable = Instantiate(damageCanvas, vector3, Quaternion.identity).GetComponent<DamageNum>();
        damagable.ShowUIDamage(Mathf.RoundToInt(Buff.bloodEffectDamage), 0);
    }

    public void Bullet(Collider2D other) //子彈
    {
        float _amount = NormalBullet.atk;

        int cri = 0;

        if (Random.Range(1, Buff.critChance) == 1)
        {
            ScoreInformation.scorePower += (int)_amount;
            cri = 1;
            _amount *= Random.Range(Buff.critDamage - 0.5f, Buff.critDamage + 0.5f);

        }

        hp -= _amount;

        Vector3 vector3 = new Vector3(this.transform.position.x + Random.Range(0.1f, 3.0f), this.transform.position.y + Random.Range(0.1f, 3.0f), this.transform.position.z);
        DamageNum damagable = Instantiate(damageCanvas, vector3, Quaternion.identity).GetComponent<DamageNum>();
        damagable.ShowUIDamage(Mathf.RoundToInt(_amount), cri);
        cri = 0;

        Vector2 diffrence = transform.position - other.transform.position;
        diffrence.Normalize();
        transform.position = new Vector2(transform.position.x + diffrence.x / 2, transform.position.y + diffrence.y / 2);

        HurtShader();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
    [Header("怪物:鬼  基礎屬性")]
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

    private Transform target;

    int on = 0;

    // Start is called before the first frame update
    void Start()
    {
        Boss10_Rank1.ghostNum += 1;
        Boss10.damageDown -= 0.1f;
        Boss10_Rank1.atk += 1;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        int dou = 1;
        for (int i = 0; i < (ClassManager.levelNum / 10) - 1; i++)
        {
            dou *= 4;
        }

        atk = (ClassManager.levelNum / 2) + 5;
        maxHp = dou * 5000;
        hp = maxHp;
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    private void Update()
    {
        Turn();

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

        if (hp <= 0 || Boss10.hp<=0) //死亡
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Boss10.damageDown += 0.1f;
            Boss10_Rank1.atk -= 1;
            Boss10_Rank1.ghostNum -= 1;

            ScoreInformation.scorePower += 500 * ClassManager.levelNum / 20;

            Destroy(gameObject);
        }
    }

    public void TakenDamage(float _amount) //怪物受到攻擊
    {
        isAttacked = true;
        for (int i = 0; i <= PlayerAttack.slashNum; i++)
        {
            int cri = 0;

            if (Random.Range(1, Buff.critChance) == 1)
            {
                ScoreInformation.scorePower += (int)_amount;
                cri = 1;
                _amount *= Random.Range(Buff.critDamage - 0.5f, Buff.critDamage + 0.5f);
            }

            hp -= _amount;

            Vector3 vector3 = new Vector3(this.transform.position.x + Random.Range(0.1f, 7.0f), this.transform.position.y + Random.Range(0.1f, 7.0f), this.transform.position.z);
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
        if (other.gameObject.tag == "slash") //怪物
        {
            float minDamage = Slash.minDamage;
            float maxDamage = Slash.maxDamage;
            float attackDamage = Random.Range(minDamage, maxDamage);

            if (Buff.bloodEffectOn == 1)
            {
                if (on == 0) InvokeRepeating("BloodEffect", 0.0f, 1f);
            }

            if (!isAttacked)
            {
                TakenDamage(attackDamage);

                Vector2 diffrence = transform.position - other.transform.position;
                diffrence.Normalize();
                transform.position = new Vector2(transform.position.x + diffrence.x / 2, transform.position.y + diffrence.y / 2);
            }
        }

        if (other.gameObject.tag == "bullet")
        {
            Bullet(other);
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

    public void Bullet(Collider2D other) //子彈
    {
        float _amount = NormalBullet.atk;

        int cri = 0;

        if (Random.Range(1, 10) == 1)
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

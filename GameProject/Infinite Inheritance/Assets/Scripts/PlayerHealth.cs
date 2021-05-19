using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("基礎屬性載入")]
    [SerializeField] public float maxHp;
    static public float hp;
    static public float nowHp;
    static public float maxNowHp;

    static public float hpUp=0.0f;

    [Header("受傷效果調整")]
    public bool isAttacked;
    private float attackDamage;
    private Animator anim;

    public Image hpImage;
    public Image hpEffectImage;
    [SerializeField] private float hurtSpeed = 0.005f;

    public GameObject damageCanvas;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = Load01.maxHp;
        hp = Load01.hp;
        
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        maxNowHp = maxHp;
        nowHp = hp;
        hpImage.fillAmount = hp / maxHp; //血量顯示

        if(hpEffectImage.fillAmount > hpImage.fillAmount) //血條緩衝掉血
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }

        if (hp <= 0)
        {
            Load01.save = 0;
            Restart();
        }
    }

    public void TakenDamage(int min, int max) //玩家受到攻擊
    {
        if (!isAttacked)
        {
            attackDamage = Random.Range(min, max);
            hp -= attackDamage;
            DamageNum damagable = Instantiate(damageCanvas, transform.position, Quaternion.identity).GetComponent<DamageNum>();
            damagable.ShowUIDamage(Mathf.RoundToInt(attackDamage), 0);
            StartCoroutine(InvincibleCo());

            if(hp <= 0)
            {
                Load01.save = 0;
                Restart();               
            }
        }
    }

    IEnumerator InvincibleCo() //受傷無敵時間
    {
        isAttacked = true;
        anim.SetBool("isAttacked", true);
        yield return new WaitForSeconds(2.0f);
        anim.SetBool("isAttacked", false);
        isAttacked = false;
    }

    public void HpUp()
    {
        maxHp += hpUp;
        hp += hpUp;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    
    public void BossAttack()
    {
        DamageNum damagable = Instantiate(damageCanvas, transform.position, Quaternion.identity).GetComponent<DamageNum>();
        damagable.ShowUIDamage(Mathf.RoundToInt(Boss10_Rank1.bossDamage), 0);
    }
}

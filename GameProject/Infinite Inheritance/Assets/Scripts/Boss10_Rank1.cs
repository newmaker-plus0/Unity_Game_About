using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss10_Rank1 : MonoBehaviour
{
    private Transform target;
    public GameObject boss;
    public GameObject other;
    protected AudioSource audioSource;

    static public int hit = 0; //幾連擊

    static public int atk = 10;

    static public int set = 0; //召喚

    static public int attack = 0, bossDamage=0, hitAfter=0; //是否命中

    public GameObject damageCanvas;

    public GameObject ghost;

    static public int ghostNum=0;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        InvokeRepeating("AdMode", 4f, 4f);
        InvokeRepeating("Ghost", 5f, 5f);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Boss10.hp > Boss10.maxHp) Boss10.hp = Boss10.maxHp;

        if(hitAfter==1)
        {
            Vector2 diffrence = target.transform.position - this.transform.position;
            diffrence.x = diffrence.x % 3;
            diffrence.y = diffrence.y % 3;

            while(hitAfter<3)
            {
                float x = target.transform.position.x + diffrence.x;
                float y = target.transform.position.y + diffrence.y;
                if( x<-15.8 || x>13.75 )
                {
                    break;
                }else if( y<-10.7 || y>10.85 )
                {
                    break;
                }

                target.transform.position = new Vector2(target.transform.position.x + diffrence.x, target.transform.position.y + diffrence.y);
                hitAfter += 1;
            }

            hitAfter = 0;
        }        
    }

    public void AdMode()
    {
        hit = 0;
        attack = 0;
        Vector2 diffrence = target.transform.position - this.transform.position;

        if (diffrence.x >= 0) diffrence.x = -2;
        else diffrence.x = 2;

        if (diffrence.y >= 0) diffrence.y = -2;
        else diffrence.y = 2;

        boss.gameObject.transform.position = new Vector2(target.position.x + diffrence.x, target.position.y + diffrence.y);
        StartCoroutine(Ad());
    }

    public void Admode1()
    {
        ;
    }

    private void Attack() //刀刃出現位置
    {
        audioSource.Play();
        //鼠標位置「目標點位置」-當前位置「人物所在位置」
        Vector2 difference = target.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; //Radius > Degree 弧度轉角度
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        transform.GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator Ad()
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

        if (attack == 0)
        {
            yield return new WaitForSeconds(0.1f);
            Attack();
            yield return new WaitForSeconds(0.4f);
            if (attack == 1)
            {
                if(hit==1)
                {
                    PlayerHealth.hp -= atk;

                    bossDamage = atk;
                    GameObject.Find("Player").SendMessage("BossAttack");                   
                    
                    set = 1;
                    StartCoroutine(Set());
                    Attack();
                    yield return new WaitForSeconds(0.4f);
                    if(hit==2)
                    {
                        PlayerHealth.hp -= atk + 5;

                        bossDamage = atk + 5;
                        GameObject.Find("Player").SendMessage("BossAttack");

                        Boss10.hp += (atk + 5) * 35 * (ClassManager.levelNum % 10) * 6;

                        DamageNum damagable = Instantiate(damageCanvas, transform.position, Quaternion.identity).GetComponent<DamageNum>();
                        damagable.ShowUIDamage(Mathf.RoundToInt((atk + 5) * 10000), 0);

                        Attack();
                        yield return new WaitForSeconds(0.4f);
                        if(hit==3)
                        {
                            PlayerHealth.hp -= Boss10_Rank1.atk + 10;

                            bossDamage = atk + 10;
                            GameObject.Find("Player").SendMessage("BossAttack");
                            
                            hitAfter = 1;
                        }
                    }
                }
            }
        }
    }

    IEnumerator Set()
    {
        yield return new WaitForSeconds(0.6f);
        Boss10_Rank1.set = 0;
    }

    public void Ghost()
    {
        if (ghostNum < 10)
        {
            Instantiate(ghost, target.position, boss.transform.rotation);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject boomEffectBase; //基本爆炸效果

    public float time=3f; //子彈自毀時間
    static public float atackDamageStatic=20; //公開子彈傷害

    private void Start()
    {
        boomEffectBase.GetComponent<SpriteRenderer>().color = Weapon.bulletArm.gameObject.GetComponent<SpriteRenderer>().color;
        Destroy(gameObject, time); //自毀
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Wall") //碰牆銷毀
        {
            Instantiate(boomEffectBase, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Enemy") //碰敵銷毀
        {
            Instantiate(boomEffectBase, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

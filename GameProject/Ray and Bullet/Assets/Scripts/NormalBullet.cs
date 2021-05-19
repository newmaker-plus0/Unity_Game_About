using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : MonoBehaviour
{
    public GameObject player;
    private Transform target;
    static public float atkTracker = 5;

    [Header("時間控制參數")]
    public float activeTime;
    public float activeStart;

    int rotationAngle=0;

    public float bulletSpeed;

    private void OnEnable()
    {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        rotationAngle = Random.Range(-80, 80);

        activeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > activeStart + activeTime)
        {
            //返回對象池
            ShadowPool.instance.BulletReturnPool(this.gameObject);
            
        }
        Chase();
    }

    public void Chase()
    {
        Vector3 direction = target.transform.position - this.transform.position; //朝向目標
        float ang = 360 - Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg; //方向向量轉角度
        transform.eulerAngles = new Vector3(0, 0, ang); //角度設為對應角度

        this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, rotationAngle);
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy") //敵人
        {
            ShadowPool.instance.BulletReturnPool(this.gameObject);
        }
    }
}

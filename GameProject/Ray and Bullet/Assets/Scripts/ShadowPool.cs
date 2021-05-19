using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    public static ShadowPool instance;

    public GameObject bulletPrefab;

    public int bulletShadowCount;
    
    private Queue<GameObject> bulletAvailableObjects = new Queue<GameObject>();

    private Transform target;

    void Awake()
    {
        instance = this;

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //初始化對象池
        BulletFillPool();
    }

    public void BulletFillPool() //子彈
    {
        for (int i = 0; i < bulletShadowCount; i++)
        {
            var newShadow = Instantiate(bulletPrefab);
            newShadow.transform.SetParent(transform);

            //取消啟用，返回對象池
            BulletReturnPool(newShadow);
        }
    }

    public void BulletReturnPool(GameObject gameObject) //子彈
    {
        gameObject.SetActive(false);
        bulletAvailableObjects.Enqueue(gameObject);
    }

    public GameObject BulletGetFromPool() //子彈
    {
        if (bulletAvailableObjects.Count == 0)
        {
            BulletFillPool();
        }
        var outShadow = bulletAvailableObjects.Dequeue();

        outShadow.SetActive(true);
        outShadow.transform.position = target.GetChild(1).transform.position;

        return outShadow;
    }
}

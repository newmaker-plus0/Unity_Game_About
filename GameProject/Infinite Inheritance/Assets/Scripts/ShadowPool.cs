using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    public static ShadowPool instance;

    public GameObject shadowPrefab;
    public GameObject bulletPrefab;

    public int shadowCount;
    public int bulletShadowCount;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private Queue<GameObject> bulletAvailableObjects = new Queue<GameObject>();

    private Transform target;

    void Awake()
    {
        instance = this;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        //初始化對象池
        FillPool();
        BulletFillPool();
    }

    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++)
        {
            var newShadow = Instantiate(shadowPrefab);
            newShadow.transform.SetParent(transform);

            //取消啟用，返回對象池
            ReturnPool(newShadow);
        }
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

    public void ReturnPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
        availableObjects.Enqueue(gameObject);
    }

    public void BulletReturnPool(GameObject gameObject) //子彈
    {
        gameObject.SetActive(false);
        bulletAvailableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            FillPool();
        }
        var outShadow = availableObjects.Dequeue();

        outShadow.SetActive(true);

        return outShadow;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    private Transform target;

    public float speed;
    public float nextWaypointDistance; //檢測周遭某距離內是否有障礙物

    public Transform enemyGFX;

    Path path; //路徑
    int currentWaypoint = 0; //現在所在點
    bool reachEndOfPath = false; //是否到達目標點

    Seeker seeker; //尋找的類型
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        InvokeRepeating("UpdatePath", 0f, 0.5f); //每0.5秒刷新路線
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete); //是否尋找到路線
    }

    void OnPathComplete(Path p)
    {
        if(!p.error) //找到
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) //沒路徑，回傳
            return;

        if(currentWaypoint >= path.vectorPath.Count) //是否抵達目標位置(現在點大於路徑點的數量)
        {
            reachEndOfPath = true;
            return;
        }else
        {
            reachEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime; //計算力

        rb.AddForce(force); //透過力，推動敵人

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]); //計算目標與自己的距離

        if(distance < nextWaypointDistance) //如果小於新距離，更換新的路徑
        {
            currentWaypoint++;
        }

        if(force.x >= 0.01f) //是否轉向
        {
            enemyGFX.localScale = new Vector3(enemyGFX.localScale.x, enemyGFX.localScale.y, enemyGFX.localScale.z);
        }
        else if(force.x <= -0.01f) 
        {
            enemyGFX.localScale = new Vector3(-Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
        }
    }
}

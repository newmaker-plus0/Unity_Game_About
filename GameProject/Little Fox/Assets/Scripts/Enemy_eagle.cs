using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_eagle : Enemy
{
    private Rigidbody2D rb;
    public Transform top, buttom;
    public float Speed;
    private float Topy, Buttomy;

    private bool isup = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        Topy = top.position.y;
        Buttomy = buttom.position.y;
        Destroy(top.gameObject);
        Destroy(buttom.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    //腳色移動
    void Movement()
    {
        if (isup)
        {
            rb.velocity = new Vector2(rb.velocity.x, Speed);
            if (transform.position.y > Topy)
            {
                isup = false;
            }
        }else
        {
            rb.velocity = new Vector2(rb.velocity.x, -Speed);
            if(transform.position.y < Buttomy)
            {
                isup = true;
            }
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_Controller : Enemy
{
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Animator anim;

    public Collider2D coll;
    public LayerMask ground;
    public AudioSource audioSource;
    public Transform CellingCheck;
    public Collider2D DisColl;
    public Transform groundCheck;

    public float speed = 10f;
    public float jumpForce;   
    public int Cherry;

    public Text CherryNum;
    private bool isHurt;
    public bool isGround, isJump;

    bool jumpPressed;
    int jumpCount;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);

        if(!isHurt)
        {
        Movement();
        Jump();
        }
        SwitchAnim();
    }

    //腳色移動
    void Movement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float facedircetion = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove*speed, rb.velocity.y);
        anim.SetFloat("running", Mathf.Abs(facedircetion));

        //腳色移動
        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }

        //腳色跳躍
        

        Crouch();
    }

    void Jump()
    {
        if(isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            SoundManager.instance.JumpAudio();
            jumpCount--;
            jumpPressed = false;
        }
        else if(jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            SoundManager.instance.JumpAudio();
            jumpCount--;
            jumpPressed = false;
        }
    }

    //動畫切換
    void SwitchAnim()
    {
        anim.SetBool("idle", true);
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));

        if (isGround)
        {
            anim.SetBool("falling", false);
        }
        else if (!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
        }
        else if(rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }
        if(isHurt)
        {
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0);
            if(Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurt", false);
                anim.SetBool("idle", true);
                isHurt = false;
            }
        }
    }

    //碰撞觸發器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //收集
        if(collision.tag == "Collection")
        {
            SoundManager.instance.CherryAudio();
            Destroy(collision.gameObject);
            Cherry += 1;
            CherryNum.text = Cherry.ToString();
        }

        //重啟
        if (collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            Invoke("Restart", 2f);
        }
    }

    //消滅敵人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))
            {      
            enemy.JumpOn();            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
            jumpCount++;
            }else if(transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-10, rb.velocity.y);
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
        } 
    }

    //趴下
    void Crouch()
    {
        if (!Physics2D.OverlapCircle(CellingCheck.position, 0.2f, ground))
        {
            if (Input.GetButton("Crouch"))
            {
                anim.SetBool("crouch", true);
                DisColl.enabled = false;
            }
            else
            {
                anim.SetBool("crouch", false);
                DisColl.enabled = true;
            }
        }       
    }

    //重啟
    void Restart()
    {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public Animator anim;
    public GameObject playerCamera, bowSr, bowCenter;
    public SpriteRenderer sr;
    public Text playerNameText;

    //人物操作
    public float moveSpeed;
    public float jumpForce;
    public bool disableInput = false;

    //生命值
    public float defaultHp = 100f, nowHp = 100f;
    public GameObject fillImageHp, localFIH, customFIH, playerCanvas;

    private void Awake()
    {
        if(photonView.isMine)
        {
            fillImageHp = localFIH;
            playerCamera.SetActive(true);
            playerNameText.text = PhotonNetwork.playerName;
            GameManager.instance.localPlayer = this.gameObject;
        }
        else
        {
            fillImageHp = customFIH;
            playerNameText.text = photonView.owner.name;
            playerNameText.color = Color.cyan;
        }

        fillImageHp.SetActive(true);
    }

    private void Update()
    {
        if(photonView.isMine && !disableInput)
        {
            CheckInput();
        }
    }

    public void CheckInput()
    {
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
        transform.position += move * moveSpeed * Time.deltaTime;

        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x)
        {
            photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }

        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
        //bowSr.transform.localEulerAngles = new Vector3(0f, 180f, bowSr.transform.localEulerAngles.z);
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
        //bowSr.transform.localEulerAngles = new Vector3(0f, 0f, bowSr.transform.localEulerAngles.z);
    }

    [PunRPC] 
    public void ReduceHealth(float amount)
    {
        ModifyHealth(amount);
    }

    [PunRPC]
    public void Dead()
    {
        rb.gravityScale = 0;
        bc.enabled = false;
        sr.enabled = false;
        bowCenter.SetActive(false);
        playerCanvas.SetActive(false);
    }

    [PunRPC]
    public void Respawn()
    {
        rb.gravityScale = 1;
        bc.enabled = true;
        sr.enabled = true;
        bowCenter.SetActive(true);
        playerCanvas.SetActive(true);
        nowHp = defaultHp;
        fillImageHp.GetComponent<Image>().fillAmount = 1f;
    }

    private void CheckHealth()
    {
        if(photonView.isMine && nowHp <= 0)
        {
            GameManager.instance.EnableRespawn();
            this.GetComponent<PhotonView>().RPC("Dead", PhotonTargets.AllBuffered);
        }
    }

    public void ModifyHealth(float amount)
    {
        if(photonView.isMine)
        {
            nowHp -= amount;
            fillImageHp.transform.GetChild(1).GetComponent<Image>().fillAmount = nowHp / defaultHp;
        }
        else
        {
            nowHp -= amount;
            fillImageHp.transform.GetChild(1).GetComponent<Image>().fillAmount = nowHp / defaultHp;
        }

        CheckHealth();
    }
}

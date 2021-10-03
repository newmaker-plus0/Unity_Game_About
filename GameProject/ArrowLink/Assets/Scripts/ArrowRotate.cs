using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotate :Photon.MonoBehaviour
{
    private bool getRb = true;
    private Rigidbody2D rb;
    private bool hasHit = false;
    public float destroyTime = 4f;

    //數值
    public float damage;

    // Update is called once per frame
    void Update()
    {
        if(transform.GetComponent<Rigidbody2D>() != null && hasHit == false)
        {
            if (getRb) GetRb();
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void GetRb()
    {
        rb = GetComponent<Rigidbody2D>();
        getRb = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.isMine)
        {
            return;
        }

        /*hasHit = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;*/

        PhotonView target = collision.GetComponent<PhotonView>();

        //撞到其他物件
        if (collision.tag == "Wall")
        {
            StartCoroutine("DestroyByTime");
        }

        if(target != null && (!target.isMine || target.isSceneView))
        {
            if(collision.tag == "Player")
            {
                target.RPC("ReduceHealth", PhotonTargets.AllBuffered, damage);
                this.GetComponent<PhotonView>().RPC("DestroyObject", PhotonTargets.AllBuffered);
            }
        }
    }

    IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(destroyTime);
        this.GetComponent<PhotonView>().RPC("DestroyObject", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}

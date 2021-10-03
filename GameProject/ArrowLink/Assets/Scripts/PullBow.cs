using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBow : Photon.MonoBehaviour
{
    //射擊相關
    private Vector2 startFingerPos, nowFingerPos, diretion;
    public Vector2 bowPos;
    public LineRenderer myStringLine;
    private float length, arrowStartX;
    public float shootForce = 25f;
    public GameObject arrow, shootPoint, newArrow;
    private bool isExitArrow = false, isPull = false;
    private int shootViewID, arrowViewID;

    //拖動延遲判定
    private float time;
    public float timeInterval;

    public float arrowDamage = 10f;

    private void Start()
    {
        arrowStartX = shootPoint.transform.localPosition.x;
    }

    private void Update()
    {
        if(photonView.isMine)
        {
            JudgeFingle();
            shootViewID = transform.GetComponent<PhotonView>().viewID;
        }
    }

    public void JudgeFingle()
    {
        //點擊
        if (Input.GetMouseButtonDown(0))
        {
            startFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(startFingerPos);
        }

        //延遲判定
        if (Input.GetMouseButton(0) && time < timeInterval)
        {
            time += Time.deltaTime;
        }

            //拉動
        if (Input.GetMouseButton(0) && time >= timeInterval)
        {
            //生成箭矢
            if (!isExitArrow)
            {
                newArrow = PhotonNetwork.Instantiate(arrow.name, new Vector2(this.transform.position.x, this.transform.position.y), 
                shootPoint.transform.rotation, 0);
                arrowViewID = newArrow.GetComponent<PhotonView>().viewID;
                //newArrow.transform.parent = this.transform;
                //newArrow.transform.localPosition = shootPoint.transform.localPosition;
                this.GetComponent<PhotonView>().RPC("Locate", PhotonTargets.AllBuffered, shootViewID, arrowViewID);
                isExitArrow = true;
            }

            isPull = true;
            nowFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //弓箭旋轉
            bowPos = transform.position;
            diretion = bowPos - nowFingerPos;
            transform.right = diretion * 10f;

            //弓弦拉動
            length = diretion.magnitude / 10f;
            length = Mathf.Clamp(length, 0f, 0.8f);

            //弓箭拉動
            Vector3 arrowPos = newArrow.transform.localPosition;
            arrowPos.x = (arrowStartX - length);
            newArrow.transform.localPosition = arrowPos;

            myStringLine.SetPosition(1, new Vector3(0.1f + (length * -1f), 0, 0));
            //Debug.Log(nowFingerPos);
        }

        //放開
        if(Input.GetMouseButtonUp(0))
        {
            shootForce = shootForce * Mathf.Clamp(length, 0.5f, 2f);

            isExitArrow = false;
            time = 0f;
            if(isPull) this.GetComponent<PhotonView>().RPC("ShootArrow", PhotonTargets.AllBuffered, shootViewID, arrowViewID);
            isPull = false;

            length = 0;
            myStringLine.SetPosition(1, new Vector3(0.1f + (length * -1f), 0, 0));
        }
    }

    [PunRPC]
    private void ShootArrow(int shootID, int arrowID)
    {
        Transform shoot = PhotonView.Find(shootID).transform;
        GameObject arrow = PhotonView.Find(arrowID).gameObject;
        arrow.GetComponent<ArrowRotate>().damage = arrowDamage;
        arrow.transform.parent = null;
        if(arrow.GetComponent<Rigidbody2D>() == null)
        {
            arrow.AddComponent<Rigidbody2D>();
            arrow.AddComponent<PhotonRigidbody2DView>();
            arrow.GetComponent<PhotonView>().ObservedComponents.Add(arrow.GetComponent<PhotonRigidbody2DView>());
            arrow.GetComponent<Rigidbody2D>().velocity = transform.right * shootForce;
            shootForce = 25f;
        }
    }

    [PunRPC]
    public void Locate(int shootID, int arrowID)
    {
        Transform shoot = PhotonView.Find(shootID).transform;
        GameObject arrow = PhotonView.Find(arrowID).gameObject;
        arrow.transform.parent = shoot;
        arrow.transform.localPosition = shootPoint.transform.localPosition;
    }
}

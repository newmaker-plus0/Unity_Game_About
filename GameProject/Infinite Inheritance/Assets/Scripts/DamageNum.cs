using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNum : MonoBehaviour
{
    [Header("傷害數字顯示")]
    public Text damageText;
    public float lifeTimer;
    public float upSpeed;

    private void Start()
    {
        Destroy(gameObject, lifeTimer);
    }

    private void Update()
    {
        transform.position += new Vector3(0, upSpeed * Time.deltaTime, 0); //文字上浮效果
    }

    public void ShowUIDamage(float _amount, int cri) //傷害數字顯示
    {
        if(cri==1)
        {
        damageText.text = "cri" + _amount.ToString();
        }
        else
        {
            damageText.text = _amount.ToString();
        }
    }
}

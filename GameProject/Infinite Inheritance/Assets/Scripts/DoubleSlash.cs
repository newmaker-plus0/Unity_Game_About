using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSlash : MonoBehaviour
{
    public void EndAttack() //攻擊結束
    {
        gameObject.SetActive(false);
    }
}

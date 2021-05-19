using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    //關閉手刀動畫
    public void CloseSlash()
    {
        gameObject.SetActive(false);
    }
}

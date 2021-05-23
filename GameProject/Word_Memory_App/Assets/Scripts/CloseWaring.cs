using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWaring : MonoBehaviour
{
    public GameObject waringHome;

    //關閉
    public void Close()
    {
        waringHome.SetActive(false);
    }
}

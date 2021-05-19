using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss10_Slash : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") //魔王的劍
        {
            Boss10_Rank1.attack = 1;
            Boss10_Rank1.hit += 1;
        }
    }
}

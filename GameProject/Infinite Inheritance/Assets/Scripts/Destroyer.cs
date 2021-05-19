using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] public float lifeTimer;

    private void Start()
    {
        Destroy(gameObject, lifeTimer); //銷毀遊戲物件
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator Anim;

    protected AudioSource deathAudio;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Anim = GetComponent<Animator>();
        deathAudio = GetComponent<AudioSource>();
    }

    //死亡
    public void Death()
    {        
        Destroy(gameObject);
    }

    //死亡效果
    public void JumpOn()
    {
        deathAudio.Play();
        Anim.SetTrigger("death");
    }
}

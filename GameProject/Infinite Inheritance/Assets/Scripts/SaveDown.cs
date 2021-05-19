using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDown : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Save());
        }
    }

    IEnumerator Save()
    {
        anim.SetBool("save", true);
        yield return new WaitForSeconds(1.0f);
        anim.SetBool("save", false);
    }
}

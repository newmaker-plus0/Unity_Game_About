using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTempo : MonoBehaviour
{
    public Text tempo;
    int tempoNum = 4;

    // Start is called before the first frame update
    void Start()
    {
        tempoNum = 4;
        InvokeRepeating("Tempo", 0.0f, 1.0f);
    }

    public void Tempo()
    {
        if (tempoNum <= 0) tempoNum = 4;
        tempo.text = tempoNum.ToString();
        tempoNum -= 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreInformation : MonoBehaviour
{
    [Header("分數")]
    public Text score;
    static public int scorePower = 0;
    public int score1;

    private void Update()
    {
        score1 = scorePower;
        Power();
    }

    public void Power()
    {
        score.text = score1.ToString();
    }
}

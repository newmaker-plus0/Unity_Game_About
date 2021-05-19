using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGunMode : MonoBehaviour
{
    public void Open()
    {
        PlayerAttack.change = 1;
    }

    public void Close()
    {
        PlayerAttack.change = 2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIntro : MonoBehaviour
{
    public Text maxHp;
    public Text hp;
    public Text speed;
    public Text slashNum;
    public Text dashTime, dashCoolDown, dashSpeed;
    public Text bloosEffectDamage;
    public Text critDamage;
    public Text critChance;
    public Text scaleX;
    public Text max, min;

    // Update is called once per frame
    void Update()
    {
        maxHp.text = PlayerHealth.maxNowHp.ToString();
        hp.text = PlayerHealth.nowHp.ToString();
        speed.text = PlayerMovement.nowSpeed.ToString();
        slashNum.text = PlayerAttack.slashNum.ToString();
        dashTime.text = PlayerMovement.dashTime.ToString();
        dashCoolDown.text = PlayerMovement.dashCoolDown.ToString();
        dashSpeed.text = PlayerMovement.dashSpeed.ToString();
        bloosEffectDamage.text = Buff.bloodEffectDamage.ToString();
        critDamage.text = Buff.critDamage.ToString();
        critChance.text = Buff.critChance.ToString();
        scaleX.text = Buff.nowScaleX.ToString();
        max.text = Slash.maxDamage.ToString();
        min.text = Slash.minDamage.ToString();
    }
}

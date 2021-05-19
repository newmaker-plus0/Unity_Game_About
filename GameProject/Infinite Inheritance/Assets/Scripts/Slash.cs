using UnityEngine;

public class Slash : MonoBehaviour
{
    public static float minDamage=30;
    public static float maxDamage=50;

    public void EndAttack() //攻擊結束
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(minDamage>maxDamage)
        {
            float tem = maxDamage;
            maxDamage = minDamage;
            minDamage = maxDamage;

        }
    }
}

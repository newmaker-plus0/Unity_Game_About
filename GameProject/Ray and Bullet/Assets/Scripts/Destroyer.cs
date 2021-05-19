using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float timer; //自爆時間

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer); //自爆
    }
}

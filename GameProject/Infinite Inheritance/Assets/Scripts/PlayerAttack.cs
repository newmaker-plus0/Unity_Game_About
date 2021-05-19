using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    static public int slashNum=0;
    protected AudioSource audioSource;

    private float time;
    private float timeSet;

    static public int change = 2;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timeSet = 0.1f;
        InvokeRepeating("Re", 0f, 10f);
    }

    // Update is called once per frame
    private void Update()
    {
        time += Time.deltaTime;

        if (change == 1)
        {
            if (Input.GetButton("Fire1") && time >= timeSet)
            {
                ShadowPool.instance.BulletGetFromPool(); NormalBullet.atk += 2;
                ShadowPool.instance.BulletGetFromPool(); NormalBullet.atk += 2;
                ShadowPool.instance.BulletGetFromPool(); NormalBullet.atk += 2;
                NormalBullet.atk += 2;
                time = 0;
            }
        }else if (change == 2)
        {
            if(Input.GetMouseButtonDown(0)) //無效的螢幕震動
                {
                    Attack();
                    StartCoroutine(FindObjectOfType<CameraController>().CameraShake(0.2f, 0.2f));
                }
        }
    }

    private void Attack() //刀刃出現位置
    {
        audioSource.Play();
        //鼠標位置「目標點位置」-當前位置「人物所在位置」
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; //Radius > Degree 弧度轉角度
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        
        if(slashNum>=1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Re()
    {
        NormalBullet.atk = 5 * ClassManager.levelNum + ClassManager.levelNum * 1;
    }
}

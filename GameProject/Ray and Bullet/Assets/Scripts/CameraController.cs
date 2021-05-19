using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target; //鎖定目標
    [SerializeField] private float smoothSpeed; //滑動速度
    [SerializeField] private float minX, minY, maxX, maxY; //區間角落，超出區間，攝影機再移動

    // Start is called before the first frame update
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }
}

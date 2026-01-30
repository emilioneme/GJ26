using UnityEngine;

public class SpotlightSpin : MonoBehaviour
{
    [SerializeField]
    public GameObject spotlightTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up * speed * Time.deltaTime);
        Vector3 direction = (transform.position - spotlightTarget.transform.position).normalized;
        Vector3 zeroedDir = new Vector3(direction.x, 0, direction.z);
        transform.rotation = Quaternion.LookRotation(zeroedDir);
    }
}

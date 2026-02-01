using UnityEngine;

public class SpotlightSpin : MonoBehaviour
{
    [SerializeField]
    public GameObject spotlightTarget;
    public GameObject spotlightOnRail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (transform.position - spotlightTarget.transform.position).normalized;
        Vector3 zeroedDir = new Vector3(direction.x, 0, direction.z);
        Vector3 spotlightOnRailDir = new Vector3(direction.y, 0, 0);
        transform.rotation = Quaternion.LookRotation(zeroedDir);
        spotlightOnRail.transform.rotation = Quaternion.LookRotation(direction);
    }
}

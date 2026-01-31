using UnityEngine;

public class StandingInmateRotation : MonoBehaviour
{
    [SerializeField]
    public GameObject groupTarget;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (groupTarget.transform.position - transform.position).normalized;
        Vector3 zeroedDir = new Vector3(direction.x, 0, direction.z);
        transform.rotation = Quaternion.LookRotation(zeroedDir);
    }
}

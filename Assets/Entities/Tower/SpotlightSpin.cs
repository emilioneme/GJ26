using UnityEngine;

public class SpotlightSpin : MonoBehaviour
{
    [SerializeField]
    public GameObject spotlightTarget;
    public GameObject spotlightOnRail;
    public bool playerTarget = false;
    [SerializeField] float yawWobble = 0.3f;    // horizontal amount
    [SerializeField] float pitchWobble = 0.3f;  // vertical amount
    [SerializeField] float wobbleSpeed = 0.5f;
    float yawSeed;
    float pitchSeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yawSeed = Random.value * 100f;
        pitchSeed = Random.value * 100f;
    }

    // Update is called once per frame
    void Update()
    {
        // if (playerTarget)
        // {
        //     Vector3 playerDirection = (transform.position - GameManager.Instance.player.transform.position).normalized;
        //     Vector3 zeroedPlayerDir = new Vector3(playerDirection.x + Random.Range(-1, 1), 0, playerDirection.z + Random.Range(-1, 1));
            
        //     transform.rotation = Quaternion.LookRotation(zeroedPlayerDir);
        //     spotlightOnRail.transform.rotation = Quaternion.LookRotation(playerDirection);
        //     return;
        // }
        if (playerTarget)
        {
            this.GetComponent<SpotlightCollider>().playerTarget = true;

            Vector3 playerDirection =
                (transform.position - GameManager.Instance.player.transform.position).normalized;

            // smooth offsets (replace Random.Range)
            float yawOffset =
                (Mathf.PerlinNoise(Time.time * wobbleSpeed, yawSeed) - 0.5f) * yawWobble;

            float pitchOffset =
                (Mathf.PerlinNoise(Time.time * wobbleSpeed, pitchSeed) - 0.5f) * pitchWobble;

            Vector3 zeroedPlayerDir = new Vector3(
                playerDirection.x + yawOffset,
                0,
                playerDirection.z + yawOffset
            );

            // base rotation (unchanged logic)
            transform.rotation = Quaternion.LookRotation(zeroedPlayerDir);

            // spotlight vertical rotation (unchanged logic + pitch)
            spotlightOnRail.transform.rotation =
                Quaternion.LookRotation(playerDirection + Vector3.up * pitchOffset);

            return;
        }

        this.GetComponent<SpotlightCollider>().playerTarget = false;

        Vector3 direction = (transform.position - spotlightTarget.transform.position).normalized;
        Vector3 zeroedDir = new Vector3(direction.x, 0, direction.z);
        
        transform.rotation = Quaternion.LookRotation(zeroedDir);
        spotlightOnRail.transform.rotation = Quaternion.LookRotation(direction);
    }
}

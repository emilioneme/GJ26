using UnityEditor;
using UnityEngine;

public class GuardWatchingBehaviour : MonoBehaviour
{
    [SerializeField] float damageCooldown = 0.5f;
    [SerializeField] float minBlendingEfficacy = 0.6f;
    [SerializeField] float damageMultiplier = 3f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float sphereCastRadius = 3f;
    [SerializeField] public Animator an;

    [SerializeField] GameObject guardSound;
    
    BlendingHandler blendingHandler;
    float lastTimeDamaged = 0;

    MovingController mc;
    DesiredDirection dd;
    InmateRotationInput ri;

    bool hasPlayedIdleSound = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mc = transform.GetComponent<MovingController>();
        dd = transform.GetComponent<DesiredDirection>();
        ri = transform.GetComponent<InmateRotationInput>();
    }

    void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            sphereCastRadius,
            layerMask
        );

        blendingHandler = null;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out BlendingHandler bh))
            {
                blendingHandler = bh;
                StandStill(hit.gameObject);
                return;
            }
        }

        ReturnToDefault();
    }

    public void ReturnToDefault()
    {
        mc.moveSpeed = 2;
        dd.enabled = true;
        ri.enabled = true;
        an.SetBool("IsIdle", false);
        hasPlayedIdleSound = false;
    }

    public void StandStill(GameObject target)
    {
        if (!hasPlayedIdleSound)
        {
            Destroy(Instantiate(guardSound, transform));
            hasPlayedIdleSound = true;
        }
        mc.moveSpeed = 0;
        dd.enabled = false;
        ri.enabled = false;
        an.SetBool("IsIdle", true);
        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void RunToTarget(Vector3 target)
    {
        GameObject dummy = new GameObject("RunTarget");
        dummy.transform.position = target;
        dd.target = dummy.transform;
    }

    // Update is called once per frame
    void Update()
    {

        if(blendingHandler == null)
        {
            return;
        }           
        if (Time.time - lastTimeDamaged > damageCooldown)
        {
            //Debug.Log("BlendingEfficacy: " + blendingHandler.currentBlendingEfficacy);
            if(blendingHandler.currentBlendingEfficacy < minBlendingEfficacy)
            {
                float damage = Mathf.InverseLerp(1 - minBlendingEfficacy, -1, blendingHandler.currentBlendingEfficacy) * damageMultiplier;
                //GameManager.Instance.RaiseAlert(damage);
                //Debug.Log(damage);
                lastTimeDamaged = Time.time;
            }
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.TryGetComponent(out BlendingHandler bh))
    //     {
    //         blendingHandler = bh;
    //         Debug.Log("Entered guard view");
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.TryGetComponent(out BlendingHandler bh) && bh == blendingHandler)
    //     {
    //         blendingHandler = null;
    //         Debug.Log("Exited guard view");
    //     }
    // }
}

using UnityEngine;

public class StandingTargetColliderScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player detected");
            BlendingHandler bh = other.gameObject.GetComponent<BlendingHandler>();
            bh.standingTarget = this.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player detected leaving");
            BlendingHandler bh = other.gameObject.GetComponent<BlendingHandler>();
            bh.standingTarget = null;
        }
    }

}

using UnityEngine;

public class SpotlightCollider : MonoBehaviour
{
    [SerializeField] float damageCooldown = 0.5f;
    [SerializeField] float minBlendingEfficacy = 0.5f;

    BlendingHandler blendingHandler;

    public bool playerTarget;
    
    
    float lastTimeDamaged = 0;

    public void Update()
    {
        if (playerTarget)
        {
            blendingHandler = GameManager.Instance.player.GetComponent<BlendingHandler>();
        }

        if (blendingHandler == null)
            return;

        if (Time.time - lastTimeDamaged > damageCooldown)
        {
            Debug.Log("BlendingEfficacy: " + blendingHandler.currentBlendingEfficacy);
            if(blendingHandler.currentBlendingEfficacy < minBlendingEfficacy)
            {
                float damage = Mathf.InverseLerp(1 - minBlendingEfficacy, -1, blendingHandler.currentBlendingEfficacy);
                GameManager.Instance.RaiseAlert(damage);
                //Debug.Log(damage);
                lastTimeDamaged = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BlendingHandler bh))
        {
            blendingHandler = bh;
            Debug.Log("Entered spotlight");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BlendingHandler bh) && bh == blendingHandler)
        {
            blendingHandler = null;
            Debug.Log("Exited spotlight");
        }
    }
}

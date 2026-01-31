using UnityEngine;

public class SpotlightCollider : MonoBehaviour
{
    [SerializeField] float damageCooldown = 1f;
    [SerializeField] float minBlendingEfficacy = 0.5f;

    BlendingHandler blendingHandler;
    
    float lastTimeDamaged = 0;

    public void Update()
    {
        if (blendingHandler == null)
            return;

        if (Time.time - lastTimeDamaged > damageCooldown)
        {
            Debug.Log("BlendingEfficacy: " + blendingHandler.currentBlendingEfficacy);
            if(blendingHandler.currentBlendingEfficacy < minBlendingEfficacy)
            {
                float damage = Mathf.InverseLerp(1 - minBlendingEfficacy, -1, blendingHandler.currentBlendingEfficacy);
                GameManager.Instance.TakeDamage(damage);
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
